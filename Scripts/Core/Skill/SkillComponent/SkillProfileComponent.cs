namespace Skill
{
    public class SkillProfileComponent : SkillBaseComponent
    {
        public SkillInfo skillInfo { get; private set; } = null;
        public ResourceSkillScript resScript { get; private set; } = null;
        public float flowTime { get; private set; } = 0f;

        public SkillProfileComponent(SkillBase skill) : base(skill)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            skillInfo = null;
            resScript = null;
            flowTime = 0f;
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            flowTime += dt;
        }

        public virtual void Set(SkillInfo skillInfo)
        {
            this.skillInfo = skillInfo;
            this.resScript = skillInfo.resFire.script;
        }

        public override void OnDisable()
        {
            if (skillInfo != null)
            {
                skillInfo.OnDisable();
                skillInfo = null;
            }

            base.OnDisable();
        }
    }
}