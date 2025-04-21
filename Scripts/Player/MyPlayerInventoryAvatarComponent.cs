using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyPlayerComponent
{
    public class MyPlayerInventoryAvatarComponent : MyPlayerInventoryBaseComponent
    {
        private readonly ReadOnlyCollection<ItemFilterType> filterTypes = new List<ItemFilterType>
        {
            ItemFilterType.EQUIP,
            ItemFilterType.WEAPON,
            ItemFilterType.ARMOR,
            ItemFilterType.ALL,
        }.ReadOnly();

        public MyPlayerInventoryAvatarComponent(MyPlayer mp) : base(mp)
        {

        }

        public override ICollection<ItemFilterType> GetSortTypes()
        {
            return filterTypes;
        }

        public override bool IsFiltering(ResourceItem resItem)
        {
            switch (selectedFilterType)
            {
                case ItemFilterType.ALL: return resItem.IsAvatar();
                case ItemFilterType.EQUIP: return resItem.IsAvatar() && MyPlayer.Instance.core.inventory.IsEquipedItem(resItem.id);
                case ItemFilterType.WEAPON: return resItem.IsAvatar() && resItem.IsWeapon();
                case ItemFilterType.ARMOR: return resItem.IsAvatar() && resItem.IsArmor();
            }

            return false;
        }
    }
}