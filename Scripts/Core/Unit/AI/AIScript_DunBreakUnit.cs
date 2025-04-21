using UnityEngine;
using System.Collections.Generic;

public class AIScript_DunBreakUnit : AIScript_Enemy
{
    public AIScript_DunBreakUnit(Unit unit) : base(unit)
    {

    }

    private bool IsTargetInRange(Unit target)
    {
        //if (IsTargetInNearRange(target))
        //{
        //    return true;
        //}

        //if (IsInView())
        //{
        //    return true;
        //}

        return true;
    }

    private bool IsTargetInNearRange(Unit target)
    {
        var mainPos = owner.core.transform.GetPosition();
        var targetPos = target.core.transform.GetPosition();
        var sqrDistance = (targetPos - mainPos).sqrMagnitude;
        var sqrRange = Mathf.Pow(Env.Distance(200f), 2);

        return sqrDistance <= sqrRange;
    }

    private bool IsInView()
    {
        var mode = ModeManager.Instance.mode;
        if (mode == null)
        {
            return true;
        }

        var targetPos = owner.core.transform.GetPosition();
        return mode.core.camera.IsInView(targetPos, Vector2.zero);
    }

    protected override List<Unit> GetTargetsFromSkill(UnitSkill skill)
    {
        return UnitManager.Instance.FindTargets(owner, skill.resSkill.fire.applyTargets, owner.core.target.GetIgnoreTargetUIDs(), IsTargetInRange);
    }
}
