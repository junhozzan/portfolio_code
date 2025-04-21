using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UILab
{
    public class CpUI_LabCell : MyOSABasicItem
    {
        [SerializeField] UIText nameText = null;
        [SerializeField] UIText levelValueText = null;
        [SerializeField] UIText descText = null;
        [SerializeField] GameObject allUpButton = null;
        [SerializeField] CpUI_CostButton upButton = null;


        private ResourceLab resLab = null;
        private LoopCmd cmdUpButton = null;
        private Cmd cmdAllUpButton = null;

        private long useVirtualAmount = 0;

        public void Init()
        {
            cmdUpButton = new LoopCmd(this, upButton.gameObject, new LevelUpLoopFunctions(this), 0.1f);
            cmdAllUpButton = Cmd.Add(allUpButton, eCmdTrigger.OnClick, Cmd_AllLevelUp);
        }

        public override void DoReset()
        {
            upButton.DoReset();
        }

        public override void Refresh()
        {
            RefreshName();
            RefreshButtons();
            RefrshByLevel();
        }

        private void RefreshName()
        {
            if (nameText == null || resLab == null)
            {
                return;
            }

            nameText.SetText(resLab.GetName());
            //nameText.SetFitterHorizontal();
        }

        private void RefrshByLevel()
        {
            RefreshLevelText();
            RefreshDescText();
        }

        private void RefreshLevelText()
        {
            if (levelValueText == null || resLab == null)
            {
                return;
            }

            var level = MyPlayer.Instance.core.lab.GetLevel(resLab.id);
            levelValueText.SetText(level.ToString());
        }

        private void RefreshDescText()
        {
            if (descText == null || resLab == null)
            {
                return;
            }

            if (resLab.targetAbilities.Count == 0)
            {
                return;
            }

            var ability = resLab.targetAbilities[0];
            if (ability.statItems.Count == 0)
            {
                return;
            }

            var statItem = ability.statItems[0];
            var level = MyPlayer.Instance.core.lab.GetLevel(resLab.id);

            descText.SetText(resLab.desc.L(StatItem.ValueToString(statItem.stat, statItem.GetValue(StatItem.Param.RiseParam.Of(level, 0), 1f))));
        }

        private void RefreshButtons()
        {
            if (resLab == null)
            {
                return;
            }

            upButton.DoReset();

            var isMaxLevel = IsMaxLevel();
            var isLackNextWealth = IsLackNextWealth();
            var activate = true;
            activate &= activate ? !isMaxLevel : activate;
            activate &= activate ? !isLackNextWealth : activate;

            cmdUpButton.Use(activate);
            cmdAllUpButton.Use(activate);

            var text = string.Empty;
            var color = Color.white;
            if (isMaxLevel)
            {
                text = "MAX";
            }
            else
            {
                var resWealthItem = ResourceManager.Instance.item.GetItem(resLab.costItemID);
                if (resWealthItem != null)
                {
                    upButton.SetIcon(resWealthItem.appearance);
                }

                text = UIBase.GetCostText(GetNeedPrice());
                color = isLackNextWealth ? GameData.COLOR.RED_FONT : Color.white;
            }

            upButton.SetValueText(text, color);
        }

        private long GetNeedPrice()
        {
            var level = MyPlayer.Instance.core.lab.GetLevel(resLab.id);
            return resLab.GetNeedCost(level, level + 1);
        }

        private bool IsMaxLevel()
        {
            return MyPlayer.Instance.core.lab.GetLevel(resLab.id) >= resLab.maxLevel;
        }

        private bool IsLackNextWealth()
        {
            var level = MyPlayer.Instance.core.lab.GetLevel(resLab.id);
            var amount = MyPlayer.Instance.core.item.GetAmount(resLab.costItemID);
            return resLab.GetNeedCost(level, level + 1) > amount;
        }

        public override void UpdateViews(MyOSABasic.IOsaItem tOsaItem)
        {
            if (!(tOsaItem is CpUI_Lab.LabOsaItem osaItem))
            {
                return;
            }

            resLab = osaItem.resLab;
            Refresh();
        }

        private void VirtualLevelUp()
        {
            if (resLab == null)
            {
                return;
            }

            var currlevel = MyPlayer.Instance.core.lab.GetLevel(resLab.id);
            var nextLevel = currlevel + 1;

            useVirtualAmount += -resLab.GetNeedCost(currlevel, nextLevel);

            MyPlayer.Instance.core.item.SetVirtualAmount(resLab.costItemID, useVirtualAmount);
            MyPlayer.Instance.core.lab.AddVirtualLevel(resLab.id, 1);

            GameEvent.Instance.AddEvent(GameEventType.UPDATE_LAB_VIRTUAL);
        }

        private void LevelUp(int addLevel)
        {
            if (resLab == null)
            {
                return;
            }

            useVirtualAmount = 0;
            MyPlayer.Instance.core.item.SetVirtualAmount(resLab.costItemID, 0);
            MyPlayer.Instance.core.lab.LevelUp(resLab.id, addLevel);
        }

        private void Cmd_AllLevelUp()
        {
            if (resLab == null)
            {
                return;
            }

            var currLevel = MyPlayer.Instance.core.lab.GetLevel(resLab.id);
            var currAmount = MyPlayer.Instance.core.item.GetAmount(resLab.costItemID);

            var addLevel = 0;
            var totalWealth = 0L;
            for (int i = 0; i < resLab.maxLevel - currLevel; ++i)
            {
                var vLevel = currLevel + i;
                totalWealth += resLab.GetNeedCost(vLevel, vLevel + 1);
                if (totalWealth > currAmount)
                {
                    break;
                }

                ++addLevel;
            }

            UIBase.ClickSound();
            LevelUp(addLevel);
        }

        private class LevelUpLoopFunctions : LoopCmd.Functions
        {
            private readonly CpUI_LabCell cell = null;

            public LevelUpLoopFunctions(CpUI_LabCell cell)
            {
                this.cell = cell;
            }

            public override void OnLoop(int loopCount)
            {
                UIBase.LoopClickSound();
                cell.VirtualLevelUp();
            }

            public override void OnComplete(int loopCount)
            {
                cell.LevelUp(loopCount);
            }

            public override bool IsBreak(int loopCount)
            {
                if (cell.IsMaxLevel())
                {
                    return true;
                }

                if (cell.IsLackNextWealth())
                {
                    return true;
                }

                return false;
            }
        }
    }
}