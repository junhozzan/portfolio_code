using System.Collections.Generic;

namespace UnitComponent
{
    public class EnemyUnitSkinCustomPartsComponent : UnitBaseComponent
    {
        private readonly List<ResourceSPUM> parts = new List<ResourceSPUM>();

        public EnemyUnitSkinCustomPartsComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            parts.Clear();
        }

        public void Refresh()
        {
            parts.Clear();
            foreach (var customPartsID in owner.core.profile.tunit.resUnit.customPartsIDs)
            {
                var resCustomParts = ResourceManager.Instance.spum.GetCustomParts(customPartsID);
                if (resCustomParts == null)
                {
                    continue;
                }

                var resSpum = ResourceManager.Instance.spum.GetSPUM(resCustomParts.GetRandomSpumID());
                if (resSpum == null)
                {
                    continue;
                }

                parts.Add(resSpum);
            }
        }

        public ICollection<ResourceSPUM> GetSkins()
        {
            return parts;
        }
    }
}