using System;
using System.Collections.Generic;

namespace MyPlayerComponent
{
    public class MyPlayerLabComponent : MyPlayerBaseComponent, IMenuItem
    {
        private readonly Dictionary<int, TLab> labs = new Dictionary<int, TLab>();
        private readonly Dictionary<int, long> virtualAddLevels = new Dictionary<int, long>(); 
        private readonly List<StatItem.Param> statItemParams = new List<StatItem.Param>(128);

        public MyPlayerLabComponent(MyPlayer mp) : base(mp)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.GET_LAB_DATAS, Handle_GET_LAB_DATAS)
                .Add(GameEventType.UPDATE_LAB, Handle_UPDATE_LAB)
                ;
        }

        private void Handle_GET_LAB_DATAS(object[] args)
        {
            var tArg = GameEvent.GetSafe<GET_LAB_DATAS>(args, 0);
            if (tArg == null)
            {
                return;
            }

            UpdateLabs(tArg.tlabs);
        }

        private void Handle_UPDATE_LAB(object[] args)
        {
            ResetVirtualLevels();

            var tlab = GameEvent.GetSafe<TLab>(args, 0);
            if (tlab == null)
            {
                return;
            }

            UpdateLab(tlab);
        }

        private void UpdateLabs(IEnumerable<TLab> labs)
        {
            foreach (var lab in labs)
            {
                UpdateLab(lab);
            }
        }

        private void UpdateLab(TLab lab)
        {
            if (!labs.TryGetValue(lab.resID, out var v))
            {
                labs.Add(lab.resID, null);
            }

            if (v != null)
            {
                v.OnDisable();
            }

            labs[lab.resID] = lab;
        }

        public void ResetVirtualLevels()
        {
            virtualAddLevels.Clear();
        }

        public void AddVirtualLevel(int id, long add)
        {
            if (!virtualAddLevels.ContainsKey(id))
            {
                virtualAddLevels.Add(id, 0);
            }

            virtualAddLevels[id] += add;
        }

        public long GetLevel(int id)
        {
            var res = ResourceManager.Instance.lab.GetLab(id);
            if (res == null)
            {
                return 0;
            }

            var a = labs.TryGetValue(id, out var v1) ? v1.GetLevel() : 0;
            var b = virtualAddLevels.TryGetValue(id, out var v2) ? v2 : 0;

            return Math.Min(a + b, res.maxLevel);
        }

        public void LevelUp(int id, int add)
        {
            VirtualServer.Send(Packet.UPGRADE_LAB,
                (arg) =>
                {

                    if (!VirtualServer.TryGet(arg, out UPDATE_LAB tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_LAB, tArg.tlab);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MISSION, tArg.tmissions);
                },
                id,
                add);
        }

        public List<StatItem.Param> GetStatItemParams(Unit target)
        {
            statItemParams.Clear();

            var from = MyUnit.Instance;
            if (from == null || !UnitRule.IsValid(from))
            {
                return statItemParams;
            }

            foreach (var lab in labs.Values)
            {
                var resLab = ResourceManager.Instance.lab.GetLab(lab.resID);
                if (resLab == null)
                {
                    continue;
                }

                statItemParams.AddRange(StatItem.GetParams(from, target, resLab.targetAbilities, StatItem.Param.RiseParam.Of(lab.GetLevel(), 0), null));
            }

            return statItemParams;
        }

        public ICollection<TLab> GetLabs()
        {
            return labs.Values;
        }

        void IMenuItem.On(int value)
        {
            UILab.CpUI_Lab.Instance.On();
        }

        bool IMenuItem.Notice()
        {
            return false;
        }
    }
}

