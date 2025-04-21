using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UIInventory;

namespace UIPopup
{
    public class CpUI_PopupFrame_Dictionary : CpUI_PopupFrame_Base, IUInventory
    {
        [SerializeField] CpUI_Inventory_ItemInfo itemInfo = null;
        [SerializeField] CpUI_Inventory_ItemTree itemTree = null;

        public override void Init(CpUI_Popup parent, Action<CpUI_PopupFrame_Base> onCloseAt)
        {
            base.Init(parent, onCloseAt);

            itemInfo.Init(this);
            itemTree.Init(this);
        }

        public override void On()
        {
            base.On();

            (this as IUInventory).GetInventory().SetSelectItem(-1);

            itemInfo.On();
            itemTree.On();
        }

        IInventory IUInventory.GetInventory()
        {
            return MyPlayer.Instance.core.inventory.dictionary;
        }

        void IUInventory.RefreshExternal()
        {
            itemInfo.Refresh();
            itemTree.Refresh();
        }
    }
}