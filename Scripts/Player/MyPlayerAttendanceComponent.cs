using System;
using System.Collections.Generic;
using System.Linq;

namespace MyPlayerComponent
{
    public class MyPlayerAttendanceComponent : MyPlayerBaseComponent, IMenuItem
    {
        private readonly Dictionary<int, TAttendance> attendances = new Dictionary<int, TAttendance>();
        private readonly List<int> sendDayList = new List<int>();

        public MyPlayerAttendanceComponent(MyPlayer mp) : base(mp)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.GET_ATTENDANCE_DATAS, Handle_GET_ATTENDANCE_DATAS)
                .Add(GameEventType.JOIN_FIELD, Handle_JOIN_FIELD)
                .Add(GameEventType.UPDATE_ATTENDANCE, Handle_UPDATE_ATTENDANCE)
                ;
        }

        private void Handle_JOIN_FIELD(object[] args)
        {
            var now = Main.Instance.time.now;
            var openAttendances = ResourceManager.Instance.attendance.GetAttendances()
                .Where(x => x.IsValidDate(now))
                .OrderBy(x => x.sort)
                .Select(x => GetAttendance(x.id))
                .Where(x => IsExistDayReward(x.resID))
                .Select(x => x.resID)
                .ToList();

            UIAttendance.CpUI_Attendance.Instance.On(openAttendances);
        }

        private void Handle_GET_ATTENDANCE_DATAS(object[] args)
        {
            var tArg = GameEvent.GetSafe<GET_ATTENDANCE_DATAS>(args, 0);
            if (tArg == null)
            {
                return;
            }

            UpdateAttendances(tArg.tattendances);
        }

        private void Handle_UPDATE_ATTENDANCE(object[] args)
        {
            var tattendance = GameEvent.GetSafe<TAttendance>(args, 0);
            if (tattendance == null)
            {
                return;
            }

            UpdateAttendance(tattendance);
        }

        private void UpdateAttendances(IEnumerable<TAttendance> _attendances)
        {
            foreach (var _a in _attendances)
            {
                UpdateAttendance(_a);
            }
        }

        private void UpdateAttendance(TAttendance _a)
        {
            if (!attendances.TryGetValue(_a.resID, out var a))
            {
                attendances.Add(_a.resID, null);
            }

            if (a != null)
            {
                a.OnDisable();
            }

            attendances[_a.resID] = _a;
        }

        public TAttendance GetAttendance(int resID)
        {
            if (!attendances.TryGetValue(resID, out var v))
            {
                attendances.Add(resID, v = TManager.Instance.Get<TAttendance>());
            }

            return v;
        }

        public void ClearAttendance(int resID, int day)
        {
            sendDayList.Clear();
            sendDayList.Add(day);

            VirtualServer.Send(Packet.CLEAR_ATTENDANCE,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out CLEAR_ATTENDANCE tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ATTENDANCE, tArg.tattendance);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                    GameEvent.Instance.AddEvent(GameEventType.SHOW_GET_INFO, tArg.getInfos, new Action(UIAttendance.CpUI_Attendance.Instance.Off));
                },
                resID,
                sendDayList);

            sendDayList.Clear();
        }

        public void AllClearAttendance(int resID)
        {
            sendDayList.Clear();

            if (!attendances.TryGetValue(resID, out var attendance))
            {
                return;
            }

            for (int day = 1; day <= attendance.GetCount(); ++day)
            {
                var flag = (long)1 << day;
                if ((attendance.GetFlag() & flag) == flag)
                {
                    continue;
                }

                sendDayList.Add(day);
            }

            if (sendDayList.Count == 0)
            {
                return;
            }

            VirtualServer.Send(Packet.CLEAR_ATTENDANCE,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out CLEAR_ATTENDANCE tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ATTENDANCE, tArg.tattendance);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                    GameEvent.Instance.AddEvent(GameEventType.SHOW_GET_INFO, tArg.getInfos, new Action(UIAttendance.CpUI_Attendance.Instance.Off));
                },
                resID,
                sendDayList);

            sendDayList.Clear();
        }

        public bool IsExistDayReward(int resID)
        {
            if (attendances.TryGetValue(resID, out var attendance))
            {
                var resAttendance = ResourceManager.Instance.attendance.GetAttendance(resID);
                if (resAttendance == null)
                {
                    return false;
                }

                for (int day = 1; day <= Math.Min(attendance.GetCount(), resAttendance.dayCount); ++day)
                {
                    var flag = (long)1 << day;
                    if ((attendance.GetFlag() & flag) == flag)
                    {
                        continue;
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 완료 이후 하루 이상 지난 출석인지 확인용
        /// </summary>
        /// <returns></returns>
        public bool IsCompleteByOverOneDay(ResourceAttendance res)
        {
            var attendance = GetAttendance(res.id);
            if ((attendance.GetFlag() & res.completeFlag) != res.completeFlag)
            {
                return false;
            }

            var nowEpochSec = Main.Instance.time.nowToEpochSecond();
            if (nowEpochSec - attendance.GetCompleteDate() < 86400)
            {
                return false;
            }

            return true;
        }

        void IMenuItem.On(int value)
        {
            var now = Main.Instance.time.now;
            // 모든 출석 오픈 확인
            var openAttendances = ResourceManager.Instance.attendance.GetAttendances()
                .Where(x => x.IsValidDate(now) && !IsCompleteByOverOneDay(x))
                .OrderBy(x => x.sort)
                .Select(x => x.id)
                .ToList();

            if (openAttendances.Count == 0)
            {
                Main.Instance.ShowFloatingMessage("key_ex_attendance_0");
                return;
            }

            UIAttendance.CpUI_Attendance.Instance.On(openAttendances);
        }

        bool IMenuItem.Notice()
        {
            foreach (var resID in attendances.Keys)
            {
                if (IsExistDayReward(resID))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
