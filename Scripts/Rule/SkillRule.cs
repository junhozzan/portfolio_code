using System.Collections.Generic;
using UnityEngine;
using System;

public class SkillRule
{
    public static float GetBasePower(SkillInfo skillInfo, Unit to)
    {
        var power = skillInfo.GetAbility(eAbility.POWER);
        if (power <= 0f)
        {
            return 0f;
        }

        return Mathf.Max(power - to.core.stat.GetValue(eAbility.DEFENCE), 1f)
            * Mathf.Max(1f - to.core.stat.GetValue(eAbility.RESIST_NONE), 0f);
    }

    public static float GetBaseHolyPower(SkillInfo skillInfo, Unit to)
    {
        return skillInfo.GetAbility(eAbility.POWER_HOLY) 
            * Mathf.Max(1f - to.core.stat.GetValue(eAbility.RESIST_HOLY), 0f);
    }

    public static float GetBaseDarkPower(SkillInfo skillInfo, Unit to)
    {
        return skillInfo.GetAbility(eAbility.POWER_DARK) 
            * Mathf.Max(1f - to.core.stat.GetValue(eAbility.RESIST_DARK), 0f);
    }

    public static float GetDamage(DamageType damageType, SkillInfo skillInfo, Unit to, float basePower)
    {
        var power = basePower;
        if (power == 0f)
        {
            return 0f;
        }

        power += basePower * skillInfo._from.core.buff.GetAddDamageRatioByBuff(damageType, to);
        power += basePower * UnityEngine.Random.Range(-0.05f, 0.05f); // 난수(공격력의 +-5 %)

        return Math.Max(power, 1f);
    }

    public static bool IsCriticalHit(SkillInfo skillInfo, Unit to)
    {
        return Util.IsChance(skillInfo.GetAbility(eAbility.CRITICAL_CHANCE) - to.core.stat.GetValue(eAbility.CRITICAL_RESIST));
    }

    public static float GetAddCriticalDamage(float damage, SkillInfo skillInfo, Unit to)
    {
        return Mathf.Max(0f, damage * skillInfo.GetAbility(eAbility.CRITICAL_DAMAGE));
    }

    public static float GetAddIncreaseDamage(float damage, SkillInfo skillInfo, Unit to)
    {
        return Mathf.Max(0f, damage * skillInfo.GetAbility(eAbility.DAMAGE_INC));
    }

    public static float GetAddDecreaseDamage(float damage, SkillInfo skillInfo, Unit to)
    {
        return -Mathf.Min(0f, damage * to.core.stat.GetValue(eAbility.DAMAGED_DEC));
    }

    public static float GetAddFixedDamage(SkillInfo skillInfo, Unit to)
    {
        return skillInfo.GetAbility(eAbility.DAMAGE_FIXED);
    }

    public static bool IsPenetrate(SkillInfo skillInfo, Unit to)
    {
        //return Util.IsChance(skillInfo.GetAbility(eAbility.PENETRATE_CHANCE));
        return true;
    }

    public static bool IsSkillPlayable(SkillInfo skillInfo)
    {
        var fire = skillInfo.resFire;
        if (fire == null)
        {
            return false;
        }

        if (!UnitRule.IsTargetable(skillInfo._from, skillInfo._target, fire.applyTargets))
        {
            return false;
        }

        return true;
    }

    public static bool IsTargetInSkillRange(Unit from, Unit to, ResourceSkill resSkill)
    {
        if (from == null || to == null || resSkill == null)
        {
            return false;
        }

        if (!InRange(from, to, from.core.skill.GetSkillRange(resSkill)))
        {
            return false;
        }

        return true;
    }

    public static bool InRange(Unit from, Unit to, float skillRange)
    {
        var myPos = from.core.transform.GetPosition();
        var targetPos = to.core.transform.GetPosition();
        var sqrDistance = (targetPos - myPos).sqrMagnitude;
        var sqrRange = Math.Pow(skillRange, 2);

        return sqrDistance <= sqrRange;
    }

    public static Unit GetNearTarget(List<Unit> targets, Vector3 pos)
    {
        Unit result = null;

        var dist = float.MaxValue;
        foreach (var target in targets)
        {
            // 가장 가까운 거리 내의 타겟 체크
            var compareDist = (pos - target.core.transform.GetPosition()).sqrMagnitude;
            if (dist <= compareDist)
            {
                continue;
            }

            result = target;
            dist = compareDist;
        }

        return result;
    }

    public static Unit GetFarTarget(List<Unit> targets, Vector3 pos)
    {
        Unit result = null;
        var dist = 0f;
        foreach (var target in targets)
        {
            // 가장 가까운 거리 내의 타겟 체크
            var compareDist = (pos - target.core.transform.GetPosition()).sqrMagnitude;
            if (dist >= compareDist)
            {
                continue;
            }

            result = target;
            dist = compareDist;
        }

        return result;
    }

    public static Unit GetRandomTarget(List<Unit> targets)
    {
        if (targets.Count == 0)
        {
            return null;
        }

        return targets[UnityEngine.Random.Range(0, targets.Count)];
    }
}
