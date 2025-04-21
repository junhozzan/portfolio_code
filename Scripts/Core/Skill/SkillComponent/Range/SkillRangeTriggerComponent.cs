
namespace Skill
{
    public class SkillRangeTriggerComponent : SkillBaseComponent
    {
        private readonly new SkillRange skill = null;

        public SkillRangeTriggerComponent(SkillRange skill) : base(skill)
        {
            this.skill = skill;
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);

            var targets = skill.core.target.GetTargets();
            if (targets.Count == 0)
            {
                return;
            }

            foreach (var target in targets)
            {
                if (!skill.core.obj.IsTrigger(target))
                {
                    continue;
                }

                skill.core.hit.HitTarget(target);
                if (skill.core.hit.IsMaxHitTarget())
                {
                    break;
                }
            }
        }
    }
}