using System.Collections.Generic;
using System;
using UnityEngine;

namespace UIPopup
{
    public class CpUI_PopupFrame_GetItems : CpUI_PopupFrame_Base
    {
        [SerializeField] MyOSAGrid osaScroll = null;

        private readonly MyOSAGrid.OsaPool<CellOSAItem> osaPool = new MyOSAGrid.OsaPool<CellOSAItem>();
        private readonly List<MyOSAGrid.IOsaItem> sortOsaItems = new List<MyOSAGrid.IOsaItem>();
        private readonly List<GetInfo> sortGetInfos = new List<GetInfo>();

        public override void Init(CpUI_Popup parent, Action<CpUI_PopupFrame_Base> onCloseAt)
        {
            base.Init(parent, onCloseAt);

            if (osaScroll != null)
            {
                osaScroll.Init(null);
            }
        }

        public void SetContents(IList<GetInfo> getInfos)
        {
            sortGetInfos.Clear();
            sortGetInfos.AddRange(getInfos);
            sortGetInfos.Sort((a, b) =>
            {
                var resItemA = ResourceManager.Instance.item.GetItem(a.itemID);
                var resItemB = ResourceManager.Instance.item.GetItem(b.itemID);

                if (resItemA.IsAvatar() && !resItemB.IsAvatar())
                {
                    return -1;
                }
                else if (!resItemA.IsAvatar() && resItemB.IsAvatar())
                {
                    return 1;
                }

                return resItemB.grade.CompareTo(resItemA.grade);
            });


            osaPool.DoReset();
            sortOsaItems.Clear();

            for (int i = 0; i < sortGetInfos.Count; ++i)
            {
                var getInfo = sortGetInfos[i];
                var osaItem = osaPool.Pop(i);
                osaItem.resItem = ResourceManager.Instance.item.GetItem(getInfo.itemID);
                osaItem.amount = getInfo.amount;

                sortOsaItems.Add(osaItem);
            }

            osaScroll?.SetItems(sortOsaItems);
            osaScroll?.ScrollTo(0);
        }

        public class CellOSAItem : MyOSAGrid.IOsaItem
        {
            public ResourceItem resItem = null;
            public long amount = 0;

            public void DoReset()
            {
                resItem = null;
                amount = 0;
            }

            public bool IsEmpty()
            {
                return resItem == null;
            }

            public int SortCompare(MyOSAGrid.IOsaItem other)
            {
                return 0;
            }
        }
    }
}