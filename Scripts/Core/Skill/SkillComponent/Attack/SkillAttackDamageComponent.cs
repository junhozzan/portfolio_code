using UnityEngine;

namespace Skill
{
    public class SkillAttackDamageComponent : SkillBaseComponent
    {
        private readonly new SkillAttack skill = null;

        public SkillAttackDamageComponent(SkillAttack skill) : base(skill)
        {
            this.skill = skill;
        }

        public long GetDamage(Unit caster, Unit target, bool isCritical)
        {
            var res = skill.core.profile.resScript;
            var skillInfo = skill.core.profile.skillInfo;
            if (res.damagesBys.TryGetValue(DamageType.NORMAL, out var by))
            {
                if (by.CheckActivateBuff(caster.core.buff.GetBuffIDs()))
                {
                    switch (by.byType)
                    {
                        case DamageByType.HP:
                            return (long)(target.core.health.hp * by.ratio);
                        case DamageByType.MAXHP:
                            return (long)(target.core.health.maxHp * by.ratio);
                    }
                }
                return 0L;
            }

            if (!res.damages.TryGetValue(DamageType.NORMAL, out var d))
            {
                return 0L;
            }

            if (!d.CheckActivateBuff(caster.core.buff.GetBuffIDs()))
            {
                return 0L;
            }

            var power = SkillRule.GetBasePower(skillInfo, target) * d.ratio;
            var damage = SkillRule.GetDamage(DamageType.NORMAL, skillInfo, target, power);

            if (isCritical)
            {
                // 크리티컬 대미지 추가
                damage += SkillRule.GetAddCriticalDamage(damage, skillInfo, target);
            }

            // 피해 증가 적용
            damage += SkillRule.GetAddIncreaseDamage(damage, skillInfo, target);

            // 피해 감소 적용
            damage += SkillRule.GetAddDecreaseDamage(damage, skillInfo, target);

            // 고정대미지
            damage += SkillRule.GetAddFixedDamage(skillInfo, target);

            return (long)Mathf.Max(0f, damage);
        }

        public long GetHolyDamange(Unit caster, Unit target)
        {
            var res = skill.core.profile.resScript;
            var skillInfo = skill.core.profile.skillInfo;
            if (!res.damages.TryGetValue(DamageType.HOLY, out var d))
            {
                return 0L;
            }

            if (!d.CheckActivateBuff(caster.core.buff.GetBuffIDs()))
            {
                return 0L;
            }

            var power = SkillRule.GetBaseHolyPower(skillInfo, target) * d.ratio;
            var damage = SkillRule.GetDamage(DamageType.HOLY, skillInfo, target, power);

            return (long)damage;
        }

        public long GetDarkDamage(Unit caster, Unit target)
        {
            var res = skill.core.profile.resScript;
            var skillInfo = skill.core.profile.skillInfo;
            if (!res.damages.TryGetValue(DamageType.DARK, out var d))
            {
                return 0L;
            }

            if (!d.CheckActivateBuff(caster.core.buff.GetBuffIDs()))
            {
                return 0L;
            }

            var power = SkillRule.GetBaseDarkPower(skillInfo, target) * d.ratio;
            var damage = SkillRule.GetDamage(DamageType.DARK, skillInfo, target, power);

            return (long)damage;
        }
    }
}