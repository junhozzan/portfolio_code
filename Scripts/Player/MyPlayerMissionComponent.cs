using System.Collections.Generic;

namespace MyPlayerComponent
{
    public class MyPlayerMissionComponent : MyPlayerBaseComponent, IMenuItem
    {
        private readonly Dictionary<int, TMission> missions = new Dictionary<int, TMission>();

        public MyPlayerMissionComponent(MyPlayer mp) : base(mp)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.GET_MISSION_DATAS, Handle_GET_MISSION_DATAS)
                .Add(GameEventType.UPDATE_MISSION, Handle_UPDATE_MISSION)
                ;
        }

        private void Handle_GET_MISSION_DATAS(object[] args)
        {
            var tArg = GameEvent.GetSafe<GET_MISSION_DATAS>(args, 0);
            if (tArg == null)
            {
                return;
            }

            UpdateMissions(tArg.tmissions);
        }

        private void Handle_UPDATE_MISSION(object[] args)
        {
            var tmissions = GameEvent.GetSafe<ICollection<TMission>>(args, 0);
            if (tmissions == null)
            {
                return;
            }

            UpdateMissions(tmissions);
        }

        private void UpdateMissions(IEnumerable<TMission> _missions)
        {
            if (_missions == null)
            {
                return;
            }

            foreach (var _m in _missions)
            {
                UpdateMission(_m);
            }
        }

        private void UpdateMission(TMission _m)
        {
            if (!missions.TryGetValue(_m.resID, out var m))
            {
                missions.Add(_m.resID, null);
            }

            if (m != null)
            {
                m.OnDisable();
            }

            missions[_m.resID] = _m;
        }

        public TMission GetMission(int id)
        {
            if (!missions.TryGetValue(id, out var v))
            {
                missions.Add(id, v = TManager.Instance.Get<TMission>().SetResID(id));
            }

            return v;
        }

        public ICollection<TMission> GetMissions()
        {
            return missions.Values;
        }

        public bool IsClearable(int missionID)
        {
            if (!ResourceManager.Instance.mission.IsEnableMission(missionID))
            {
                return false;
            }

            var resMission = ResourceManager.Instance.mission.GetMission(missionID);
            if (resMission == null)
            {
                return false;
            }

            var mission = GetMission(missionID);



            return mission.GetValue() >= resMission.GetMaxPoint(mission.GetLevel());
        }

        public bool IsCompleted(int missionID)
        {
            if (!ResourceManager.Instance.mission.IsEnableMission(missionID))
            {
                return false;
            }

            var resMission = ResourceManager.Instance.mission.GetMission(missionID);
            if (resMission == null)
            {
                return false;
            }

            var mission = GetMission(missionID);
            return mission.GetLevel() >= resMission.maxLevel;
        }

        public bool IsExistClearMission()
        {
            var missions = MyPlayer.Instance.core.mission.GetMissions();
            foreach (var tmission in missions)
            {
                if (IsCompleted(tmission.resID))
                {
                    continue;
                }

                if (!IsClearable(tmission.resID))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public void ClearMission(int id)
        {
            var missionIDs = new List<int>();
            missionIDs.Add(id);

            VirtualServer.Send(Packet.CLEAR_MISSION,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out CLEAR_MISSION tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MISSION, tArg.tmissions);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                    GameEvent.Instance.AddEvent(GameEventType.SHOW_GET_INFO, tArg.getInfos);
                },
                missionIDs,
                0);
        }

        public void AllClearMission()
        {
            var missionIDs = new List<int>();

            foreach (var mission in MyPlayer.Instance.core.mission.GetMissions())
            {
                if (!ResourceManager.Instance.mission.IsEnableMission(mission.resID))
                {
                    continue;
                }

                var resMission = ResourceManager.Instance.mission.GetMission(mission.resID);
                if (resMission == null)
                {
                    continue;
                }

                if (IsCompleted(mission.resID))
                {
                    continue;
                }

                if (!IsClearable(mission.resID))
                {
                    continue;
                }

                missionIDs.Add(mission.resID);
            }

            if (missionIDs.Count == 0)
            {
                return;
            }

            VirtualServer.Send(Packet.CLEAR_MISSION,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out CLEAR_MISSION tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MISSION, tArg.tmissions);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                    GameEvent.Instance.AddEvent(GameEventType.SHOW_GET_INFO, tArg.getInfos);
                },
                missionIDs,
                1);
        }

        void IMenuItem.On(int value)
        {
            UIMission.CpUI_Mission.Instance.On(GameData.DEFAULT.MAIN_MISSION_GROUP_ID);
        }

        bool IMenuItem.Notice()
        {
            return IsExistClearMission();
        }
    }
}
