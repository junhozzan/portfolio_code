using System.Collections.Generic;
using UnityEngine;
using System;

namespace UIPopup
{
    public class CpUI_PopupFrame_ItemUtil : CpUI_PopupFrame_Base
    {
        [SerializeField] UIInventory.CpUI_Inventory_ItemInfoFrame itemInfoFrame = null;
        [SerializeField] UIText optionTextOrigin = null;
        [SerializeField] UIRadio radio = null;
        [SerializeField] GameObject enhanceRadioTab = null;
        [SerializeField] GameObject awakenRadioTab = null;
        [SerializeField] GameObject rerollRadioTab = null;
        [SerializeField] CpUI_CostButton enhanceButton = null;
        [SerializeField] CpUI_CostButton awakenButton = null;
        [SerializeField] CpUI_CostButton rerollButton = null;
        [SerializeField] GameObject enhanceAllButton = null;
        [SerializeField] GameObject exitButton = null;

        private readonly List<TextParam> optionTexts = new List<TextParam>();
        private readonly List<TextParam> optionTextsInner = new List<TextParam>();
        private ObjectPool<UIText> optionTextPool = null;
        private Cmd cmdEnhance = null;
        private Cmd cmdEnhanceAll = null;
        private Cmd cmdReroll = null;
        private Cmd cmdAwaken = null;

        private float[] minOptions = null;
        private float[] maxOptions = null;

        private int itemID = 0;

        public override void Init(CpUI_Popup parent, Action<CpUI_PopupFrame_Base> onCloseAt)
        {
            base.Init(parent, onCloseAt);

            itemInfoFrame.Init();
            radio.Init(OnRadioEvent);

            minOptions = Util.InitArray(minOptions, GameData.DEFAULT.MAX_ITEM_OPTION_COUNT, 0f);
            maxOptions = Util.InitArray(maxOptions, GameData.DEFAULT.MAX_ITEM_OPTION_COUNT, 1f);

            optionTextPool = ObjectPool<UIText>.Of(optionTextOrigin, optionTextOrigin.transform.parent);

            cmdEnhance = Cmd.Add(enhanceButton.gameObject, eCmdTrigger.OnClick, Cmd_Enhance);
            cmdEnhanceAll = Cmd.Add(enhanceAllButton, eCmdTrigger.OnClick, Cmd_EnhanceAll);
            cmdReroll = Cmd.Add(rerollButton.gameObject, eCmdTrigger.OnClick, Cmd_Reroll);
            cmdAwaken = Cmd.Add(awakenButton.gameObject, eCmdTrigger.OnClick, Cmd_Awaken);

            Cmd.Add(exitButton, eCmdTrigger.OnClick, Cmd_Close);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                    .Add(GameEventType.UPDATE_ITEM_AMOUNT, Handle_UPDATE_ITEM_AMOUNT)
                    .Add(GameEventType.UPDATE_ITEM_ALL, Handle_UPDATE_ITEM_ALL)
                    .Add(GameEventType.EQUIP_ITEM, Handle_EQUIP_ITEM)
                    ;
        }

        private void Handle_UPDATE_ITEM_AMOUNT(object[] args)
        {
            RefreshUIWealth();
            RefreshOptions();
        }

        private void Handle_UPDATE_ITEM_ALL(object[] args)
        {
            RefreshInfoFrame();
            RefreshUIWealth();
            RefreshOptions();
        }

        private void Handle_EQUIP_ITEM(object[] args)
        {
            RefreshInfoFrame();
            RefreshUIWealth();
            RefreshOptions();
        }

        private void SetDefault()
        {
            ResetButtons();
        }

        private void ResetButtons()
        {
            enhanceButton.DoReset();
            enhanceButton.gameObject.SetActive(false);

            rerollButton.DoReset();
            rerollButton.gameObject.SetActive(false);

            awakenButton.DoReset();
            awakenButton.gameObject.SetActive(false);

            enhanceAllButton.SetActive(false);

            cmdEnhance.Use(false);
            cmdEnhanceAll.Use(false);
            cmdReroll.Use(false);
            cmdAwaken.Use(false);
        }

