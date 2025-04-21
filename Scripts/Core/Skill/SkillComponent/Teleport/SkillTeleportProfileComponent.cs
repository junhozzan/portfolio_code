namespace Skill
{
    public class SkillTeleportProfileComponent : SkillProfileComponent
    {
        public new ResourceSkillTeleport resScript = null;

        public SkillTeleportProfileComponent(SkillTeleport skill) : base(skill)
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
            this.resScript = base.resScript as ResourceSkillTeleport;
        }
    }
}