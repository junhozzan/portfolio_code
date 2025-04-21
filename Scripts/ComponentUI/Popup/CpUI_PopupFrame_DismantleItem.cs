using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace UIPopup
{
    public class CpUI_PopupFrame_DismantleItem : CpUI_PopupFrame_Base
    {
        [SerializeField] CpUI_ItemFrame beforeFrame = null;
        [SerializeField] UIText beforeNameText = null;
        [SerializeField] UIText beforeAmountText = null;

        [SerializeField] CpUI_ItemFrame afterFrame = null;
        [SerializeField] UIText afterNameText = null;
        [SerializeField] UIText afterAmountText = null;
        [SerializeField] UIAmountSlider amountSlider = null;

        [SerializeField] GameObject plusButton = null;
        [SerializeField] GameObject minusButton = null;

        private int itemID = 0;
        private Cmd cmdPlus = null;
        private Cmd cmdMinus = null;

        public override void Init(CpUI_Popup parent, Action<CpUI_PopupFrame_Base> onCloseAt)
        {
            base.Init(parent, onCloseAt);
            amountSlider.SetExternalValueChange(AmountValueChange);

            cmdPlus = Cmd.Add(plusButton, eCmdTrigger.OnClick, Cmd_Plus);
            cmdMinus = Cmd.Add(minusButton, eCmdTrigger.OnClick, Cmd_Minus);
        }

        public override void DoReset()
        {
            base.DoReset();
            itemID = -1;
            amountSlider.DoReset();
        }

        public CpUI_PopupFrame_DismantleItem On(int itemID)
        {
            this.itemID = itemID;

            SetMaxAmount();
            RefreshItemInfos();
            RefreshAmountInfos();
            RefreshButtons();

            return this;
        }

        private void RefreshButtons()
        {
            var amount = GetAmount();
            cmdPlus.Use(amount < amountSlider.GetMaxAmount());
            cmdMinus.Use(amount > 1);
        }

        private void SetMaxAmount()
        {
            amountSlider.SetMax(MyPlayer.Instance.core.item.GetAmount(itemID));
        }

        private void AmountValueChange(long amount)
        {
            RefreshAmountInfos();
            RefreshButtons();
        }

        private void RefreshItemInfos()
        {
            var resItem = ResourceManager.Instance.item.GetItem(itemID);
            if (resItem == null)
            {
                return;
            }

            var resGetItem = ResourceManager.Instance.item.GetItem(resItem.dismantle.getItemID);
            if (resGetItem == null)
            {
                return;
            }

            beforeFrame.Set(resItem, false);
            beforeNameText.SetText(resItem.GetName());

            afterFrame.Set(resGetItem, false);
            afterNameText.SetText(resGetItem.GetName());
        }

        private void RefreshAmountInfos()
        {
            var resItem = ResourceManager.Instance.item.GetItem(itemID);
            if (resItem == null)
            {
                return;
            }

            var resGetItem = ResourceManager.Instance.item.GetItem(resItem.dismantle.getItemID);
            if (resGetItem == null)
            {
                return;
            }

            var amount = GetAmount();
            beforeAmountText.SetText($"{"key_dismantle_amount".L()}:{Util.ToComma(amount)}");
            afterAmountText.SetText($"{"key_get_amount".L()}:{Util.ToComma(amount * resItem.dismantle.value)}");
        }

        public long GetAmount()
        {
            return amountSlider.GetAmount();
        }

        private void Cmd_Plus()
        {
            amountSlider.AddAmount(1);
            RefreshButtons();
        }

        private void Cmd_Minus()
        {
            amountSlider.AddAmount(-1);
            RefreshButtons();
        }
    }
}