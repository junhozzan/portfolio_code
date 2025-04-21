using UnityEngine;
using System.Linq;

namespace UIInventory
{
    public class CpUI_Inventory : UIMonoBehaviour, IMenuItem, IUInventory
    {
        private static CpUI_Inventory instance = null;
        public static CpUI_Inventory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_Inventory>("pf_ui_inventory_bottom");
                }

                return instance;
            }
        }

        [SerializeField] CpUI_Inventory_StatInfo statInfo = null;
        [SerializeField] CpUI_Inventory_ItemInfo itemInfo = null;
        [SerializeField] CpUI_Inventory_ItemTree itemTree = null;
        [SerializeField] GameObject exitButton = null;

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.CONTENTS, true);
            //UsingBlind(false);
            UsingUpdate();

            statInfo?.Init();
            itemInfo.Init(this);
            itemTree.Init(this);

            Cmd.Add(exitButton, eCmdTrigger.OnClick, Cmd_Close);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.UPDATE_ITEM_ALL, Handle_UPDATE_ITEM_ALL)
                .Add(GameEventType.UPDATE_ITEM_NEW, Handle_UPDATE_ITEM_NEW)
                .Add(GameEventType.UPDATE_ITEM_AMOUNT, UPDATE_ITEM_AMOUNT)
                .Add(GameEventType.UPDATE_INVENTORY, Handle_UPDATE_INVENTORY)
                .Add(GameEventType.EQUIP_ITEM, Handle_EQUIP_ITEM)
                ;
        }

        public void On()
        {
            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            var finalItem = MyPlayer.Instance.core.item.GetItemIDs()
                .Select(x => ResourceManager.Instance.item.GetItem(x))
                .Where(x => x != null && x.itemType == ItemType.WEAPON)
                .OrderByDescending(x => x.id)
                .FirstOrDefault();

            (this as IUInventory).GetInventory().SetSelectItem(finalItem != null ? finalItem.id : -1);

            statInfo?.On();
            itemInfo.On();
            itemTree.On();
        }

        protected override void RefreshInternal()
        {
            statInfo?.Refresh();
            itemInfo.Refresh();
            itemTree.Refresh();
        }

        private void Handle_UPDATE_ITEM_NEW(object[] args)
        {
            itemTree.RefreshOsaItems();
            Refresh();
        }

        private void UPDATE_ITEM_AMOUNT(object[] args)
        {
            itemTree.Refresh();
        }

        private void Handle_UPDATE_ITEM_ALL(object[] args)
        {
            Refresh();
        }

        private void Handle_UPDATE_INVENTORY(object[] args)
        {
            Refresh();
        }

        private void Handle_EQUIP_ITEM(object[] args)
        {
            Refresh();
        }

        void IUInventory.RefreshExternal()
        {
            RefreshInternal();
        }

        IInventory IUInventory.GetInventory()
        {
            //return MyPlayer.Instance.core.inventory.avatar;
            return MyPlayer.Instance.core.inventory.equipment;
        }

        public override bool IsFixed()
        {
            return true;
        }

        void IMenuItem.On(int value)
        {
            On();
        }

        bool IMenuItem.Notice()
        {
            return false;
        }
    }
}
