using System.Collections.Generic;
using UnityEngine;

namespace UIInventory
{
    public class CpUI_Inventory_ItemFilter : UIBase
    {
        [SerializeField] GameObject grid = null;
        [SerializeField] UIButton originButton = null;

        private readonly List<ItemFilterType> sortTypes = new List<ItemFilterType>();
        private CpUI_Inventory_ItemTree itemTree = null;
        private IUInventory uiInventory = null;
        private ObjectPool<UIButton> buttonPool = null;

        public void Init(CpUI_Inventory_ItemTree itemTree, IUInventory uiInventory)
        {
            this.itemTree = itemTree;
            this.uiInventory = uiInventory;

            this.buttonPool = ObjectPool<UIButton>.Of(originButton, grid, onCreateInit: OnCreateInit);
        }

        private void OnCreateInit(UIButton button)
        {
            button.Init(Cmd_Click);
        }

        public void On()
        {
            Close();
        }

        private void Show()
        {
            var selectedItemFilterType = uiInventory.GetInventory().GetFilterType();
            sortTypes.Clear();
            sortTypes.AddRange(uiInventory.GetInventory().GetSortTypes());
            sortTypes.Sort((a, b) =>
            {
                if (a == selectedItemFilterType && b != selectedItemFilterType)
                {
                    return -1;
                }
                else if (a != selectedItemFilterType && b == selectedItemFilterType)
                {
                    return 1;
                }

                return 0;
            });

            buttonPool.Clear();
            foreach (var type in sortTypes)
            {
                var button = buttonPool.Pop();
                button.transform.SetAsLastSibling();
                button.SetKey(Util.EnumToInt(type));
                button.SetText($"key_item_filter_{type.ToString().ToLower()}".L());
                button.SetTextColor(selectedItemFilterType == type ? GameData.COLOR.SELECT_FONT : Color.white);
            }
        }

        private void Close()
        {
            var selectedItemFilterType = uiInventory.GetInventory().GetFilterType();

            buttonPool.Clear();
            var button = buttonPool.Pop();
            button.SetKey(Util.EnumToInt(selectedItemFilterType));
            button.SetText($"key_item_filter_{selectedItemFilterType.ToString().ToLower()}".L());
            button.SetTextColor(GameData.COLOR.SELECT_FONT);
        }

        private bool IsShow()
        {
            var count = 0;
            foreach (var button in buttonPool.GetPool())
            {
                if (!button.gameObject.activeSelf)
                {
                    continue;
                }

                ++count;
                if (count > 1)
                {
                    return true;
                }
            }

            return false;
        }

        private void Cmd_Click(int i)
        {
            if (IsShow())
            {
                uiInventory.GetInventory().SetFilterType(Util.IntToEnum<ItemFilterType>(i));
                Close();

                itemTree.RefreshOsaItems();
            }
            else
            {
                Show();
            }
        }

    }
}