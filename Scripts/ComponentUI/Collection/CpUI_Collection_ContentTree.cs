using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UICollection
{
    public class CpUI_Collection_ContentTree : UIBase
    {
        [SerializeField] MyOSAHierarchy osaScroll = null;

        private RootOsaItem rootOsaItem = null;
        private Dictionary<long, GroupOsaItem> recycleGroupItems = new Dictionary<long, GroupOsaItem>();

        public void Init()
        {
            osaScroll.Init(OnCreateItem);
        }

        private void OnCreateItem(MyOSAHierarchyItem item)
        {
            if (!(item is CpUI_Collection_ContentTreeCell cell))
            {
                return;
            }

            cell.Init();
        }

        public void On()
        {
            RefreshRootOsaItem();
        }

        public void Refresh()
        {
            RefreshCells();
        }

        private void RefreshRootOsaItem()
        {
            var groups = ResourceManager.Instance.collection.GetCollectionGroups()
                .Select(x =>
                {
                    if (!recycleGroupItems.TryGetValue(x.id, out var v))
                    {
                        recycleGroupItems.Add(x.id, v = GroupOsaItem.Of(x));
                    }

                    return v;
                });

            rootOsaItem = RootOsaItem.Of(groups);
            rootOsaItem.CollapseAll();

            SortItems(false, 0);
        }

        private void RefreshCells()
        {
            osaScroll.Refresh();
        }

        private void SortItems(bool isCollapseAll, int viewStart)
        {
            if (isCollapseAll)
            {
                rootOsaItem.CollapseAll();
            }

            var isCatch = false;
            var targetOrder = 0;

            void addOrder()
            {
                if (isCatch)
                {
                    return;
                }

                ++targetOrder;
            }

            foreach (GroupOsaItem groupOsaItem in rootOsaItem.children)
            {
                addOrder();

                if (!groupOsaItem.isExpand)
                {
                    continue;
                }

                foreach (ContentOsaItem contentOsaItem in groupOsaItem.children)
                {
                    addOrder();

                    if (!contentOsaItem.isExpand)
                    {
                        continue;
                    }
                }
            }

            if (osaScroll != null)
            {
                osaScroll.SetList(rootOsaItem);
                osaScroll.Scroll(targetOrder, viewStart);
            }
        }
    }
}
