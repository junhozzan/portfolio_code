
namespace Skill
{
    public class SkillRangeProfileComponent : SkillProfileComponent
    {
        public new ResourceSkillRange resScript = null;

        public SkillRangeProfileComponent(SkillBase skill) : base(skill)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            resScript = null;
        }

        public override void Set(SkillInfo skillInfo)
        {
            base.Set(skillInfo);
            this.resScript = base.resScript as ResourceSkillRange;
        }
    }
}