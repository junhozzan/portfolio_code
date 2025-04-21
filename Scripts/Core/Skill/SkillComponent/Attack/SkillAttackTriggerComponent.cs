namespace Skill
{
    public class SkillAttackTriggerComponent : SkillBaseComponent
    {
        private readonly new SkillAttack skill = null;

        public SkillAttackTriggerComponent(SkillAttack skill) : base(skill)
        {
            this.skill = skill;
        }

        public bool Exec()
        {
            var skillInfo = skill.core.profile.skillInfo;
            var target = skillInfo._target;

            if (!UnitRule.IsValid(target))
            {
                skill.core.finish.Finish();
                return false;
            }

            var res = skill.core.profile.resScript;
            var caster = skillInfo._from;
            var isCritical = SkillRule.IsCriticalHit(skillInfo, target);
            var damage = skill.core.damage.GetDamage(caster, target, isCritical);
            var holyDamage = skill.core.damage.GetHolyDamange(caster, target);
            var darkDamage = skill.core.damage.GetDarkDamage(caster, target);
            target.core.damaged.TakeDamage(true, caster.core.profile.tunit.uid, damage, holyDamage, darkDamage, isCritical, res);

            var targetCenter = target.core.transform.GetCenterPosition();
            PlayTail(target, targetCenter);

            skill.core.finish.Finish();
            return true;
        }
    }
}