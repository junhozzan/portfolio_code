namespace Skill
{
    public class SkillProjectileCoreComponent : SkillCoreComponent
    {
        public readonly new SkillProjectileProfileComponent profile = null;
        public readonly SkillProjectileTargetComponent target = null;
        public readonly SkillProjectileObjectComponent obj = null;
        public readonly SkillProjectileMoveComponent move = null;
        public readonly SkillProjectileTriggerComponent trigger = null;
        public readonly SkillProjectileHitComponent hit = null;
        public readonly SkillFinishComponent finish = null;

        protected SkillProjectileCoreComponent(SkillProjectile skill) : base(skill)
        {
            this.profile = base.profile as SkillProjectileProfileComponent;
            this.target = AddComponent<SkillProjectileTargetComponent>(skill);
            this.obj = AddComponent<SkillProjectileObjectComponent>(skill);
            this.move = AddComponent<SkillProjectileMoveComponent>(skill);
            this.trigger = AddComponent<SkillProjectileTriggerComponent>(skill);
            this.hit = AddComponent<SkillProjectileHitComponent>(skill);
            this.finish = AddComponent<SkillFinishComponent>(skill);
        }

        public static SkillProjectileCoreComponent Of(SkillProjectile skill)
        {
            return new SkillProjectileCoreComponent(skill);
        }

        protected override SkillProfileComponent AddProfileComponent()
        {
            return AddComponent<SkillProjectileProfileComponent>(skill);
        }
    }
}