using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SkillInfo : SimplePoolItem
{
    private Unit from = null;
    private Unit target = null;
    private long savedTargetUID = 0;

    public Unit _from
    {
        get
        {
            return from;
        }
    }

    public Unit _target
    {
        get
        {
            if (target != null)
            {
                // 타겟 데이터 변화 감지
                if (!UnitRule.IsValid(target) || target.core.profile.tunit.uid != savedTargetUID)
                {
                    target = null;
                }
            }

            return target;
        }
    }

    public Vector3 shotPosition { get; private set; } = Vector3.zero;
    public Vector3 shotDir { get; private set; } = Vector3.up;
    public Vector3 hitPosition { get; private set; } = Vector3.zero;
    public ResourceSkill resSkill { get; private set; } = null;
    public ResourceSkillFire resFire { get; private set; } = null;

    public static SkillInfo Of()
    {
        return new SkillInfo();
    }

    public override void DoReset()
    {
        base.DoReset();

        from = null;
        target = null;
        savedTargetUID = 0;
        shotPosition = Vector3.zero;
        shotDir = Vector3.up;
        hitPosition = Vector3.zero;
        resSkill = null;
        resFire = null;
    }

    public void SetFrom(Unit unit)
    {
        this.from = unit;
    }

    public void SetResourceSkill(ResourceSkill resSkill)
    {
        this.resSkill = resSkill;
    }

    public void SetFire(ResourceSkillFire fire)
    {
        this.resFire = fire;
    }

    public void SetTarget(Unit target)
    {
        this.target = target;
        savedTargetUID = UnitRule.IsValid(target) ? target.core.profile.tunit.uid : 0;
    }

    public void SetHitPosition(Vector3 pos)
    {
        hitPosition = pos;
    }

    public void SetShotPosition(Vector3 pos)
    {
        shotPosition = pos;
    }

    public void SetShotDir(Vector3 dir)
    {
        shotDir = dir;
    }

    public float GetAbility(eAbility e)
    {
        return from.core.stat.GetValue(e);
    }

    public SkillInfo Copy(SkillInfo origin)
    {
        from = origin.from;
        target = origin.target;
        savedTargetUID = origin.savedTargetUID;
        resSkill = origin.resSkill;
        resFire = origin.resFire;
        hitPosition = origin.hitPosition;
        shotPosition = origin.shotPosition;
        shotDir = origin.shotDir;

        return this;
    }

    public override void OnDisable()
    {
        target = null;
        savedTargetUID = 0;
        base.OnDisable();
    }

#if USE_DEBUG
    private const bool _DEBUG = true;
#else
    private const bool _DEBUG = false;
#endif
}

