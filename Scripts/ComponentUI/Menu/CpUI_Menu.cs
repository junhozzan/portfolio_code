using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using GAME_DATA;


namespace UIMenu
{
    public class CpUI_Menu : UIMonoBehaviour
    {
        private static CpUI_Menu instance = null;
        public static CpUI_Menu Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_Menu>("pf_ui_menu");
                }

                return instance;
            }
        }

        [SerializeField] GameObject openMenuButton = null;
        [SerializeField] GameObject openMenuExpandMark = null;
        [SerializeField] GameObject openMenuCloseMark = null;
        [SerializeField] GameObject openMenuNotice = null;

        [SerializeField] CpUI_MenuCellGroup cellGroup = null;

        private ObjectPool<CpUI_MenuCellGroup> cellGroupPool = null;
        private List<CpUI_MenuCellGroup> showCellGroups = new List<CpUI_MenuCellGroup>();
        private ReadOnlyDictionary<GameDataMenu.MenuType, IMenuItem> menuItems = null;

        private bool isOpened = false;

        public override void Init()
        {
            base.Init();
            UsingUpdate();
            SetCanvas(UIManager.eCanvans.BASE, true);

            this.cellGroupPool = ObjectPool<CpUI_MenuCellGroup>.Of(cellGroup, cellGroup.transform.parent, onCreateInit: OnCreateCellGroup);
            this.menuItems = CreateMenuItems();

            Cmd.Add(openMenuButton, eCmdTrigger.OnClick, Cmd_OpenMenu);
        }

        private ReadOnlyDictionary<GameDataMenu.MenuType, IMenuItem> CreateMenuItems()
        {
            return new Dictionary<GameDataMenu.MenuType, IMenuItem>()
            {
                //[GameDataMenu.MenuType.STORE] = MyPlayer.Instance.core.store,
                //[GameDataMenu.MenuType.LAB] = MyPlayer.Instance.core.lab,
                //[GameDataMenu.MenuType.AVATAR] = UIInventory.CpUI_Inventory_Avatar.Instance,
                //[GameDataMenu.MenuType.ATTENDANCE] = MyPlayer.Instance.core.attendance,
                //[GameDataMenu.MenuType.COLLECTION] = MyPlayer.Instance.core.collection,
                //[GameDataMenu.MenuType.MODE] = ModeManager.Instance,
                [GameDataMenu.MenuType.MISSION] = MyPlayer.Instance.core.mission,
                [GameDataMenu.MenuType.INVENTORY] = UIInventory.CpUI_Inventory.Instance,
                [GameDataMenu.MenuType.RANKING] = UIRank.CpUI_Rank.Instance,
                [GameDataMenu.MenuType.USERINFO] = UIUserInfo.CpUI_UserInfoView.Instance,
#if UNITY_EDITOR
                [GameDataMenu.MenuType.CHEAT] = UICheat.CpUI_Cheat.Instance,
#endif

            }
            .ReadOnly();
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.JOIN_FIELD, Refresh)
                .Add(GameEventType.UPDATE_MISSION, Refresh)
                .Add(GameEventType.UPDATE_ATTENDANCE, Refresh)
                .Add(GameEventType.UPDATE_COLLECTION, Refresh)
                ;
        }

        protected override void RefreshInternal()
        {
            RefreshNotice();
        }

        private void RefreshNotice()
        {
            foreach (var cellGroup in showCellGroups)
            {
                cellGroup.RefreshNotice();
            }

            var exist = false;
            foreach (var menuItem in menuItems.Values)
            {
                if (menuItem == null || !menuItem.Notice())
                {
                    continue;
                }

                exist = true;
                break;
            }

            openMenuNotice.SetActive(!isOpened && exist);
        }

        private void OnCreateCellGroup(CpUI_MenuCellGroup cp)
        {
            cp.Init(this);
        }

        public void On()
        {
            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            ClearAll();
            RefreshOnOffState();
            RefreshNotice();
        }

        private void ClearAll()
        {
            isOpened = false;
            cellGroupPool.Clear();
            showCellGroups.Clear();
        }

        private void CallEvent(GameDataMenu.MenuType menuType, int value)
        {
            var menuItem = GetMenuItem(menuType);
            if (menuItem == null)
            {
                return;
            }

            menuItem.On(value);
        }

        public IMenuItem GetMenuItem(GameDataMenu.MenuType type)
        {
            return menuItems.TryGetValue(type, out var v) ? v : null;
        }

        private void ShowGroup(GameDataMenu.MenuGroup menuGroup)
        {
            for (int i = 0; i < showCellGroups.Count; ++i)
            {
                if (i < menuGroup.index)
                {
                    continue;
                }

                var cellGroup = showCellGroups[i];
                cellGroup.gameObject.SetActive(false);
            }

            var removeCount = Mathf.Max(showCellGroups.Count - menuGroup.index, 0);
            showCellGroups.RemoveRange(menuGroup.index, removeCount);

            if (menuGroup.items == null || menuGroup.items.Count == 0)
            {
                return;
            }

            var group = cellGroupPool.Pop();
            group.SetCells(menuGroup.startLocation, menuGroup.items);

            showCellGroups.Add(group);
        }

        private void RefreshOnOffState()
        {
            openMenuExpandMark.SetActive(!isOpened);
            openMenuCloseMark.SetActive(isOpened);
        }

        public void OnClickMenu(GameDataMenu.MenuItem menuItem)
        {
            ShowGroup(menuItem.subGroup);
            CallEvent(menuItem.menuType, menuItem.value);
        }

        public override bool CanClose()
        {
            if (showCellGroups.Count > 1)
            {
                var last = showCellGroups[showCellGroups.Count - 1];
                last.gameObject.SetActive(false);
                showCellGroups.Remove(last);
                return false;
            }

            isOpened = false;
            ClearAll();
            RefreshOnOffState();
            RefreshNotice();

            return false;
        }

        public override bool IsFixed()
        {
            return !isOpened;
        }

        private void Cmd_OpenMenu()
        {
            isOpened = !isOpened;

            if (isOpened)
            {
                ShowGroup(GameData.MENU.GROUP);
            }
            else
            {
                ClearAll();
            }

            ClickSound();
            RefreshOnOffState();
            RefreshNotice();
        }
    }
}
