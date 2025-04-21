using System.Collections.Generic;

namespace UnitComponent
{
    public class OtherWorldUnitSkinComponent : EnemyUnitSkinComponet
    {
        public OtherWorldUnitSkinComponent(Unit owner) : base(owner)
        {

        }

        public override ICollection<ResourceSPUM> GetCustomSkins()
        {
            var mode = ModeManager.Instance.mode;
            if (mode == null)
            {
                return base.GetCustomSkins();
            }

            var equipedItemIDs = MyPlayer.Instance.core.mode.rank.GetTopRankerItemIDs(mode.core.profile.resMode.id);
            if (equipedItemIDs != null)
            {
                return GetItemToSPUMs(equipedItemIDs);
            }

            return mode.core.ally.myUnit.core.skin.GetCustomSkins();
        }
    }
}