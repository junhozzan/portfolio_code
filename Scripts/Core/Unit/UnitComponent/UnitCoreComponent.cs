namespace UnitComponent
{
    public class UnitCoreComponent : UnitBaseComponent
    {
        public readonly UnitSkillComponent skill = null;
        public readonly UnitBuffComponent buff = null;
        public readonly UnitStatComponent stat = null;
        public readonly UnitTransformComponent transform = null;
        public readonly UnitDamageComponent damage = null;
        public readonly UnitDamagedComponent damaged = null;
        public readonly UnitMoveComponent move = null;
        public readonly UnitSkinComponent skin = null;
        public readonly UnitHealthComponent health = null;
        public readonly UnitAnimComponent anim = null;
        public readonly UnitTargetComponent target = null;
        public readonly UnitStatusComponent status = null;
        public readonly UnitItemComponent item = null;
        public readonly UnitFadeComponent fade = null;
        public readonly UnitAIComponent ai = null;
        public readonly UnitRefreshComponent refresh = null;
        public readonly UnitSpawnComponent spawn = null;
        public readonly UnitFoFComponent fof = null;
        public readonly UnitProfileComponent profile = null;
        public readonly UnitManaComponent mana = null;
        public readonly UnitDeadComponent dead = null;
        public readonly UnitKillComponent kill = null;
        public readonly UnitKilledComponent killed = null;
        public readonly UnitJumpComponent jump = null;
        public readonly UnitUIComponent ui = null;

        protected UnitCoreComponent(Unit owner) : base(owner)
        {
            this.skill = AddSkillComponent();
            this.buff = AddBuffComponent();
            this.stat = AddStatComponent();
            this.transform = AddTransformComponent();
            this.damage = AddDamageComponent();
            this.damaged = AddDamagedComponent();
            this.move = AddMoveComponent();
            this.skin = AddSkinComponent();
            this.health = AddHealthComponent();
            this.anim = AddAnimComponent();
            this.target = AddTargetComponent();
            this.status = AddStatusComponent();
            this.item = AddItemComponent();
            this.fade = AddFadeComponent();
            this.ai = AddAIComponent();
            this.refresh = AddRefreshComponent();
            this.spawn = AddSpawnComponent();
            this.fof = AddFoFComponent();
            this.profile = AddProfileComponent();
            this.mana = AddManaComponent();
            this.dead = AddDeadComponent();
            this.kill = AddKillComponent();
            this.killed = AddKilledComponent();
            this.jump = AddJumpComponent();
            this.ui = AddUIComponent();
        }

        public static UnitCoreComponent Of(Unit owner)
        {
            return new UnitCoreComponent(owner);
        }

        protected virtual UnitSkillComponent AddSkillComponent()
        {
            return AddComponent<UnitSkillComponent>(owner);
        }

        protected virtual UnitBuffComponent AddBuffComponent()
        {
            return AddComponent<UnitBuffComponent>(owner);
        }

        protected virtual UnitStatComponent AddStatComponent()
        {
            return AddComponent<UnitStatComponent>(owner);
        }

        protected virtual UnitTransformComponent AddTransformComponent()
        {
            return AddComponent<UnitTransformComponent>(owner);
        }

        protected virtual UnitDamageComponent AddDamageComponent()
        {
            return AddComponent<UnitDamageComponent>(owner);
        }

        protected virtual UnitDamagedComponent AddDamagedComponent()
        {
            return AddComponent<UnitDamagedComponent>(owner);
        }

        protected virtual UnitMoveComponent AddMoveComponent()
        {
            return AddComponent<UnitMoveComponent>(owner);
        }

        protected virtual UnitSkinComponent AddSkinComponent()
        {
            return AddComponent<UnitSkinComponent>(owner);
        }

        protected virtual UnitHealthComponent AddHealthComponent()
        {
            return AddComponent<UnitHealthComponent>(owner);
        }

        protected virtual UnitAnimComponent AddAnimComponent()
        {
            return AddComponent<UnitAnimComponent>(owner);
        }

        protected virtual UnitTargetComponent AddTargetComponent()
        {
            return AddComponent<UnitTargetComponent>(owner);
        }

        protected virtual UnitStatusComponent AddStatusComponent()
        {
            return AddComponent<UnitStatusComponent>(owner);
        }

        protected virtual UnitItemComponent AddItemComponent()
        {
            return AddComponent<UnitItemComponent>(owner);
        }

        protected virtual UnitFadeComponent AddFadeComponent()
        {
            return AddComponent<UnitFadeComponent>(owner);
        }

        protected virtual UnitAIComponent AddAIComponent()
        {
            return AddComponent<UnitAIComponent>(owner);
        }

        protected virtual UnitRefreshComponent AddRefreshComponent()
        {
            return AddComponent<UnitRefreshComponent>(owner);
        }

        protected virtual UnitSpawnComponent AddSpawnComponent()
        {
            return AddComponent<UnitSpawnComponent>(owner);
        }

        protected virtual UnitFoFComponent AddFoFComponent()
        {
            return AddComponent<UnitFoFComponent>(owner);
        }

        protected virtual UnitProfileComponent AddProfileComponent()
        {
            return AddComponent<UnitProfileComponent>(owner);
        }

        protected virtual UnitUIComponent AddUIComponent()
        {
            return AddComponent<UnitUIComponent>(owner);
        }

        protected virtual UnitDeadComponent AddDeadComponent()
        {
            return AddComponent<UnitDeadComponent>(owner);
        }

        protected virtual UnitKillComponent AddKillComponent()
        {
            return AddComponent<UnitKillComponent>(owner);
        }

        protected virtual UnitKilledComponent AddKilledComponent()
        {
            return AddComponent<UnitKilledComponent>(owner);
        }

        protected virtual UnitManaComponent AddManaComponent()
        {
            return AddComponent<UnitManaComponent>(owner);
        }

        protected virtual UnitJumpComponent AddJumpComponent()
        {
            return AddComponent<UnitJumpComponent>(owner);
        }
    }
}
