using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyPlayerComponent
{
    public class MyPlayerInventoryEquipmentComponent : MyPlayerInventoryBaseComponent
    {
        private readonly ReadOnlyCollection<ItemFilterType> filterTypes = new List<ItemFilterType>
        {
            ItemFilterType.ALL,
            ItemFilterType.EQUIP,
            ItemFilterType.WEAPON,
            ItemFilterType.ARMOR,
            ItemFilterType.JEW,
            ItemFilterType.STURDY,
            ItemFilterType.CURSED,

        }.ReadOnly();

        public MyPlayerInventoryEquipmentComponent(MyPlayer mp) : base(mp)
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
                case ItemFilterType.ALL: return !resItem.IsAvatar();
                case ItemFilterType.EQUIP: return !resItem.IsAvatar() && MyPlayer.Instance.core.inventory.IsEquipedItem(resItem.id);
                case ItemFilterType.WEAPON: return !resItem.IsAvatar() && resItem.IsWeapon();
                case ItemFilterType.ARMOR: return !resItem.IsAvatar() && resItem.IsArmor();
                case ItemFilterType.JEW: return !resItem.IsAvatar() && resItem.itemType == ItemType.JEW;
                case ItemFilterType.STURDY: return resItem.setType == ItemSetType.STURDY;
                case ItemFilterType.CURSED: return resItem.setType == ItemSetType.CURSED;
            }

            return false;
        }
    }
}