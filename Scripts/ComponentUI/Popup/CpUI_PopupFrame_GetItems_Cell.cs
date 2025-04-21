using UnityEngine;

namespace UIPopup
{
    public class CpUI_PopupFrame_GetItems_Cell : MyOSAGridItem
    {
        [SerializeField] CpUI_ItemFrame icon = null;
        [SerializeField] UIText nameText = null;
        [SerializeField] UIText amountText = null;

        private ResourceItem resItem = null;
        private long amount = 0;

        public override void DoReset()
        {
            resItem = null;
            amount = 0;
            icon.SetDefault();
        }

        public override void Refresh()
        {
            RefreshIcon();
            RefreshNameText();
            RefreshAmountText();
        }

        private void RefreshIcon()
        {
            if (icon == null)
            {
                return;
            }

            icon.Set(resItem, true);
        }

        private void RefreshNameText()
        {
            if (nameText == null)
            {
                return;
            }

            nameText.SetText(resItem.GetName());
            nameText.SetTextColor(resItem.GetGradeColor());
        }

        private void RefreshAmountText()
        {
            if (amountText == null)
            {
                return;
            }

            amountText.SetText(Util.ToComma(amount));
        }

        public override void UpdateViews(MyOSAGrid.IOsaItem tOsaItem)
        {
            if (tOsaItem is CpUI_PopupFrame_GetItems.CellOSAItem osaItem)
            {
                resItem = osaItem.resItem;
                amount = osaItem.amount;

                Refresh();
            }
        }

        public override bool IsEmpty()
        {
            return resItem == null;
        }
    }
}