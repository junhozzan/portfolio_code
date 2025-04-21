namespace Skill
{
    public class SkillAttackCoreComponent : SkillCoreComponent
    {
        public readonly new SkillAttackProfileComponent profile = null;
        public readonly SkillAttackDamageComponent damage = null;
        public readonly SkillAttackTriggerComponent trigger = null;
        public readonly SkillFinishComponent finish = null;

        protected SkillAttackCoreComponent(SkillAttack skill) : base(skill)
        {
            this.profile = base.profile as SkillAttackProfileComponent;
            this.damage = AddComponent<SkillAttackDamageComponent>(skill);
            this.trigger = AddComponent<SkillAttackTriggerComponent>(skill);
            this.finish = AddComponent<SkillFinishComponent>(skill);
        }

        public static SkillAttackCoreComponent Of(SkillAttack skill)
        {
            return new SkillAttackCoreComponent(skill);
        }

        protected override SkillProfileComponent AddProfileComponent()
        {
            return AddComponent<SkillAttackProfileComponent>(skill);
        }
    }
}