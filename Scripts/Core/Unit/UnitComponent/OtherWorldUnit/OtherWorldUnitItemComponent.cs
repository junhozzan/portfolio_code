using System.Collections.Generic;

namespace UnitComponent
{
    public class OtherWorldUnitItemComponent : UnitItemComponent
    {

        public OtherWorldUnitItemComponent(Unit owner) : base(owner)
        {
        }

        public override List<int> GetEquipedItemIDs()
        {
            var mode = ModeManager.Instance.mode;
            if (mode == null)
            {
                return base.GetEquipedItemIDs();
            }

            var itemIDs = MyPlayer.Instance.core.mode.rank.GetTopRankerItemIDs(mode.core.profile.resMode.id);
            if (itemIDs != null)
            {
                return itemIDs;
            }

            var myUnit = mode.core.ally.myUnit;
            if (myUnit == null)
            {
                return base.GetEquipedItemIDs();
            }

            return myUnit.core.item.GetEquipedItemIDs();
        }
    }
}