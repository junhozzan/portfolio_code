using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GAME_DATA;
using UnityEngine.UI;

namespace UIMenu
{
    public class CpUI_MenuCellGroup : UIBase
    {
        [SerializeField] GridLayoutGroup group = null;
        [SerializeField] CpUI_MenuCell cell = null;

        private ObjectPool<CpUI_MenuCell> cellPool = null;
        private CpUI_Menu uiMenu = null;

        public void Init(CpUI_Menu uiMenu)
        {
            this.uiMenu = uiMenu;
            this.cellPool = ObjectPool<CpUI_MenuCell>.Of(cell, group.gameObject, onCreateInit: OnCreateCell);
        }

        private void OnCreateCell(CpUI_MenuCell cp)
        {
            cp.Init(uiMenu);
        }

        public void SetCells(int startLocation, ICollection<GameDataMenu.MenuItem> items)
        {
            var cellSizeY = (int) group.cellSize.y;
            group.padding.top = cellSizeY * startLocation;

            cellPool.Clear();
            foreach (var item in items)
            {
#if !UNITY_EDITOR
                if (item.menuType == GameDataMenu.MenuType.CHEAT)
                {
                    continue;
                }
#endif

                var cell = cellPool.Pop();
                cell.SetMenuItem(item);
            }
        }

        public void RefreshNotice()
        {
            var cells = cellPool.GetPool();
            foreach (var cell in cells)
            {
                if (!cell.gameObject.activeSelf)
                {
                    continue;
                }

                cell.RefreshNotice();
            }
        }
    }
}