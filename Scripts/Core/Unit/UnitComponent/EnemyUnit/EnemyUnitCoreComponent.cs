namespace UnitComponent
{
    public class EnemyUnitCoreComponent : UnitCoreComponent
    {
        public readonly new EnemyUnitSkinComponet skin = null;

        protected EnemyUnitCoreComponent(Unit owner) : base(owner)
        {
            this.skin = base.skin as EnemyUnitSkinComponet;
        }

        public static new EnemyUnitCoreComponent Of(Unit owner)
        {
            return new EnemyUnitCoreComponent(owner);
        }

        protected override UnitTransformComponent AddTransformComponent()
        {
            return AddComponent<EnemyUnitTransformComponent>(owner);
        }

        protected override UnitDamagedComponent AddDamagedComponent()
        {
            return AddComponent<EnemyUnitDamagedComponent>(owner);
        }

        protected override UnitSkinComponent AddSkinComponent()
        {
            return AddComponent<EnemyUnitSkinComponet>(owner);
        }

        protected override UnitDeadComponent AddDeadComponent()
        {
            return AddComponent<EnemyUnitDeadComponent>(owner);
        }

        protected override UnitRefreshComponent AddRefreshComponent()
        {
            return AddComponent<EnemyUnitRefreshComponent>(owner);
        }

        protected override UnitTargetComponent AddTargetComponent()
        {
            return AddComponent<EnemyUnitTargetComponent>(owner);
        }
    }
}