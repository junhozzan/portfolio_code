using System;
using System.Collections.Generic;

public class UnitRule
{
    public static bool IsAlive(Unit unit)
    {
        return IsValid(unit) && !unit.core.dead.isDead;
    }

    public static bool IsValid(Unit unit)
    {
        return unit != null && unit.core.profile.tunit != null && unit.IsUsed();
    }

    public static bool IsTargetable(Unit from, Unit to, IReadOnlyCollection<ResourceApplyTarget> applyTargets)
    {
        if (applyTargets == null || applyTargets.Count == 0)
        {
            return true;
        }

        if (!IsValid(to))
        {
            return false;
        }

        var isTargetable = false;
        foreach (var applyTarget in applyTargets)
        {
            if (!applyTarget.IsTarget(from, to))
            {
                continue;
            }

            isTargetable = true;
            break;
        }

        return isTargetable;
    }
}
