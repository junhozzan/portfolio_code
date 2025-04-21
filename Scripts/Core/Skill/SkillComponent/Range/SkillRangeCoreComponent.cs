namespace Skill
{
    public class SkillRangeCoreComponent : SkillCoreComponent
    {
        public readonly new SkillRangeProfileComponent profile = null;
        public readonly SkillRangeTargetComponent target = null;
        public readonly SkillRangeObjectComponent obj = null;
        public readonly SkillRangeTriggerComponent trigger = null;
        public readonly SkillRangeHitComponent hit = null;
        public readonly SkillRangeTimeOutComponent timeOut = null;
        public readonly SkillFinishComponent finish = null;

        protected SkillRangeCoreComponent(SkillRange skill) : base(skill)
        {
            this.profile = base.profile as SkillRangeProfileComponent;
            this.target = AddComponent<SkillRangeTargetComponent>(skill);
            this.obj = AddComponent<SkillRangeObjectComponent>(skill);
            this.trigger = AddComponent<SkillRangeTriggerComponent>(skill);
            this.hit = AddComponent<SkillRangeHitComponent>(skill);
            this.timeOut = AddComponent<SkillRangeTimeOutComponent>(skill);
            this.finish = AddComponent<SkillFinishComponent>(skill);
        }

        public static SkillRangeCoreComponent Of(SkillRange skill)
        {
            return new SkillRangeCoreComponent(skill);
        }

        protected override SkillProfileComponent AddProfileComponent()
        {
            return AddComponent<SkillRangeProfileComponent>(skill);
        }
    }
}