using System.Collections.Generic;

namespace MyPlayerComponent
{
    public class MyPlayerModeComponent : MyPlayerBaseComponent
    {
        public readonly MyPlayerModeRankComponent rank = null;

        private readonly Dictionary<int, TMode> modes = new Dictionary<int, TMode>();

        public MyPlayerModeComponent(MyPlayer mp) : base(mp)
        {
            rank = AddComponent<MyPlayerModeRankComponent>(mp);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.GET_MODE_DATAS, Handle_GET_MODE_DATAS)
                .Add(GameEventType.UPDATE_MODE, Handle_UPDATE_MODE)
                .Add(GameEventType.MODE_FAIL, Handle_MODE_FAIL)
                ;
        }

        private void Handle_GET_MODE_DATAS(object[] args)
        {
            var tArg = GameEvent.GetSafe<GET_MODE_DATAS>(args, 0);
            if (tArg == null)
            {
                return;
            }
            
            Clear();
            UpdateModes(tArg.tmodes);
        }

        private void Handle_UPDATE_MODE(object[] args)
        {
            var tmode = GameEvent.GetSafe<TMode>(args, 0);
            if (tmode == null)
            {
                return;
            }

            UpdateMode(tmode);
        }

        private void Handle_MODE_FAIL(object[] args)
        {
            var tmode = GameEvent.GetSafe<TMode>(args, 0);
            if (tmode == null)
            {
                return;
            }

            UpdateMode(tmode);
        }

        private void Clear()
        {
            modes.Clear();
        }

        private void UpdateMode(TMode tmode)
        {
            if (!modes.TryGetValue(tmode.id, out var v))
            {
                modes.Add(tmode.id, null);
            }

            if (v != null)
            {
                v.OnDisable();
            }

            modes[tmode.id] = tmode;
        }

        private void UpdateModes(IEnumerable<TMode> modes)
        {
            foreach (var tmode in modes)
            {
                UpdateMode(tmode);
            }
        }

        public TMode GetMode(int id)
        {
            if (!modes.TryGetValue(id, out var v))
            {
                modes.Add(id, v = TManager.Instance.Get<TMode>().SetResID(id));
            }

            return v;
        }
    }
}