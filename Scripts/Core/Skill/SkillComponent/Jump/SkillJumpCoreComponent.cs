
namespace Skill
{
    public class SkillJumpCoreComponent : SkillCoreComponent
    {
        public readonly new SkillJumpProfileComponent profile = null;
        public readonly SkillJumpTriggerComponent trigger = null;
        public readonly SkillJumpHitComponent hit = null;
        public readonly SkillFinishComponent finish = null;

        public SkillJumpCoreComponent(SkillBase skill) : base(skill)
        {
            this.profile = base.profile as SkillJumpProfileComponent;
            this.trigger = AddComponent<SkillJumpTriggerComponent>(skill);
            this.hit = AddComponent<SkillJumpHitComponent>(skill);
            this.finish = AddComponent<SkillFinishComponent>(skill);
        }

        public static new SkillJumpCoreComponent Of(SkillBase skill)
        {
            return new SkillJumpCoreComponent(skill);
        }

        protected override SkillProfileComponent AddProfileComponent()
        {
            return AddComponent<SkillJumpProfileComponent>(skill);
        }
    }
}