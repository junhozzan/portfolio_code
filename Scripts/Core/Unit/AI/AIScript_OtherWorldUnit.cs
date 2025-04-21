using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using behaviorTree;

public class AIScript_OtherWorldUnit : AIScript
{
    public AIScript_OtherWorldUnit(Unit unit) : base(unit)
    {

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
