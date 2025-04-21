using UnityEngine;
using System.Collections.Generic;
using BuffScript;

public class BuffScriptBase : SimplePoolItem
{
    protected BuffScriptType type = BuffScriptType.NONE;
    protected UnitBuff buff = null;

    public override void DoReset()
    {
        base.DoReset();

        type = BuffScriptType.NONE;
        buff = null;
    }

    public virtual void SetResource(ResourceBuffScriptBase resBuffScript)
    {
        this.type = resBuffScript.type;
    }

    public void SetBuff(UnitBuff buff)
    {
        this.buff = buff;
    }

    public virtual void On()
    {

    }

    public virtual void Off()
    {

    }

    public virtual void UpdateDt(float dt)
    {

    }

    public virtual void HandleUseSkill(Unit to)
    {

    }

    public virtual void Attack(ResourceSkillAttack resAttack, Unit to, float damage)
    {

    }

    public virtual void Attacked(ResourceSkillAttack resAttack, Unit from, float damage)
    {

    }

    public virtual void Kill(Unit to)
    {

    }

    public virtual void Killed(Unit from)
    {

    }

    public virtual float GetAddDamageRatio(DamageType damageType, Unit to)
    {
        return 0f;
    }

    public virtual ICollection<ResourceTargetAbility> GetTargetAbilities()
    {
        return null;
    }

    public UnitBuff GetBuff()
    {
        return buff;
    }

    public BuffScriptType GetScriptType()
    {
        return type;
    }
}
