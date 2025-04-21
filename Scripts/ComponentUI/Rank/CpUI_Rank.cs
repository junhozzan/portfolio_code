using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using MyPlayerComponent;
using System;

namespace UIRank
{
    public class CpUI_Rank : UIMonoBehaviour, IMenuItem
    {
        private static CpUI_Rank m_instance = null;
        public static CpUI_Rank Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = UIManager.Instance.Find<CpUI_Rank>("pf_ui_rank_v");
                }

                return m_instance;
            }
        }

        [SerializeField] MyOSABasic osaScroll = null;
        [SerializeField] CpUI_Rank_MyRank myRankCell = null;
        [SerializeField] UIText stateText = null;
        [SerializeField] UIRadio radio = null;
        [SerializeField] GameObject tabMode1 = null;
        //[SerializeField] GameObject tabMode2 = null;
        //[SerializeField] GameObject tabMode3 = null;
        [SerializeField] GameObject closeButton = null;
        [SerializeField] GameObject refreshButton = null;
        [SerializeField] UIImage coolTime = null;

        private readonly MyOSABasic.OsaPool<RankOsaItem> osaPool = new MyOSABasic.OsaPool<RankOsaItem>();
        private readonly List<MyOSABasic.IOsaItem> sortOsaItems = new List<MyOSABasic.IOsaItem>();

        private ReadOnlyDictionary<GameObject, int> modeIDByTab = null;

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.CONTENTS, true);
            UsingBlind(false);
            UsingUpdate();

            radio.Init(OnRadioTab);
            osaScroll.Init(OnCreateCell);

            modeIDByTab = new Dictionary<GameObject, int>()
            {
                [tabMode1] = GameData.MODE_DATA.MODE_1_ID,
                //[tabMode2] = GameData.MODE_DATA.MODE_2_ID,
                //[tabMode3] = GameData.MODE_DATA.MODE_3_ID,
            }.ReadOnly();

            Cmd.Add(closeButton, eCmdTrigger.OnClick, Cmd_Close);
            Cmd.Add(refreshButton, eCmdTrigger.OnClick, Cmd_Refresh);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.RESPONSE_RANK, Refresh)
                ;
        }

        private void OnCreateCell(MyOSABasicItem item)
        {
            if (item is CpUI_Rank_Cell cell)
            {

            }
        }

        private void OnRadioTab(GameObject go)
        {
            ClickSound();
            RequestOrRefresh();
        }

        private void On(int modeID)
        {
            GameObject tab;
            if (modeID == GameData.MODE_DATA.MODE_1_ID)
            {
                tab = tabMode1;
            }
            //else if (modeID == GameData.MODE_DATA.MODE_2_ID)
            //{
            //    tab = tabMode2;
            //}
            //else if (modeID == GameData.MODE_DATA.MODE_3_ID)
            //{
            //    tab = tabMode3;
            //}
            else
            {
                return;
            }

            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            radio.Choice(tab);
            RequestOrRefresh();
        }

        public override void UpdateDt(float unDt, DateTime now)
        {
            base.UpdateDt(unDt, now);

            UpdateAdCoolTime();
        }

        private void UpdateAdCoolTime()
        {
            var resAd = ResourceManager.Instance.ad.GetAd(GetModeAdID());
            if (resAd == null)
            {
                return;
            }
            
            var ad = MyPlayer.Instance.core.ad.GetAd(resAd.id);
            var current = Main.Instance.time.nowToEpochSecond();
            if (ad.coolTime > current)
            {
                coolTime.gameObject.SetActive(true);
                coolTime.SetFillAmount(Mathf.Clamp01((ad.coolTime - current) / (float)resAd.coolTime));
            }
            else
            {
                coolTime.gameObject.SetActive(false);
            }
        }

        protected override void RefreshInternal()
        {
            RefreshRank();
        }

        private void RequestOrRefresh()
        {
            if (!modeIDByTab.TryGetValue(radio.current, out var modeID))
            {
                return;
            }

            var rankInfo = MyPlayer.Instance.core.mode.rank.GetRankInfo(modeID);
            if (rankInfo == null)
            {
                return;
            }

            if (rankInfo.isRequest)
            {
                Requesting();
                return;
            }

            if (rankInfo.requestCount > 0)
            {
                RefreshRank();
            }
            else
            {
                RequestRank(false);
            }
        }

        private void RequestRank(bool register)
        {
            if (!modeIDByTab.TryGetValue(radio.current, out var modeID))
            {
                Fail();
                return;
            }

            var rankInfo = MyPlayer.Instance.core.mode.rank.GetRankInfo(modeID);
            if (rankInfo == null)
            {
                Fail();
                return;
            }

            Requesting();
            MyPlayer.Instance.core.mode.rank.RequestRank(modeID, register);
        }

        private void RefreshRank()
        {
            if (!modeIDByTab.TryGetValue(radio.current, out var modeID))
            {
                return;
            }

            var rankInfo = MyPlayer.Instance.core.mode.rank.GetRankInfo(modeID);
            if (rankInfo == null)
            {
                return;
            }

            if (rankInfo.isRequest)
            {
                return;
            }

            Clean();
            osaPool.DoReset();
            sortOsaItems.Clear();

            for (int i = 0; i < rankInfo.top50.Count; ++i)
            {
                var item = osaPool.Pop(i);
                var rank = rankInfo.top50[i];

                item.rank = rank;

                sortOsaItems.Add(item);
            }

            if (sortOsaItems.Count > 0)
            {
                osaScroll.gameObject.SetActive(true);
                osaScroll.SetItems(sortOsaItems);
            }
            else
            {
                None();
            }

            myRankCell.SetRank(rankInfo.myRank);
        }

        private void Clean()
        {
            myRankCell.DoReset();
            osaScroll.gameObject.SetActive(false);
            stateText.gameObject.SetActive(false);
        }

        private void Requesting()
        {
            Clean();
            stateText.gameObject.SetActive(true);
            stateText.SetText("key_ex_get_rank".L());
        }

        private void Fail()
        {
            Clean();
            stateText.gameObject.SetActive(true);
            stateText.SetText("key_ex_get_rank_fail".L());
        }

        private void None()
        {
            Clean();
            stateText.gameObject.SetActive(true);
            stateText.SetText("key_ex_get_rank_none".L());
        }

        private int GetModeAdID()
        {
            if (!modeIDByTab.TryGetValue(radio.current, out var modeID))
            {
                return GameData.DEFAULT.DEFAULT_SHOW_AD_ID;
            }

            var resMode = ResourceManager.Instance.mode.GetMode(modeID);
            if (resMode == null)
            {
                return GameData.DEFAULT.DEFAULT_SHOW_AD_ID;
            }

            return resMode.rankAdID;
        }

        private bool IsAdCoolTime()
        {
            var resAd = ResourceManager.Instance.ad.GetAd(GetModeAdID());
            if (resAd == null)
            {
                return false;
            }

            var ad = MyPlayer.Instance.core.ad.GetAd(resAd.id);
            var current = Main.Instance.time.nowToEpochSecond();

            return ad.coolTime > current;
        }

        private void Cmd_Refresh()
        {
            ClickSound();

            if (!modeIDByTab.TryGetValue(radio.current, out var modeID))
            {
                return;
            }

            var resMode = ResourceManager.Instance.mode.GetMode(modeID);
            if (resMode == null)
            {
                return;
            }

            if (IsAdCoolTime())
            {
                Main.Instance.ShowFloatingMessage("key_ex_ad_cool".L());
                return;
            }

            var adID = GetModeAdID();
            var resAd = ResourceManager.Instance.ad.GetAd(adID);
            if (resAd == null)
            {
                return;
            }

            var popup = PopupExtend.Instance.ShowRankRegister(resAd);
            popup.SetButton().SetTextKey("key_ok")
                .SetBackground(GameData.COLOR.OK_BUTTON, GameData.COLOR.OK_BUTTON_FRAME)
                .SetOnActive(() =>
                {
                    var mode = MyPlayer.Instance.core.mode.GetMode(resMode.id);
                    if (mode.GetBestScore() == 0)
                    {
                        Main.Instance.ShowFloatingMessage("key_ex_score_is_zero".L());
                        return;
                    }

                    if (resAd.isShow)
                    {
                        AdManager.Instance.ShowRewardedAd(AdManager.AdType.AD_MOB,
                            resAd,
                            (result) => 
                            {
                                if (result != AdManager.Result.Complete)
                                {
                                    return;
                                }

                                OnShowAdComplete(resAd);
                            });
                    }
                    else
                    {
                        OnShowAdComplete(resAd);
                    }
                });
        }

        private void OnShowAdComplete(ResourceAdvertisement resAd)
        {
            RequestRank(true);

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
                resAd.id);
        }


        void IMenuItem.On(int value)
        {
            var mode = ModeManager.Instance.mode;
            if (mode == null)
            {
                return;
            }

            On(mode.core.profile.resMode.id);
        }

        bool IMenuItem.Notice()
        {
            return false;
        }

        public class RankOsaItem : MyOSABasic.IOsaItem
        {
            public Rank rank = null;

            public void DoReset()
            {
                rank = null;
            }

            public bool IsEmpty()
            {
                return rank == null;
            }

            public int SortCompare(MyOSABasic.IOsaItem other)
            {
                return 0;
            }
        }
    }
}
