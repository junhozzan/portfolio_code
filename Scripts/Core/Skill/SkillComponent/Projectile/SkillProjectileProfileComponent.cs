namespace Skill
{
    public class SkillProjectileProfileComponent : SkillProfileComponent
    {
        public new ResourceSkillProjectile resScript = null;

        public SkillProjectileProfileComponent(SkillProjectile skill) : base(skill)
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
            this.resScript = base.resScript as ResourceSkillProjectile;
        }
    }
}