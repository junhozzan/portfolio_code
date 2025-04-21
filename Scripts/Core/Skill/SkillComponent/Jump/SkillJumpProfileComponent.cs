namespace Skill
{
    public class SkillJumpProfileComponent : SkillProfileComponent
    {
        public new ResourceSkillJump resScript = null;

        public SkillJumpProfileComponent(SkillJump skill) : base(skill)
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
            this.resScript = base.resScript as ResourceSkillJump;
        }
    }
}