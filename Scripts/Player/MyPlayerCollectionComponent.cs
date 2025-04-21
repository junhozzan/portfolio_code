using System;
using System.Collections.Generic;

namespace MyPlayerComponent
{
    public class MyPlayerCollectionComponent : MyPlayerBaseComponent, IMenuItem
    {
        private readonly List<StatItem.Param> statItemParams = new List<StatItem.Param>(128);
        private readonly Dictionary<int, TCollection> collections = new Dictionary<int, TCollection>();
        private readonly List<int> sendCollectionIDs = new List<int>();

        public MyPlayerCollectionComponent(MyPlayer mp) : base(mp)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.GET_COLLECTION_DATAS, Handle_GET_COLLECTION_DATAS)
                .Add(GameEventType.UPDATE_COLLECTION, Handle_UPDATE_COLLECTION)
                ;
        }

        private void Handle_GET_COLLECTION_DATAS(object[] args)
        {
            var tArg = GameEvent.GetSafe<GET_COLLECTION_DATAS>(args, 0);
            if (tArg == null)
            {
                return;
            }

            UpdateCollections(tArg.tcollections);
        }

        private void Handle_UPDATE_COLLECTION(object[] args)
        {
            var tcollections = GameEvent.GetSafe<ICollection<TCollection>>(args, 0);
            if (tcollections == null)
            {
                return;
            }

            UpdateCollections(tcollections);
        }

        private void UpdateCollections(IEnumerable<TCollection> tcollections)
        {
            foreach (var tcollection in tcollections)
            {
                UpdateCollection(tcollection);
            }
        }

        private void UpdateCollection(TCollection tcollection)
        {
            if (!collections.TryGetValue(tcollection.resID, out var v))
            {
                collections.Add(tcollection.resID, null);
            }

            if (v != null)
            {
                v.OnDisable();
            }

            collections[tcollection.resID] = tcollection;
        }

        public TCollection GetCollection(int id)
        {
            if (!collections.TryGetValue(id, out var v))
            {
                var temp = TManager.Instance.Get<TCollection>();
                temp.SetResID(id);

                collections.Add(id, v = temp);
            }

            return v;
        }

        public ICollection<TCollection> GetCollections()
        {
            return collections.Values;
        }

        public void EnrollData(int resID)
        {
            sendCollectionIDs.Clear();
            sendCollectionIDs.Add(resID);

            VirtualServer.Send(Packet.ENROLL_COLLECTION,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out ENROLL_COLLECTION tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_COLLECTION, tArg.tcollections);
                },
                sendCollectionIDs);

            sendCollectionIDs.Clear();
        }

        public bool CanEnrollData(int resID)
        {
            var res = ResourceManager.Instance.collection.GetCollection(resID);
            if (res == null)
            {
                return false;
            }

            var collection = GetCollection(res.id);
            if (collection.IsComplete())
            {
                return false;
            }

            foreach (var data in res.datas)
            {
                if (mp.core.item.TryGetItem(data.itemID, out var item))
                {
                    if (item.GetLevel() < data.itemLevel)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public List<StatItem.Param> GetStatItemParams(Unit target)
        {
            statItemParams.Clear();

            var from = MyUnit.Instance;
            if (from == null || !UnitRule.IsValid(from))
            {
                return statItemParams;
            }

            foreach (var collection in collections.Values)
            {
                if (!collection.IsComplete())
                {
                    continue;
                }

                var resCollection = ResourceManager.Instance.collection.GetCollection(collection.resID);
                if (resCollection == null)
                {
                    continue;
                }

                statItemParams.AddRange(StatItem.GetParams(from, target, resCollection.targetAbilities, StatItem.Param.RiseParam.zero, null));
            }

            return statItemParams;
        }

        void IMenuItem.On(int value)
        {
            UICollection.CpUI_Collection.Instance.On();
        }

        bool IMenuItem.Notice()
        {
            foreach (var res in ResourceManager.Instance.collection.GetCollections())
            {
                if (!CanEnrollData(res.id))
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}