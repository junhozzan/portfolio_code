namespace Skill
{
    public class SkillJumpTriggerComponent : SkillBaseComponent
    {
        private readonly new SkillJump skill = null;

        public SkillJumpTriggerComponent(SkillJump skill) : base(skill)
        {
            this.skill = skill;
        }

        public bool Exec()
        {
            var skillInfo = skill.core.profile.skillInfo;
            var target = skillInfo._target;
            var caster = skillInfo._from;
            if (!UnitRule.IsAlive(caster) || !UnitRule.IsValid(target))
            {
                skill.core.finish.Finish();
                return false;
            }

            var point = target.core.transform.GetPosition();
            var vec = (point - caster.core.transform.GetPosition()).normalized;
            
            var jumpPoint = point + vec * skill.core.profile.resScript.offsetDistance;
            caster.core.jump.SetJump(skill.core.profile.resScript.time, jumpPoint);

            return true;
        }
    }
}