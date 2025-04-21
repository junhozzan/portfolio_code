namespace UnitComponent
{
    public class OtherWorldUnitCoreComponent : EnemyUnitCoreComponent
    {
        private OtherWorldUnitCoreComponent(Unit owner) : base(owner)
        {

        }

        public static new OtherWorldUnitCoreComponent Of(Unit owner)
        {
            return new OtherWorldUnitCoreComponent(owner);
        }

        protected override UnitItemComponent AddItemComponent()
        {
            return AddComponent<OtherWorldUnitItemComponent>(owner);
        }

        protected override UnitTargetComponent AddTargetComponent()
        {
            return AddComponent<OtherWorldUnitTargetComponent>(owner);
        }

        protected override UnitSkinComponent AddSkinComponent()
        {
            return AddComponent<OtherWorldUnitSkinComponent>(owner);
        }
    }
}