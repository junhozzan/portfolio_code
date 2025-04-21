using UnityEngine;

public class UnitSkill : SimplePoolItem
{
    private TSkill tskill = null;

    public Unit owner { get; private set; } = null;
    public ResourceSkill resSkill { get; private set; } = null;

    public static UnitSkill Of()
    {
        return new UnitSkill();
    }

    public override void DoReset()
    {
        base.DoReset();

        owner = null;
        tskill = null;
    }

    public void SetOwner(Unit owner)
    {
        this.owner = owner;
    }

    public void UpdateTSkill(TSkill _tskill)
    {
        if (tskill != null)
        {
            tskill.OnDisable();
        }

        this.tskill = _tskill;
        this.resSkill = ResourceManager.Instance.skill.GetSkill(_tskill.resID);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (tskill != null)
        {
            tskill.OnDisable();
            tskill = null;
        }
    }

    public bool IsTargetInSkillRange(Unit target)
    {
        if (SkillRule.IsTargetInSkillRange(owner, target, resSkill))
        {
            return true;
        }

        return false;
    }

    public float GetMaxCoolTime()
    {
        return tskill.GetMaxCoolTime();
    }
}