namespace Skill
{
    public class SkillCoreComponent : SkillBaseComponent
    {
        public readonly SkillProfileComponent profile = null;

        protected SkillCoreComponent(SkillBase skill) : base(skill)
        {
            this.profile = AddProfileComponent();
        }

        public static SkillCoreComponent Of(SkillBase skill)
        {
            return new SkillCoreComponent(skill);
        }

        protected virtual SkillProfileComponent AddProfileComponent()
        {
            return AddComponent<SkillProfileComponent>(skill);
        }
    }
}