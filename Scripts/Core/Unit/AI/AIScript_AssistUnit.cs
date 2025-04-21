using behaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class AIScript_AssistUnit : AIScript
{
    private readonly new AssistUnit owner = null;
    public AIScript_AssistUnit(Unit unit) : base(unit) 
    {
        this.owner = unit as AssistUnit;
    }

    private bool IsTargetInRange(Unit target)
    {
        if (owner == null)
        {
            return true;
        }

        var mainUnit = owner.core.profile.summoner;
        if (!UnitRule.IsValid(mainUnit))
        {
            return true;
        }

        var mainPos = mainUnit.core.transform.GetPosition();
        var targetPos = target.core.transform.GetPosition();
        var sqrDistance = (targetPos - mainPos).sqrMagnitude;
        var sqrRange = Mathf.Pow(Env.Distance(400f), 2);

        return sqrDistance <= sqrRange;
    }

    protected override List<Unit> GetTargetsFromSkill(UnitSkill skill)
    {
        return UnitManager.Instance.FindTargets(base.owner, skill.resSkill.fire.applyTargets, base.owner.core.target.GetIgnoreTargetUIDs(), IsTargetInRange);
    }

    public override Node CreateAI()
    {
        return
            new Selector(
                new Sequence(
                    new AIResultAction(Pause),
                    new AIResultAction(StopMove)
                    ),
                new Sequence(
                    new AIResultAction(IsAlive),
                    new AIResultAction(IsActivity),
                    new AIAction(RemoveSkill),
                    new AIAction(RegistSkill),

                    new Selector(
                        new AIResultAction(IsAttackMotion),
                        UseSkillNodes(),
                        DefaultMove()
                        )
                    )
                );
    }
}
