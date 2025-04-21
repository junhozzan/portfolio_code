using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GAME_DATA;

namespace UIMenu
{
    public class CpUI_MenuCell : UIBase
    {
        [SerializeField] UIText nameText = null;
        [SerializeField] UIImage icon = null;
        [SerializeField] GameObject button = null;
        [SerializeField] GameObject notice = null;
        [SerializeField] GameObject arrow = null;

        private CpUI_Menu uiMenu = null;
        private GameDataMenu.MenuItem menuItem = null;

        public void Init(CpUI_Menu uiMenu)
        {
            this.uiMenu = uiMenu;

            Cmd.Add(button, eCmdTrigger.OnClick, Cmd_Click);
        }

        public void SetMenuItem(GameDataMenu.MenuItem menuItem)
        {
            this.menuItem = menuItem;
            RefreshName();
            RefreshIcon();
            RefreshNotice();
            RefresArrow();
        }

        public void RefreshNotice()
        {
            var imenuItem = CpUI_Menu.Instance.GetMenuItem(menuItem.menuType);
            notice.SetActive(imenuItem != null ? imenuItem.Notice() : false);
        }

        private void RefreshName()
        {
            nameText.SetTextKey(menuItem.title);
            //nameText.SetFitterHorizontal(2f);
        }

        private void RefreshIcon()
        {
            if (menuItem == null || string.IsNullOrEmpty(menuItem.sprite))
            {
                icon.gameObject.SetActive(false);
                return;
            }

            icon.SetSprite(Atlas.UI_BASE, menuItem.sprite);
            icon.gameObject.SetActive(true);
        }

        private void RefresArrow()
        {
            arrow.SetActive(menuItem.isShowArrow);
        }

        private void Cmd_Click()
        {
            ClickSound();

            uiMenu.OnClickMenu(menuItem);
        }
    }
}