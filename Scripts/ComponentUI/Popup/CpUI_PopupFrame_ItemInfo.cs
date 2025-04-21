using System;
using UnityEngine;

namespace UIPopup
{
    public class CpUI_PopupFrame_ItemInfo : CpUI_PopupFrame_Base
    {
        [SerializeField] UIInventory.CpUI_Inventory_ItemInfoFrame itemInfoFrame = null;

        public override void Init(CpUI_Popup parent, Action<CpUI_PopupFrame_Base> onCloseAt)
        {
            base.Init(parent, onCloseAt);

            itemInfoFrame.Init();
        }

        public CpUI_PopupFrame_ItemInfo On(int itemID)
        {
            if (MyPlayer.Instance.core.item.TryGetItem(itemID, out var item))
            {
                itemInfoFrame.Refresh(item);
            }

            return this;
        }
    }
}