using System.Collections.Generic;

namespace UnitComponent
{
    public class MyUnitSkinComponent : UnitSkinComponent
    {
        public MyUnitSkinComponent(Unit owner) : base(owner)
        {

        }

        public override ICollection<ResourceSPUM> GetCustomSkins()
        {
            return GetItemToSPUMs(owner.core.item.GetEquipedItemIDs());
        }
    }
}