        private void RefreshRadioTab()
        {
            var resItem = ResourceManager.Instance.item.GetItem(itemID);
            if (resItem == null)
            {
                return;
            }

            enhanceRadioTab.SetActive(resItem.enhance.use);
            awakenRadioTab.SetActive(resItem.awaken.use);
            rerollRadioTab.SetActive(resItem.reroll.use);

            GameObject openTab = null;
            if (enhanceRadioTab.activeSelf)
            {
                openTab = enhanceRadioTab;
            }
            else if (awakenRadioTab.activeSelf)
            {
                openTab = awakenRadioTab;
            }
            else if (rerollRadioTab.activeSelf)
            {
                openTab = rerollRadioTab;
            }
            else
            {
                openTab = null;
            }

            radio.Choice(openTab);
        }

        public CpUI_PopupFrame_ItemUtil On(int itemID)
        {
            this.itemID = itemID;

            SetDefault();
            RefreshRadioTab();
            RefreshInfoFrame();
            RefreshUIWealth();
            RefreshOptions();

            return this;
        }

        //private void Refresh()
        //{
        //    RefreshInfoFrame();
        //    RefreshUIWealth();
        //    RefreshOptions();
        //}

        private void RefreshOptions()
        {
            if (radio.current == enhanceRadioTab)
            {
                // 강화
                RefreshButtonByEnhance();
                RefreshOptionTextByEnhance();
            }
            else if (radio.current == awakenRadioTab)
            {
                // 초월
                RefreshButtonByAwaken();
                RefreshOptionTextByAwaken();
            }
            else if (radio.current == rerollRadioTab)
            {
                // 옵션 변경
                RefreshButtonByReroll();
                RefreshOptionTextByReroll();
            }
        }

        private void RefreshUIWealth()
        {
            var resItem = ResourceManager.Instance.item.GetItem(itemID);
            if (resItem == null)
            {
                return;
            }

            SetWealthParams(resItem.awaken.costItemID, resItem.reroll.costItemID, resItem.enhance.costItemID);
        }

        private void RefreshInfoFrame()
        {
            if (!MyPlayer.Instance.core.item.TryGetItem(itemID, out var item))
            {
                return;
            }

            itemInfoFrame.Refresh(item);
        }

        private void RefreshOptionTextByEnhance()
        {
            optionTextPool.Clear();
            optionTexts.Clear();

            if (!MyPlayer.Instance.core.item.TryGetItem(itemID, out var item))
            {
                return;
            }

            var resItem = item.resItem;
            if (resItem == null)
            {
                return;
            }

            var itemAmount = item.GetAmount();
            var currLevel = item.GetLevel();
            var maxLevel = resItem.GetMaxLevel(item.GetAwaken());
            if (currLevel >= maxLevel)
            {
                var text = optionTextPool.Pop();
                text.SetTextColor(GameData.COLOR.STAT_MAX);
                text.SetText(string.Format("key_ex_level_max".L(), $"Lv.{maxLevel}"));
                return;
            }

            var nextLevel = currLevel + 1;
            var itemOptions = item.GetOptions();

            var currRiseParam = StatItem.Param.RiseParam.Of(currLevel, itemAmount);
            var nextRiseParam = StatItem.Param.RiseParam.Of(nextLevel, itemAmount);

            optionTexts.Add(TextParam.Of(GameData.COLOR.ITEM_INFO_MAIN, "key_level".L()));
            optionTexts.Add(TextParam.Of(GameData.COLOR.STAT_PREV, $"Lv.{currLevel} <color=#{GameData.COLOR.STAT_NEXT.hex}>▶ Lv.{nextLevel}</color>"));

            optionTextsInner.Clear();
            foreach (var ability in resItem.equip.targetAbilities)
            {
                foreach (var statItem in ability.statItems)
                {
                    var currText = statItem.ToString(currRiseParam, statItem.GetOption(itemOptions));
                    var nextText = statItem.ToString(nextRiseParam, statItem.GetOption(itemOptions));
                    if (currText == nextText)
                    {
                        continue;
                    }

                    optionTextsInner.Add(TextParam.Of(GameData.COLOR.STAT_PREV, $"{currText} <color=#{GameData.COLOR.STAT_NEXT.hex}>▶ {nextText}</color>"));
                }
            }

            if (optionTextsInner.Count > 0)
            {
                optionTexts.Add(TextParam.empty);
                optionTexts.Add(TextParam.Of(GameData.COLOR.ITEM_INFO_MAIN, "key_equip_ability".L()));
                optionTexts.AddRange(optionTextsInner);
                optionTextsInner.Clear();
            }

            foreach (var ability in resItem.hold.targetAbilities)
            {
                foreach (var statItem in ability.statItems)
                {
                    var currText = statItem.ToString(currRiseParam, statItem.GetOption(itemOptions));
                    var nextText = statItem.ToString(nextRiseParam, statItem.GetOption(itemOptions));
                    if (currText == nextText)
                    {
                        continue;
                    }

                    optionTextsInner.Add(TextParam.Of(GameData.COLOR.STAT_PREV, $"{currText} <color=#{GameData.COLOR.STAT_NEXT.hex}>▶ {nextText}</color>"));
                }
            }

            if (optionTextsInner.Count > 0)
            {
                optionTexts.Add(TextParam.empty);
                optionTexts.Add(TextParam.Of(GameData.COLOR.ITEM_INFO_MAIN, "key_hold_ability".L()));
                optionTexts.AddRange(optionTextsInner);
                optionTextsInner.Clear();
            }

            if (resItem.appearance.TryGetLevelToSpecialText(currLevel, out var v1)
                && resItem.appearance.TryGetLevelToSpecialText(nextLevel, out var v2))
            {
                if (v1 != v2)
                {
                    optionTexts.Add(TextParam.empty);
                    optionTexts.Add(TextParam.Of(GameData.COLOR.ITEM_INFO_MAIN, "key_special_ability".L()));
                    optionTexts.Add(TextParam.Of(GameData.COLOR.STAT_PREV, v1.L()));
                    optionTexts.Add(TextParam.Of(GameData.COLOR.STAT_NEXT, $"▶ {v2.L()}"));
                }
            }

            foreach (var optionText in optionTexts)
            {
                var text = optionTextPool.Pop();
                text.SetTextColor(optionText.color);
                text.SetText(optionText.text);
            }
        }

