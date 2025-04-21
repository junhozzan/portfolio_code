using System.Collections.Generic;

namespace MyPlayerComponent
{
    public class MyPlayerItemComponent : MyPlayerBaseComponent
    {
        private readonly Dictionary<int, TItem> items = new Dictionary<int, TItem>();
        private readonly Dictionary<int, long> virtualAmounts = new Dictionary<int, long>();

        public MyPlayerItemComponent(MyPlayer mp) : base(mp)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.GET_ITEM_DATAS, Handle_GET_ITEM_DATAS)
                .Add(GameEventType.UPDATE_ITEM, Handle_UPDATE_ITEM)
                ;
        }

        private void Handle_GET_ITEM_DATAS(object[] args)
        {
            var tArg = GameEvent.GetSafe<GET_ITEM_DATAS>(args, 0);
            if (tArg == null)
            {
                return;
            }

            Clear();
            UpdateItems(tArg.titems);
        }

        private void Handle_UPDATE_ITEM(object[] args)
        {
            var titems = GameEvent.GetSafe<ICollection<TItem>>(args, 0);
            if (titems == null || titems.Count == 0)
            {
                return;
            }

            UpdateItems(titems);

            var updateFlag = GetUpdateFlag(titems);
            if (updateFlag.HasFlag(TItem.UpdateFlag.ALL))
            {
                GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM_ALL);
            }
            else if (updateFlag.HasFlag(TItem.UpdateFlag.NEW))
            {
                GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM_NEW);
            }
            else if (updateFlag.HasFlag(TItem.UpdateFlag.AMOUNT))
            {
                GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM_AMOUNT);
            }
        }

        public void Clear()
        {
            items.Clear();
        }

        private void UpdateItem(TItem newItem)
        {
            if (!items.TryGetValue(newItem.id, out var item))
            {
                items.Add(newItem.id, null);
            }
            else
            {
                item.OnDisable();
            }

            items[newItem.id] = newItem;
        }

        private void UpdateItems(IEnumerable<TItem> items)
        {
            foreach (var titem in items)
            {
                UpdateItem(titem);
            }
        }

        public ICollection<TItem> GetItems()
        {
            return items.Values;
        }

        public ICollection<int> GetItemIDs()
        {
            return items.Keys;
        }

        public bool TryGetItem(int resItemID, out TItem item)
        {
            return items.TryGetValue(resItemID, out item);
        }

        public void SetVirtualAmount(int resItemID, long amount)
        {
            if (!virtualAmounts.ContainsKey(resItemID))
            {
                virtualAmounts.Add(resItemID, 0);
            }

            virtualAmounts[resItemID] = amount;
        }

        public long GetAmount(int resItemID)
        {
            var amount = 0L;
            if (TryGetItem(resItemID, out var item))
            {
                amount += item.GetAmount();
            }

            if (virtualAmounts.TryGetValue(resItemID, out var v))
            {
                amount += v;
            }

            return amount;
        }

        private TItem.UpdateFlag GetUpdateFlag(ICollection<TItem> titems)
        {
            var flag = TItem.UpdateFlag.NONE;
            foreach (var item in titems)
            {
                flag |= item.updateFlag;
            }

            return flag;
        }
    }
}