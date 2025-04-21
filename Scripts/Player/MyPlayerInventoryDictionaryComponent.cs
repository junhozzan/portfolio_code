using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyPlayerComponent
{
    public class MyPlayerInventoryDictionaryComponent : MyPlayerInventoryBaseComponent
    {
        public MyPlayerInventoryDictionaryComponent(MyPlayer mp) : base(mp)
        {
            selectedFilterType = ItemFilterType.GRADE_1;
        }

        private readonly ReadOnlyCollection<ItemFilterType> filterTypes = new List<ItemFilterType>
        {
            ItemFilterType.GRADE_1,
            ItemFilterType.GRADE_2,
            ItemFilterType.GRADE_3,
            ItemFilterType.GRADE_4,
            ItemFilterType.GRADE_5,
            ItemFilterType.GRADE_6,
            ItemFilterType.GRADE_7,
        }.ReadOnly();

        public override ICollection<ItemFilterType> GetSortTypes()
        {
            return filterTypes;
        }

        public override bool IsFiltering(ResourceItem resItem)
        {
            if (resItem.IsAvatar())
            {
                return false;
            }

            switch (selectedFilterType)
            {
                case ItemFilterType.GRADE_1: return resItem.grade == 1;
                case ItemFilterType.GRADE_2: return resItem.grade == 2;
                case ItemFilterType.GRADE_3: return resItem.grade == 3;
                case ItemFilterType.GRADE_4: return resItem.grade == 4;
                case ItemFilterType.GRADE_5: return resItem.grade == 5;
                case ItemFilterType.GRADE_6: return resItem.grade == 6;
                case ItemFilterType.GRADE_7: return resItem.grade == 7;
            }

            return false;
        }

        public override IEnumerable<ResourceItem> GetViewItems()
        {
            return ResourceManager.Instance.item.GetItems();
        }
    }
}