        private void RefreshOptionTextByReroll()
        {
            optionTextPool.Clear();
            optionTexts.Clear();

            if (!MyPlayer.Instance.core.item.TryGetItem(itemID, out var item))
            {
                return;
            }

            var resItem = item.resItem;
            if (resItem == null)
            {
                return;
            }

            var itemLevel = item.GetLevel();
            var itemAmout = item.GetAmount();
            var itemOptions = item.GetOptions();
            var riseParam = StatItem.Param.RiseParam.Of(itemLevel, itemAmout);

            optionTextsInner.Clear();
            foreach (var ability in resItem.equip.targetAbilities)
            {
                foreach (var statItem in ability.statItems)
                {
                    var statText = StatItem.TypeToLocailzeKey(statItem.stat).L();
                    var currText = StatItem.ValueToString(statItem.stat, statItem.GetValue(riseParam, statItem.GetOption(itemOptions)));
                    var minText = StatItem.ValueToString(statItem.stat, statItem.GetValue(riseParam, statItem.GetOption(minOptions)));
                    var maxText = StatItem.ValueToString(statItem.stat, statItem.GetValue(riseParam, statItem.GetOption(maxOptions)));
                    var maxMark = statItem.GetOption(itemOptions) >= 1f ? $"<color=#{GameData.COLOR.STAT_MAX.hex}> (Max)</color>" : string.Empty;

                    optionTextsInner.Add(TextParam.Of(GameData.COLOR.STAT_PREV, $"{statText} {currText}{maxMark} <color=#{GameData.COLOR.STAT_NEXT.hex}>({minText} ~ {maxText})</color>"));
                }
            }

            if (optionTextsInner.Count > 0)
            {
                optionTexts.Add(TextParam.empty);
                optionTexts.Add(TextParam.Of(GameData.COLOR.ITEM_INFO_MAIN, "key_equip_ability".L()));
                optionTexts.AddRange(optionTextsInner);
                optionTextsInner.Clear();
            }

            optionTextsInner.Clear();
            foreach (var ability in resItem.hold.targetAbilities)
            {
                foreach (var statItem in ability.statItems)
                {
                    var statText = StatItem.TypeToLocailzeKey(statItem.stat).L();
                    var currText = StatItem.ValueToString(statItem.stat, statItem.GetValue(riseParam, statItem.GetOption(itemOptions)));
                    var minText = StatItem.ValueToString(statItem.stat, statItem.GetValue(riseParam, statItem.GetOption(minOptions)));
                    var maxText = StatItem.ValueToString(statItem.stat, statItem.GetValue(riseParam, statItem.GetOption(maxOptions)));
                    var maxMark = statItem.GetOption(itemOptions) >= 1f ? $"<color=#{GameData.COLOR.STAT_MAX.hex}> (Max)</color>" : string.Empty;

                    optionTextsInner.Add(TextParam.Of(GameData.COLOR.STAT_PREV, $"{statText} {currText}{maxMark} <color=#{GameData.COLOR.STAT_NEXT.hex}>({minText} ~ {maxText})</color>"));
                }
            }

            if (optionTextsInner.Count > 0)
            {
                optionTexts.Add(TextParam.empty);
                optionTexts.Add(TextParam.Of(GameData.COLOR.ITEM_INFO_MAIN, "key_hold_ability".L()));
                optionTexts.AddRange(optionTextsInner);
                optionTextsInner.Clear();
            }

            foreach (var optionText in optionTexts)
            {
                var text = optionTextPool.Pop();
                text.SetTextColor(optionText.color);
                text.SetText(optionText.text);
            }
        }

