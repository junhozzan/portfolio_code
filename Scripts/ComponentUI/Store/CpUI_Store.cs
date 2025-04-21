using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using MyPlayerComponent;

namespace UIStore
{
    public class CpUI_Store : UIMonoBehaviour
    {
        private static CpUI_Store instance = null;
        public static CpUI_Store Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_Store>("pf_ui_store");
                }

                return instance;
            }
        }

        [SerializeField] MyOSAGrid osaScroll = null;
        [SerializeField] CpUI_Store_Group_Cell groupCellBase = null;
        [SerializeField] GameObject exitButton = null;
        [SerializeField] UIWealth uiWealthOrigin = null;
        [SerializeField] GameObject infoButton = null;

        private List<CpUI_Store_Group_Cell> groupCells = new List<CpUI_Store_Group_Cell>();
        private MyOSAGrid.OsaPool<ItemCellOSAItem> osaPool = new MyOSAGrid.OsaPool<ItemCellOSAItem>();
        private List<MyOSAGrid.IOsaItem> sortOsaItems = new List<MyOSAGrid.IOsaItem>();

        private ResourceStoreGroup resGroup = null;
        private ObjectPool<UIWealth> uiWealthPool = null;
        private List<int> wealthCostItemIDs = new List<int>();

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.CONTENTS, true);
            UsingBlind(false, true);
            UsingUpdate();

            uiWealthPool = ObjectPool<UIWealth>.Of(uiWealthOrigin, uiWealthOrigin.transform.parent);

            osaScroll.Init(OnCreatListItem);
            InitCategoryCells();

            Cmd.Add(exitButton, eCmdTrigger.OnClick, Cmd_Close);
            Cmd.Add(infoButton, eCmdTrigger.OnClick, Cmd_OpenDictionary);
        }

        private void OnCreatListItem(MyOSAGridItem item)
        {
            if (item is CpUI_Store_Item_Cell cp)
            {
                cp.Init(OnClickListCell);
            }
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.PURCHASE_STORE_ITEM, Refresh)
                ;
        }

        private void InitCategoryCells()
        {
            groupCells.Clear();
            groupCellBase.gameObject.SetActive(false);

            foreach (var resGroup in ResourceManager.Instance.store.GetStoreGroups())
            {
                var cell = Instantiate(groupCellBase, groupCellBase.transform.parent);
                cell.gameObject.SetActive(true);
                cell.Init(resGroup, OnClickCategoryCell);

                groupCells.Add(cell);
            }
        }

        public void On()
        {
            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            resGroup = ResourceManager.Instance.store.GetStoreGroups().FirstOrDefault();
            if (resGroup == null)
            {
                return;
            }

            RefreshCategoryCells();
            SetListCells();
            RefreshWealth();
        }

        private void RefreshCategoryCells()
        {
            foreach (var cell in groupCells)
            {
                cell.SetOnState(resGroup);
            }
        }

        protected override void RefreshInternal()
        {
            RefreshItemCell();
            RefreshWealth();
        }

        private void SetListCells()
        {
            osaPool.DoReset();
            sortOsaItems.Clear();

            var resGroup = this.resGroup;
            if (resGroup == null)
            {
                return;
            }

            for (int i = 0; i < resGroup.storeIDs.Count; ++i)
            {
                var resItem = ResourceManager.Instance.store.GetStoreItem(resGroup.storeIDs[i]);
                if (resItem == null)
                {
                    continue;
                }

                var osaItem = osaPool.Pop(i);
                osaItem.resStoreItem = resItem;

                sortOsaItems.Add(osaItem);
            }

            osaScroll.SetItems(sortOsaItems);
            osaScroll.Scroll(0);
        }

        private void RefreshWealth()
        {
            uiWealthPool.Clear();
            wealthCostItemIDs.Clear();
            foreach (var storeID in resGroup.storeIDs)
            {
                var resStoreItem = ResourceManager.Instance.store.GetStoreItem(storeID);
                if (resStoreItem == null)
                {
                    continue;
                }

                if (wealthCostItemIDs.Contains(resStoreItem.costItemID))
                {
                    continue;
                }

                wealthCostItemIDs.Add(resStoreItem.costItemID);
            }

            wealthCostItemIDs.Sort((a, b) => a.CompareTo(b));

            foreach (var costItemID in wealthCostItemIDs)
            {
                uiWealthPool.Pop().SetWealthID(costItemID).Refresh();
            }

            wealthCostItemIDs.Clear();
        }

        private void RefreshItemCell()
        {
            osaScroll.Refresh();
        }

        private void OnClickListCell(ResourceStoreItem resStoreItem)
        {
            void sendPurchaseStoreITem()
            {
                VirtualServer.Send(Packet.PURCHASE_STORE_ITEM,
                    (arg) =>
                    {
                        if (!VirtualServer.TryGet(arg, out PURCHASE_STORE_ITEM tArg)) 
                        {
                            return;
                        }

                        GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                        GameEvent.Instance.AddEvent(GameEventType.PURCHASE_STORE_ITEM, tArg.tstore);
                        GameEvent.Instance.AddEvent(GameEventType.SHOW_GET_INFO, tArg.getInfos);
                    },
                    resStoreItem.id);
            }

            var resCostItem = ResourceManager.Instance.item.GetItem(resStoreItem.costItemID);
            if (resCostItem == null)
            {
                return;
            }

            var popup = PopupExtend.Instance.ShowByPack(resStoreItem.packID, resStoreItem.IsAdReward());
            popup.SetButton().SetTextKey("key_cancel");

            var canPurchase = !resStoreItem.IsAdReward() && resStoreItem.costValue > MyPlayer.Instance.core.item.GetAmount(resStoreItem.costItemID);
            var okButton = popup.SetButton()
                .SetBackground(GameData.COLOR.OK_BUTTON, GameData.COLOR.OK_BUTTON_FRAME);

            if (resCostItem.appearance.use)
            {
                okButton.SetIcon(resCostItem.appearance.atlas, resCostItem.appearance.sprite);
            }

            if (resStoreItem.IsAdReward())
            {
                var resAd = ResourceManager.Instance.ad.GetAd(resStoreItem.adID);
                if (resAd != null)
                {
                    okButton.SetOnUpdate(() =>
                    {
                        var ad = MyPlayer.Instance.core.ad.GetAd(resAd.id);
                        var current = Main.Instance.time.nowToEpochSecond();
                        var s = ad.coolTime > current ? Util.SecToTimer2(ad.coolTime - current) : "key_show_ad".L();
                        okButton.SetText(s, GameData.COLOR.POPUP_BUTTON_FONT);
                    });
                }

                okButton.SetOnActive(() =>
                {
                    var ad = MyPlayer.Instance.core.ad.GetAd(resAd.id);
                    var current = Main.Instance.time.nowToEpochSecond();
                    if (ad.coolTime > current)
                    {
                        Main.Instance.ShowFloatingMessage("key_ex_delay_ad".L());
                        return;
                    }

                    AdManager.Instance.ShowRewardedAd(AdManager.AdType.AD_MOB,
                        resAd,
                        (result) =>
                        {
                            if (result != AdManager.Result.Complete)
                            {
                                return;
                            }

                            VirtualServer.Send(Packet.SHOW_AD,
                                (arg) =>
                                {
                                    if (!VirtualServer.TryGet(arg, out SHOW_AD tArg))
                                    {
                                        return;
                                    }

                                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_AD, tArg.tadvertisement);
                                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                                    GameEvent.Instance.AddEvent(GameEventType.SHOW_GET_INFO, tArg.getInfos);
                                },
                                resStoreItem.adID);

                            sendPurchaseStoreITem();
                        });
                });
            }
            else
            {
                if (resStoreItem.costValue > 0)
                {
                    okButton.SetText(resStoreItem.costValue.ToString("#,##0"), canPurchase ? GameData.COLOR.RED_FONT : GameData.COLOR.POPUP_BUTTON_FONT);
                }
                else
                {
                    okButton.SetText("key_free".L(), GameData.COLOR.POPUP_BUTTON_FONT);
                }

                okButton.SetOnActive(() =>
                {
                    sendPurchaseStoreITem();
                });
            }
        }

        private void OnClickCategoryCell(ResourceStoreGroup resGroup)
        {
            this.resGroup = resGroup;

            RefreshCategoryCells();
            SetListCells();
            RefreshWealth();
        }

        private void Cmd_OpenDictionary()
        {
            PopupExtend.Instance.ShowDictionary();
        }

        public class ItemCellOSAItem : MyOSAGrid.IOsaItem
        {
            public ResourceStoreItem resStoreItem = null;

            public void DoReset()
            {
                resStoreItem = null;
            }

            public bool IsEmpty()
            {
                return resStoreItem == null;
            }

            public int SortCompare(MyOSAGrid.IOsaItem other)
            {
                return 0;
            }
        }
    }
}