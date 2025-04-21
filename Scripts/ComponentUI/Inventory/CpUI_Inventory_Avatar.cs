using System.Linq;
using UnityEngine;

namespace UIInventory
{
    public class CpUI_Inventory_Avatar : UIMonoBehaviour, IMenuItem, IUInventory
    {
        private static CpUI_Inventory_Avatar instance = null;
        public static CpUI_Inventory_Avatar Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_Inventory_Avatar>("pf_ui_inventory_avatar");
                }

                return instance;
            }
        }

        [SerializeField] CpUI_Inventory_ItemInfo itemInfo = null;
        [SerializeField] CpUI_Inventory_ItemTree itemTree = null;
        [SerializeField] GameObject exitButton = null;

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.CONTENTS, true);
            UsingBlind(false, false);
            UsingUpdate();

            itemInfo.Init(this);
            itemTree.Init(this);

            Cmd.Add(exitButton, eCmdTrigger.OnClick, Cmd_Close);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.UPDATE_INVENTORY, Refresh)
                ;
        }

        public void On()
        {
            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            (this as IUInventory).GetInventory().SetSelectItem(-1);

            itemInfo.On();
            itemTree.On();
        }

        protected override void RefreshInternal()
        {
            itemInfo.Refresh();
            itemTree.Refresh();
        }

        IInventory IUInventory.GetInventory()
        {
            return MyPlayer.Instance.core.inventory.avatar;
        }

        void IUInventory.RefreshExternal()
        {
            RefreshInternal();
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