        private void RefreshOptionTextByAwaken()
        {
            optionTextPool.Clear();
            optionTexts.Clear();

            if (!MyPlayer.Instance.core.item.TryGetItem(itemID, out var item))
            {
                return;
            }

            var resItem = item.resItem;
            if (resItem == null)
            {
                return;
            }

            var itemAwaken = item.GetAwaken();
            if (resItem.awaken.maxAwaken > 0 && itemAwaken >= resItem.awaken.maxAwaken)
            {
                var text = optionTextPool.Pop();
                text.SetTextColor(GameData.COLOR.STAT_MAX);
                text.SetText(string.Format("key_ex_limit_max".L(), $"Lv.{resItem.awaken.maxAwaken}"));
                return;
            }

            optionTexts.Add(TextParam.Of(GameData.COLOR.ITEM_INFO_MAIN, "key_awaken".L()));
            optionTexts.Add(TextParam.Of(GameData.COLOR.STAT_PREV, $"Lv.{itemAwaken} <color=#{GameData.COLOR.STAT_NEXT.hex}>▶ Lv.{itemAwaken + 1}</color>"));

            optionTexts.Add(TextParam.empty);
            optionTexts.Add(TextParam.Of(GameData.COLOR.ITEM_INFO_MAIN, "key_max_level".L()));
            optionTexts.Add(TextParam.Of(GameData.COLOR.STAT_PREV, $"Lv.{resItem.GetMaxLevel(itemAwaken)} <color=#{GameData.COLOR.STAT_NEXT.hex}>▶ Lv.{resItem.GetMaxLevel(itemAwaken + 1)}</color>"));

            foreach (var optionText in optionTexts)
            {
                var text = optionTextPool.Pop();
                text.SetTextColor(optionText.color);
                text.SetText(optionText.text);
            }
        }

        private void RefreshButtonByEnhance()
        {
            ResetButtons();

            if (!MyPlayer.Instance.core.item.TryGetItem(itemID, out var item))
            {
                return;
            }

            var resItem = item.resItem;
            if (resItem == null)
            {
                return;
            }

            enhanceButton.gameObject.SetActive(true);
            enhanceAllButton.SetActive(true);

            var text = string.Empty;
            var color = Color.white;
            var canLevelUp = false;
            var resCostItem = ResourceManager.Instance.item.GetItem(resItem.enhance.costItemID);

            var itemLevel = item.GetLevel();
            if (itemLevel >= resItem.GetMaxLevel(item.GetAwaken()) || resCostItem == null)
            {
                // 최대 레벨업
                text = "MAX";
            }
            else
            {
                enhanceButton.SetIcon(resCostItem.appearance);

                var myAmount = MyPlayer.Instance.core.item.GetAmount(resCostItem.id);
                var needCost = resItem.enhance.GetNeedCost(itemLevel, itemLevel + 1);

                text = $"{myAmount}/{needCost}";
                if (myAmount >= needCost)
                {
                    canLevelUp = true;
                }
                else
                {
                    color = GameData.COLOR.RED_FONT;
                }
            }

            enhanceButton.SetValueText(text, color);

            if (!canLevelUp)
            {
                return;
            }

            cmdEnhance.Use(true);
            cmdEnhanceAll.Use(true);
        }

        private void RefreshButtonByReroll()
        {
            ResetButtons();

            if (!MyPlayer.Instance.core.item.TryGetItem(itemID, out var item))
            {
                return;
            }

            var resItem = item.resItem;
            if (resItem == null)
            {
                return;
            }

            var resCostItem = ResourceManager.Instance.item.GetItem(resItem.reroll.costItemID);
            if (resCostItem == null)
            {
                return;
            }

            rerollButton.gameObject.SetActive(true);
            rerollButton.SetIcon(resCostItem.appearance);

            var myAmount = MyPlayer.Instance.core.item.GetAmount(resCostItem.id);
            var enable = myAmount >= resItem.reroll.value;
            rerollButton.SetValueText($"{myAmount}/{resItem.reroll.value}", enable ? Color.white : GameData.COLOR.RED_FONT);

            if (!enable)
            {
                return;
            }

            cmdReroll.Use(true);
        }

