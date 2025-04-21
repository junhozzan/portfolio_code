using System.Collections.Generic;

namespace UnitComponent
{
    public class UnitItemComponent : UnitBaseComponent
    {
        public readonly UnitItemStatComponent stat = null;
        private readonly List<TItem> itemsByResource = new List<TItem>();
        private readonly List<TItem> allItems = new List<TItem>();



        private readonly static List<TItem> emptyItems = new List<TItem>();
        private readonly static List<int> emptyEquipedItemIDs = new List<int>();

        public UnitItemComponent(Unit owner) : base(owner)
        {
            stat = AddComponent<UnitItemStatComponent>(owner);
        }

        public override void DoReset()
        {
            base.DoReset();

            foreach (var item in itemsByResource)
            {
                item.OnDisable();
            }

            itemsByResource.Clear();
        }

        public override void OnDisable()
        {
            foreach (var item in itemsByResource)
            {
                item.OnDisable();
            }

            itemsByResource.Clear();

            base.OnDisable();
        }

        public void HandleTUnit()
        {
            foreach (var item in itemsByResource)
            {
                item.OnDisable();
            }

            itemsByResource.Clear();

            foreach (var itemID in owner.core.profile.tunit.resUnit.itemIDs)
            {
                var item = TManager.Instance.Get<TItem>();
                item.SetResID(itemID);

                itemsByResource.Add(item);
            }
        }

        public ICollection<TItem> GetResourceItems()
        {
            return itemsByResource;
        }

        public virtual ICollection<TItem> GetItems()
        {
            return emptyItems;
        }

        public ICollection<TItem> GetAllItems()
        {
            allItems.Clear();
            allItems.AddRange(GetResourceItems());
            allItems.AddRange(GetItems());

            return allItems;
        }

        public virtual List<int> GetEquipedItemIDs()
        {
            return emptyEquipedItemIDs;
        }
    }
}