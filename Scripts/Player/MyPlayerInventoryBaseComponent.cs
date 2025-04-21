using System.Collections.Generic;
using System.Linq;

namespace MyPlayerComponent
{
    public class MyPlayerInventoryBaseComponent : MyPlayerBaseComponent, IInventory
    {
        protected int selectedItemID = 0;
        protected ItemFilterType selectedFilterType = ItemFilterType.ALL;
        private readonly TItem tempItem = TItem.Of();

        public MyPlayerInventoryBaseComponent(MyPlayer mp) : base(mp)
        {

        }

        public int GetSelectedItemID()
        {
            return selectedItemID;
        }

        public bool IsSelectedItem(int itemID)
        {
            return selectedItemID == itemID;
        }

        public void SetSelectItem(int itemID)
        {
            selectedItemID = itemID;
        }

        public virtual bool IsFiltering(ResourceItem resItem)
        {
            return false;
        }

        public void SetFilterType(ItemFilterType itemFilterType)
        {
            selectedFilterType = itemFilterType;
        }

        public  ItemFilterType GetFilterType()
        {
            return selectedFilterType;
        }

        public virtual ICollection<ItemFilterType> GetSortTypes()
        {
            return null;
        }

        public virtual bool TryGetItem(int id, out TItem item)
        {
            if (id < 0)
            {
                item = null;
                return false;
            }

            if (!mp.core.item.TryGetItem(id, out item))
            {
                item = tempItem.SetResID(id);
            }

            return item != null;
        }

        public virtual IEnumerable<ResourceItem> GetViewItems()
        {
            return ResourceManager.Instance.item.GetItems()
                .Where(x => x.itemType != ItemType.WEALTH && x.itemType != ItemType.VIRTUAL);
        }
    }
}