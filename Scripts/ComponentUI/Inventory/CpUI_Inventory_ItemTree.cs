using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UIInventory
{
    public class CpUI_Inventory_ItemTree : UIBase
    {
        [SerializeField] CpUI_Inventory_ItemFilter filter = null;
        [SerializeField] MyOSABasic osaScroll = null;
        [SerializeField] UIText equipPointText = null;
        [SerializeField] GameObject allEnhanceButton = null;

        private readonly MyOSABasic.OsaPool<ItemOsaItem> osaPool = new MyOSABasic.OsaPool<ItemOsaItem>();
        private readonly List<MyOSABasic.IOsaItem> sortOsaItems = new List<MyOSABasic.IOsaItem>();
        private List<ResourceItem> cachedSortDatas = new List<ResourceItem> ();
        private int cachedOwnItemCount = 0;
        
        private IUInventory uiInventory = null;
        
        public void Init(IUInventory uiInventory)
        {
            this.uiInventory = uiInventory;

            filter.Init(this, uiInventory);
            osaScroll.Init(OnCreateCell);

            Cmd.Add(allEnhanceButton, eCmdTrigger.OnClick, Cmd_AllEnhance);
        }

        private void OnCreateCell(MyOSABasicItem item)
        {
            if (item is CpUI_Inventory_ItemTreeCell cell)
            {
                cell.Init(uiInventory);
            }
        }

        public void On()
        {
            filter.On();
            //RefreshPointText();
            RefreshOsaItems();
        }

        public void Refresh()
        {
            RefreshPointText();
            osaScroll.Refresh();
        }

        private void RefreshPointText()
        {
            equipPointText.SetText($"{"key_equip_item".L()}({MyPlayer.Instance.core.inventory.GetEquipPoint()}/{MyPlayer.Instance.core.inventory.GetMaxEquipPoint()})");
        }

        public void RefreshOsaItems()
        {
            var playerItemIDs = MyPlayer.Instance.core.item.GetItemIDs();
            var isAdded = cachedOwnItemCount != playerItemIDs.Count;
            cachedOwnItemCount = playerItemIDs.Count;

            if (isAdded)
            {
                var viewItems = uiInventory.GetInventory().GetViewItems();
                cachedSortDatas = viewItems
                    .OrderByDescending(x => playerItemIDs.Contains(x.id))
                    .ThenByDescending(x => x.grade)
                    //.ThenByDescending(x => x.setType)
                    .ThenByDescending(x => x.id)
                    .ToList();
            }

            osaPool.DoReset();
            sortOsaItems.Clear();

            var filterDatas = cachedSortDatas
                .Where(uiInventory.GetInventory().IsFiltering)
                .ToArray();

            for (int i = 0; i < filterDatas.Length; ++i)
            {
                var osaItem = osaPool.Pop(i);
                osaItem.resItem = filterDatas[i];

                sortOsaItems.Add(osaItem);
            }

            osaScroll.SetItems(sortOsaItems);
        }

        private void Cmd_AllEnhance()
        {
            ClickSound();
            MyPlayer.Instance.core.inventory.EnhanceItemAll();
        }

        public class ItemOsaItem : MyOSABasic.IOsaItem
        {
            public ResourceItem resItem = null;

            public void DoReset()
            {
                resItem = null;
            }

            public bool IsEmpty()
            {
                return resItem == null;
            }

            public int SortCompare(MyOSABasic.IOsaItem other)
            {
                return 0;
            }
        }
    }
}