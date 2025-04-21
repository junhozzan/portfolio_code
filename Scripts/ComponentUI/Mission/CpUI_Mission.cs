using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIMission
{
    public class CpUI_Mission : UIMonoBehaviour
    {
        private static CpUI_Mission instance = null;
        public static CpUI_Mission Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_Mission>("pf_ui_mission_v");
                }

                return instance;
            }
        }

        [SerializeField] MyOSABasic osaScroll = null;
        [SerializeField] GameObject clearAllButton = null;
        [SerializeField] GameObject exitButton = null;
        [SerializeField] UIWealth uiWealthOrigin = null;

        private readonly MyOSABasic.OsaPool<MissionOsaItem> osaPool = new MyOSABasic.OsaPool<MissionOsaItem>();
        private readonly List<MyOSABasic.IOsaItem> sortOsaItems = new List<MyOSABasic.IOsaItem>();

        private ObjectPool<UIWealth> uiWealthPool = null;
        private Cmd cmdAllClear = null;

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.CONTENTS, true);
            UsingBlind(false);
            UsingUpdate();

            uiWealthPool = ObjectPool<UIWealth>.Of(uiWealthOrigin, uiWealthOrigin.transform.parent);

            if (osaScroll != null)
            {
                osaScroll.Init(OnCreateCell);
            }

            cmdAllClear = Cmd.Add(clearAllButton, eCmdTrigger.OnClick, Cmd_ClearAll);
            Cmd.Add(exitButton, eCmdTrigger.OnClick, Cmd_Close);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.UPDATE_MISSION, Refresh)
                ;
        }

        private void OnCreateCell(MyOSABasicItem item)
        {
            if (item is CpUI_Mission_Cell cell)
            {
                cell.Init();
            }
        }

        public void On(int groupID)
        {
            var resMissionGroup = ResourceManager.Instance.mission.GetMissionGroup(groupID);
            if (resMissionGroup == null)
            {
                return;
            }

            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            osaPool.DoReset();
            sortOsaItems.Clear();

            for (int i = 0; i < resMissionGroup.missionIDs.Count; ++i)
            {
                var resMission = ResourceManager.Instance.mission.GetMission(resMissionGroup.missionIDs[i]);
                if (resMission == null)
                {
                    continue;
                }

                var osaItem = osaPool.Pop(i);
                osaItem.resMission = resMission;

                sortOsaItems.Add(osaItem);
            }

            sortOsaItems.Sort((a, b) => { return a.SortCompare(b); });

            if (osaScroll != null)
            {
                osaScroll.SetItems(sortOsaItems);
            }

            RefreshClearAllButton();
            RefreshUIWealth();
        }

        protected override void RefreshInternal()
        {
            if (osaScroll != null)
            {
                osaScroll.Refresh();
            }

            RefreshClearAllButton();
            RefreshUIWealth();
        }

        private void RefreshUIWealth()
        {
            uiWealthPool.Clear();
            uiWealthPool.Pop().SetWealthID(GameData.ITEM_DATA.GEM).Refresh();
        }

        private void RefreshClearAllButton()
        {
            cmdAllClear.Use(MyPlayer.Instance.core.mission.IsExistClearMission());
        }

        private void Cmd_ClearAll()
        {
            ClickSound();
            MyPlayer.Instance.core.mission.AllClearMission();
        }

        public class MissionOsaItem : MyOSABasic.IOsaItem
        {
            public ResourceMission resMission = null;

            public void DoReset()
            {
                resMission = null;
            }

            public bool IsEmpty()
            {
                return resMission == null;
            }

            public int SortCompare(MyOSABasic.IOsaItem other)
            {
                if (other is MissionOsaItem mOther)
                {
                    var completedA = MyPlayer.Instance.core.mission.IsCompleted(resMission.id);
                    var completedB = MyPlayer.Instance.core.mission.IsCompleted(mOther.resMission.id);
                    if (!completedA && completedB)
                    {
                        return -1;
                    }
                    else if (completedA && !completedB)
                    {
                        return 1;
                    }

                    var clearableA = MyPlayer.Instance.core.mission.IsClearable(resMission.id);
                    var clearableB = MyPlayer.Instance.core.mission.IsClearable(mOther.resMission.id);

                    if (clearableA && !clearableB)
                    {
                        return -1;
                    }
                    else if (!clearableA && clearableB)
                    {
                        return 1;
                    }

                    return 0;
                }

                return 0;
            }
        }
    }
}