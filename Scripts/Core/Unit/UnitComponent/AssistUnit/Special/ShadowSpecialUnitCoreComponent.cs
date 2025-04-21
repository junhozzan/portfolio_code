namespace UnitComponent
{
    public class ShadowSpecialUnitCoreComponent : AssistUnitCoreComponent
    {
        protected ShadowSpecialUnitCoreComponent(Unit owner) : base(owner)
        {

        }

        public static new ShadowSpecialUnitCoreComponent Of(Unit owner)
        {
            return new ShadowSpecialUnitCoreComponent(owner);
        }

        protected override UnitUIComponent AddUIComponent()
        {
            return AddComponent<ShadowSpecialUnitUIComponent>(owner);
        }
    }
}