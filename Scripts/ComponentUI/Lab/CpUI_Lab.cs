using System.Collections.Generic;
using UnityEngine;

namespace UILab
{
    public class CpUI_Lab : UIMonoBehaviour
    {
        private static CpUI_Lab instance = null;
        public static CpUI_Lab Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_Lab>("pf_ui_lab_bottom");
                }

                return instance;
            }
        }

        [SerializeField] MyOSABasic osaScroll = null;
        [SerializeField] GameObject closeButton = null;
        [SerializeField] UIWealth wealth = null;

        private readonly List<ResourceLab> viewLabs = new List<ResourceLab>();
        private readonly MyOSABasic.OsaPool<LabOsaItem> osaPool = new MyOSABasic.OsaPool<LabOsaItem>();
        private readonly List<MyOSABasic.IOsaItem> sortOsaItems = new List<MyOSABasic.IOsaItem>();

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.CONTENTS, true);
            UsingUpdate();

            if (wealth != null)
            {
                wealth.SetWealthID(GameData.ITEM_DATA.GOLD);
            }

            if (osaScroll != null)
            {
                osaScroll.Init(OnCreateCell);
            }

            Cmd.Add(closeButton, eCmdTrigger.OnClick, Cmd_Close);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.UPDATE_ITEM_AMOUNT, Refresh)
                .Add(GameEventType.UPDATE_LAB, Refresh)
                .Add(GameEventType.UPDATE_LAB_VIRTUAL, Refresh)
                ;
        }

        private void OnCreateCell(MyOSABasicItem item)
        {
            if (item is CpUI_LabCell cell)
            {
                cell.Init();
            }
        }

        public void On()
        {
            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            osaPool.DoReset();
            sortOsaItems.Clear();

            viewLabs.Clear();
            viewLabs.AddRange(ResourceManager.Instance.lab.GetLabs());

            for (int i = 0; i < viewLabs.Count; ++i)
            {
                var osaItem = osaPool.Pop(i);
                var resLab = viewLabs[i];

                osaItem.resLab = resLab;
                sortOsaItems.Add(osaItem);
            }

            if (osaScroll != null)
            {
                osaScroll.SetItems(sortOsaItems);
            }

            RefreshUIWealth();
        }

        protected override void RefreshInternal()
        {
            osaScroll?.Refresh();
            RefreshUIWealth();
        }

        private void RefreshUIWealth()
        {
            if (wealth == null)
            {
                return;
            }

            wealth.Refresh();
        }

        public class LabOsaItem : MyOSABasic.IOsaItem
        {
            public ResourceLab resLab = null;

            public void DoReset()
            {
                resLab = null;
            }

            public bool IsEmpty()
            {
                return resLab == null;
            }

            public int SortCompare(MyOSABasic.IOsaItem other)
            {
                return 0;
            }
        }
    }
}