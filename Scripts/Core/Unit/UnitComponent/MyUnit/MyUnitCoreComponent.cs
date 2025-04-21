namespace UnitComponent
{
    public class MyUnitCoreComponent : UnitCoreComponent
    {
        public readonly new MyUnitDamagedComponent damaged = null;
        public readonly new MyUnitRefreshComponent refresh = null;
        public readonly new MyUnitSpawnComponent spawn = null;
        public readonly new MyUnitTransformComponent transform = null;
        public readonly new MyUnitDamageComponent damage = null;

        private MyUnitCoreComponent(Unit owner) : base(owner)
        {
            this.damaged = base.damaged as MyUnitDamagedComponent;
            this.refresh = base.refresh as MyUnitRefreshComponent;
            this.spawn = base.spawn as MyUnitSpawnComponent;
            this.transform = base.transform as MyUnitTransformComponent;
            this.damage = base.damage as MyUnitDamageComponent;
        }

        public static new MyUnitCoreComponent Of(Unit owner)
        {
            return new MyUnitCoreComponent(owner);
        }

        protected override UnitTransformComponent AddTransformComponent()
        {
            return AddComponent<MyUnitTransformComponent>(owner);
        }

        protected override UnitDamagedComponent AddDamagedComponent()
        {
            return AddComponent<MyUnitDamagedComponent>(owner);
        }

        protected override UnitAnimComponent AddAnimComponent()
        {
            return AddComponent<MyUnitAnimComponent>(owner);
        }

        protected override UnitItemComponent AddItemComponent()
        {
            return AddComponent<MyUnitItemComponent>(owner);
        }

        protected override UnitRefreshComponent AddRefreshComponent()
        {
            return AddComponent<MyUnitRefreshComponent>(owner);
        }

        protected override UnitSpawnComponent AddSpawnComponent()
        {
            return AddComponent<MyUnitSpawnComponent>(owner);
        }

        protected override UnitTargetComponent AddTargetComponent()
        {
            return AddComponent<MyUnitTargetComponent>(owner);
        }

        protected override UnitSkinComponent AddSkinComponent()
        {
            return AddComponent<MyUnitSkinComponent>(owner);
        }

        protected override UnitStatComponent AddStatComponent()
        {
            return AddComponent<MyUnitStatComponent>(owner);
        }

        protected override UnitHealthComponent AddHealthComponent()
        {
            return AddComponent<MyUnitHealthComponent>(owner);
        }

        //protected override UnitManaComponent AddManaComponent()
        //{
        //    return AddComponent<MyUnitManaComponent>(owner);
        //}

        protected override UnitDamageComponent AddDamageComponent()
        {
            return AddComponent<MyUnitDamageComponent>(owner);
        }
    }
}