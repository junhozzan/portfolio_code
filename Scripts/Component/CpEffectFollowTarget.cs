using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpEffectFollowTarget : CpEffect
{
    private CpObject target = null;

    public override bool _useUpdate => true;

    public override void DoReset()
    {
        base.DoReset();

        target = null;
    }

    public void SetTarget(CpObject target)
    {
        this.target = target;
        SetPosition(target.GetPosition());
    }

    public override void UpdateDt(float dt)
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        if (target == null)
        {
            return;
        }

        SetPosition(target.GetPosition());
    }
}