        private void RefreshButtonByAwaken()
        {
            ResetButtons();

            if (!MyPlayer.Instance.core.item.TryGetItem(itemID, out var item))
            {
                return;
            }

            var resItem = item.resItem;
            if (resItem == null)
            {
                return;
            }

            var resCostItem = ResourceManager.Instance.item.GetItem(resItem.awaken.costItemID);
            if (resCostItem == null)
            {
                return;
            }

            awakenButton.gameObject.SetActive(true);

            var text = string.Empty;
            var color = Color.white;
            var canLimitUp = false;
            if (resItem.awaken.maxAwaken > 0 && item.GetAwaken() >= resItem.awaken.maxAwaken)
            {
                text = "MAX";
            }
            else
            {
                awakenButton.SetIcon(resCostItem.appearance);

                var myAmount = MyPlayer.Instance.core.item.GetAmount(resCostItem.id);
                var needValue = resItem.awaken.GetNeedValue(item.GetAwaken());
                text = $"{myAmount}/{needValue}";

                if (myAmount >= needValue)
                {
                    canLimitUp = true;
                }
                else
                {
                    color = GameData.COLOR.RED_FONT;
                }
            }

            awakenButton.SetValueText(text, color);

            if (!canLimitUp)
            {
                return;
            }

            cmdAwaken.Use(true);
        }

        private void OnRadioEvent(GameObject go)
        {
            RefreshInfoFrame();
            RefreshUIWealth();
            RefreshOptions();
        }

        private void Cmd_Enhance()
        {
            ClickSound();

            if (!MyPlayer.Instance.core.item.TryGetItem(itemID, out var item))
            {
                return;
            }

            MyPlayer.Instance.core.inventory.EnhanceItem(item.id, 1);
        }

        private void Cmd_EnhanceAll()
        {
            ClickSound();

            if (!MyPlayer.Instance.core.item.TryGetItem(itemID, out var item))
            {
                return;
            }

            var resItem = item.resItem;
            if (resItem == null)
            {
                return;
            }

            var costAmount = MyPlayer.Instance.core.item.GetAmount(resItem.enhance.costItemID);

            var count = 0;
            var totalNeedCost = 0L;
            for (long lv = item.GetLevel(); lv < resItem.GetMaxLevel(item.GetAwaken()); ++lv)
            {
                totalNeedCost += resItem.enhance.GetNeedCost(lv, lv + 1);
                if (totalNeedCost > costAmount)
                {
                    break;
                }

                ++count;
            }

            MyPlayer.Instance.core.inventory.EnhanceItem(item.id, count);
        }

        private void Cmd_Reroll()
        {
            ClickSound();

            var resItem = ResourceManager.Instance.item.GetItem(itemID);
            if (resItem == null)
            {
                return;
            }

            if (!MyPlayer.Instance.core.item.TryGetItem(itemID, out var item))
            {
                return;
            }

            var existMax = false;
            var array = new[]
            {
                resItem.equip.targetAbilities,
                resItem.hold.targetAbilities
            };

            var itemOptions = item.GetOptions();
            foreach (var abilities in array)
            {
                foreach (var ability in abilities)
                {
                    foreach (var statItem in ability.statItems)
                    {
                        var option = statItem.GetOption(itemOptions);
                        if (option < 1f)
                        {
                            continue;
                        }

                        existMax = true;
                        break;
                    }
                }
            }

            if (existMax)
            {
                var popup = PopupExtend.Instance.ShowItemRerollMaxCheckPopup();
                popup.SetButton().SetTextKey("key_ok")
                    .SetBackground(GameData.COLOR.OK_BUTTON, GameData.COLOR.OK_BUTTON_FRAME)
                    .SetOnActive(() =>
                    {
                        MyPlayer.Instance.core.inventory.RerollItemOption(item.id);
                    });
            }
            else
            {
                MyPlayer.Instance.core.inventory.RerollItemOption(item.id);
            }
        }

        private void Cmd_Awaken()
        {
            ClickSound();

            if (!MyPlayer.Instance.core.item.TryGetItem(itemID, out var item))
            {
                return;
            }

            MyPlayer.Instance.core.inventory.AwakenItem(item.id);
        }
    }
}