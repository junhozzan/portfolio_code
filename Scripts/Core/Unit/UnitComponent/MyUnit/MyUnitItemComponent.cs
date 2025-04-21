using System.Collections.Generic;

namespace UnitComponent
{
    public class MyUnitItemComponent : UnitItemComponent
    {
        private readonly List<TItem> items = new List<TItem>();

        public MyUnitItemComponent(Unit owner) : base(owner)
        {

        }

        public override ICollection<TItem> GetItems()
        {
            items.Clear();
            foreach (var item in MyPlayer.Instance.core.item.GetItems())
            {
                var resItem = item.resItem;
                if (resItem == null || resItem.IsWealth())
                {
                    continue;
                }

                items.Add(item);
            }

            return items;
        }

        public override List<int> GetEquipedItemIDs()
        {
            return MyPlayer.Instance.core.inventory.GetEquipedItemIDs();
        }
    }
}