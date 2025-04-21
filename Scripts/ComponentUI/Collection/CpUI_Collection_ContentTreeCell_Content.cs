using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace UICollection
{
    public class CpUI_Collection_ContentTreeCell_Content : CpUI_Collection_ContentTreeCell_Base
    {
        [SerializeField] UIText titleText = null;
        [SerializeField] UIText abilityText = null;
        [SerializeField] UIText dataTextOrigin = null;
        [SerializeField] GameObject complete = null;
        [SerializeField] GameObject enrollButton = null;
        [SerializeField] RectTransform dataTextGrid = null;

        private ObjectPool<UIText> dataTextPool = null;
        private StringBuilder nameBulder = new StringBuilder();

        private ResourceCollection resCollection
        {
            get
            {
                return parent.cellInfo.resCollection;
            }
        }

        public override void Init(CpUI_Collection_ContentTreeCell parent)
        {
            base.Init(parent);

            dataTextPool = ObjectPool<UIText>.Of(dataTextOrigin, dataTextGrid, onCreateInit: OnCreateTextInit);

            Cmd.Add(enrollButton, eCmdTrigger.OnClick, Cmd_Enroll);
        }

        private void OnCreateTextInit(UIText uiText)
        {
            uiText.UseCmd(Cmd_OpenItemUtil);
        }

        public override void Refresh()
        {
            base.Refresh();

            RefreshTitleText();
            RefreshAbilityText();
            RefreshComplete();
            RefreshEnrollButton();
            RefreshDataText();
        }

        private void RefreshTitleText()
        {
            if (resCollection == null)
            {
                return;
            }

            if (titleText == null)
            {
                return;
            }

            titleText.SetText(resCollection.GetName());
            titleText.SetTextColor(resCollection.nameColor);
        }

        private void RefreshAbilityText()
        {
            if (resCollection == null)
            {
                return;
            }

            if (abilityText == null)
            {
                return;
            }

            var s = string.Empty;
            foreach (var targetAbility in resCollection.targetAbilities)
            {
                if (targetAbility.statItems.Count > 0)
                {
                    var statItem = targetAbility.statItems[0];
                    s = StatItem.ToString(statItem.stat, statItem.GetValue(StatItem.Param.RiseParam.zero, 1f));
                }

                break;
            }

            abilityText.SetText(s);
        }

        private void RefreshComplete()
        {
            if (resCollection == null)
            {
                return;
            }

            if (complete == null)
            {
                return;
            }

            var collection = MyPlayer.Instance.core.collection.GetCollection(resCollection.id);

            complete.SetActive(collection.IsComplete());
        }

        private void RefreshEnrollButton()
        {
            if (resCollection == null)
            {
                return;
            }

            if (enrollButton == null)
            {
                return;
            }

            var enroll = MyPlayer.Instance.core.collection.CanEnrollData(resCollection.id);

            enrollButton.SetActive(enroll);
        }

        private void RefreshDataText()
        {
            dataTextPool.Clear();

            if (resCollection == null)
            {
                return;
            }

            var collection = MyPlayer.Instance.core.collection.GetCollection(resCollection.id);

            foreach (var data in resCollection.datas)
            {
                nameBulder.Clear();

                var resItem = ResourceManager.Instance.item.GetItem(data.itemID);
                if (resItem == null)
                {
                    continue;
                }

                Color colorTop = GameData.COLOR.COLLECTION_NONE_ITEM;
                Color colorBottom = GameData.COLOR.COLLECTION_NONE_ITEM;

                nameBulder.Append(resItem.GetName());
                if (data.itemLevel > 0)
                {
                    nameBulder.Append($" (Lv.{data.itemLevel})");
                }

                if (collection.IsComplete())
                {
                    colorTop = colorBottom = resItem.GetGradeColor();
                }
                else if (MyPlayer.Instance.core.item.TryGetItem(resItem.id, out var item))
                {
                    if (data.itemLevel > 0)
                    {
                        if (item.GetLevel() >= data.itemLevel)
                        {
                            colorTop = resItem.GetGradeColor();
                        }
                    }
                    else
                    {
                        colorTop = resItem.GetGradeColor();
                    }
                }

                var text = dataTextPool.Pop();
                text.SetCmdKey(resItem.id);
                text.SetText(nameBulder.ToString());
                //text.SetFitterHorizontal();
                text.SetGradient(colorTop, colorBottom);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(dataTextGrid);
        }

        public override float GetSize()
        {
            if (dataTextGrid != null)
            {
                return dataTextGrid.rect.size.y;
            }

            return rectTransform.rect.size.y;
        }

        private void Cmd_Enroll()
        {
            ClickSound();
            MyPlayer.Instance.core.collection.EnrollData(resCollection.id);
        }

        private void Cmd_OpenItemUtil(int resItemID, GameObject go)
        {
            if (!MyPlayer.Instance.core.item.TryGetItem(resItemID, out var item))
            {
                return;
            }

            ClickSound();
            PopupExtend.Instance.ShowItemUtil(item.id);
        }
    }
}
