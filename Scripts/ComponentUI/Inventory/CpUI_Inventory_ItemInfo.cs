using UnityEngine;

namespace UIInventory
{
    public class CpUI_Inventory_ItemInfo : UIBase
    {
        [SerializeField] CpUI_Inventory_ItemInfoFrame itemInfoFrame = null;
        [SerializeField] GameObject emptyFrame = null;

        private IUInventory uiInventory = null;

        public void Init(IUInventory uiInventory)
        {
            this.uiInventory = uiInventory;
            itemInfoFrame.Init();
        }

        public void On()
        {
            Refresh();
        }

        private void SetDefault()
        {
            emptyFrame.SetActive(true);
            itemInfoFrame.gameObject.SetActive(false);
        }

        public void Refresh()
        {
            SetDefault();
            RefreshInfoFrame();
        }

        private void RefreshInfoFrame()
        {
            var itemID = uiInventory.GetInventory().GetSelectedItemID();
            if (!uiInventory.GetInventory().TryGetItem(itemID, out var item))
            {
                return;
            }

            emptyFrame.SetActive(false);
            itemInfoFrame.gameObject.SetActive(true);
            itemInfoFrame.Refresh(item);
        }
    }
}