
namespace Skill
{
    public class SkillRangeTimeOutComponent : SkillBaseComponent
    {
        private readonly new SkillRange skill = null;

        public SkillRangeTimeOutComponent(SkillRange skill) : base(skill)
        {
            this.skill = skill;
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);

            if (skill.core.profile.flowTime >= skill.core.profile.resScript.time)
            {
                skill.core.finish.Finish();
            }
        }
    }
}