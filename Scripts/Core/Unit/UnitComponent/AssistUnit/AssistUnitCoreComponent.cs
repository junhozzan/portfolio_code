namespace UnitComponent
{
    public class AssistUnitCoreComponent : UnitCoreComponent
    {
        public readonly new AssistUnitProfileComponent profile = null;
        public readonly new AssistUnitTransformComponent transform = null;
        public readonly new AssistUnitSkinComponent skin = null;
        public readonly new AssistUnitDeadComponent dead = null;

        protected AssistUnitCoreComponent(Unit owner) : base(owner)
        {
            this.profile = base.profile as AssistUnitProfileComponent;
            this.transform = base.transform as AssistUnitTransformComponent;
            this.skin = base.skin as AssistUnitSkinComponent;
            this.dead = base.dead as AssistUnitDeadComponent;
        }

        public static new AssistUnitCoreComponent Of(Unit owner)
        {
            return new AssistUnitCoreComponent(owner);
        }

        protected override UnitProfileComponent AddProfileComponent()
        {
            return AddComponent<AssistUnitProfileComponent>(owner);
        }

        protected override UnitSkillComponent AddSkillComponent()
        {
            return AddComponent<AssistUnitSkillComponent>(owner);
        }

        protected override UnitBuffComponent AddBuffComponent()
        {
            return AddComponent<AssistUnitBuffComponent>(owner);
        }

        protected override UnitStatComponent AddStatComponent()
        {
            return AddComponent<AssistUnitStatComponent>(owner);
        }

        protected override UnitDamageComponent AddDamageComponent()
        {
            return AddComponent<AssistUnitDamageComponent>(owner);
        }

        protected override UnitDamagedComponent AddDamagedComponent()
        {
            return AddComponent<AssistUnitDamagedComponent>(owner);
        }

        protected override UnitTransformComponent AddTransformComponent()
        {
            return AddComponent<AssistUnitTransformComponent>(owner);
        }

        protected override UnitSkinComponent AddSkinComponent()
        {
            return AddComponent<AssistUnitSkinComponent>(owner);
        }

        protected override UnitTargetComponent AddTargetComponent()
        {
            return AddComponent<AssistUnitTargetComponent>(owner);
        }

        protected override UnitItemComponent AddItemComponent()
        {
            return AddComponent<AssistUnitItemComponent>(owner);
        }

        protected override UnitFoFComponent AddFoFComponent()
        {
            return AddComponent<AssistUnitFoFComponent>(owner);
        }

        protected override UnitDeadComponent AddDeadComponent()
        {
            return AddComponent<AssistUnitDeadComponent>(owner);
        }

        protected override UnitRefreshComponent AddRefreshComponent()
        {
            return AddComponent<AssistUnitRefreshComponent>(owner);
        }

        protected override UnitKillComponent AddKillComponent()
        {
            return AddComponent<AssistUnitKillComponent>(owner);
        }
    }
}