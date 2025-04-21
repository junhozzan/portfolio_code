using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyPlayerComponent
{
    public class MyPlayerStoreComponent : MyPlayerBaseComponent, IMenuItem
    {
        private readonly Dictionary<int, TStore> stores = new Dictionary<int, TStore>();

        public MyPlayerStoreComponent(MyPlayer mp) : base(mp)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.GET_STORE_DATAS, Handle_GET_STORE_DATAS)
                .Add(GameEventType.PURCHASE_STORE_ITEM, Handle_PURCHASE_STORE_ITEM)
                ;
        }

        private void Handle_GET_STORE_DATAS(object[] args)
        {
            var tArg = GameEvent.GetSafe<GET_STORE_DATAS>(args, 0);
            if (tArg == null)
            {
                return;
            }

            UpdateStores(tArg.tstores);
        }

        private void Handle_PURCHASE_STORE_ITEM(object[] args)
        {
            var tstore = GameEvent.GetSafe<TStore>(args, 0);
            if (tstore == null)
            {
                return;
            }

            UpdateStore(tstore);
        }

        private void UpdateStores(IEnumerable<TStore> _stores)
        {
            foreach (var _s in _stores)
            {
                UpdateStore(_s);
            }
        }

        private void UpdateStore(TStore _s)
        {
            if (!stores.TryGetValue(_s.resID, out var s))
            {
                stores.Add(_s.resID, null);
            }

            if (s != null)
            {
                s.OnDisable();
            }

            stores[_s.resID] = _s;
        }

        public TStore GetStore(int id)
        {
            if (!stores.TryGetValue(id, out var v))
            {
                stores.Add(id, v = TManager.Instance.Get<TStore>().SetResID(id));
            }

            return v;
        }

        public ICollection<TStore> GetStores()
        {
            return stores.Values;
        }

        void IMenuItem.On(int value)
        {
            UIStore.CpUI_Store.Instance.On();
        }

        bool IMenuItem.Notice()
        {
            return false;
        }
    }
}