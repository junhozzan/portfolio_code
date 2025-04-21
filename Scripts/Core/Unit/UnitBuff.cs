using System.Collections.Generic;


public class UnitBuff : SimplePoolItem
{
    public TBuff tbuff { get; private set; } = null;
    public Unit from { get; private set; } = null;
    public Unit owner { get; private set; } = null;

    private CpEffectFollowTarget effect = null;

    private readonly List<StatItem.Param> statItemParams = new List<StatItem.Param>();
    private readonly Dictionary<BuffScriptType, List<BuffScriptBase>> buffScripts = new Dictionary<BuffScriptType, List<BuffScriptBase>>();

    private static readonly List<BuffScriptBase> emptyScripts = new List<BuffScriptBase>();

    public static UnitBuff Of()
    {
        return new UnitBuff();
    }

    public override void DoReset()
    {
        base.DoReset();

        tbuff = null;
        from = null;
        owner = null;
        statItemParams.Clear();

        ClearBuffScript();
        ShowEffect(false);
    }

    private void ClearBuffScript()
    {
        foreach (var scripts in buffScripts.Values)
        {
            foreach (var script in scripts)
            {
                script.OnDisable();
            }

            scripts.Clear();
        }
    }

    public void UpdateTBuff(TBuff _tbuff)
    {
        if (tbuff != null)
        {
            tbuff.OnDisable();
        }

        tbuff = _tbuff;

        RefreshBuffScript();
    }

    private void RefreshBuffScript()
    {
        foreach (var kv in buffScripts)
        {
            var list = kv.Value;
            foreach (var buffScript in kv.Value)
            {
                buffScript.OnDisable();
            }

            list.Clear();
        }

        var resBuff = tbuff.resBuff;
        if (resBuff == null)
        {
            return;
        }

        foreach (var resBuffScript in resBuff.resBuffScripts)
        {
            var buffScript = BuffScriptManager.Instance.Get(resBuffScript);
            buffScript.SetBuff(this);

            if (!buffScripts.TryGetValue(resBuffScript.type, out var list))
            {
                buffScripts.Add(resBuffScript.type, list = new List<BuffScriptBase>());
            }

            list.Add(buffScript);
        }
    }

    public void On(Unit from, Unit to)
    {
        this.from = from;
        this.owner = to;

        foreach (var scripts in buffScripts.Values)
        {
            foreach (var script in scripts)
            {
                script.On();
            }
        }

        RefreshStatItems();
        ShowEffect(true);
    }

    public void Off()
    {
        foreach (var scripts in buffScripts.Values)
        {
            foreach (var script in scripts)
            {
                script.Off();
            }
        }

        OnDisable();
    }

    public void UpdateDt(float dt)
    {
        foreach (var scripts in buffScripts.Values)
        {
            foreach (var script in scripts)
            {
                script.UpdateDt(dt);
            }
        }
    }

    public bool IsBuffTime(long nowEpochSecond)
    {
        if (tbuff.untilAt < 0)
        {
            return true;
        }

        return tbuff.untilAt >= nowEpochSecond;
    }

    private void RefreshStatItems()
    {
        statItemParams.Clear();
        var buffLevel = tbuff.GetLevel();
        var riseParam = StatItem.Param.RiseParam.Of(buffLevel, 0);
        foreach (var scripts in buffScripts.Values)
        {
            foreach (var script in scripts)
            {
                statItemParams.AddRange(StatItem.GetParams(from, owner, script.GetTargetAbilities(), riseParam, null));
            }
        }
    }

    private void ShowEffect(bool show)
    {
        if (show)
        {
            if (effect != null)
            {
                return;
            }

            var resBuff = tbuff.resBuff;
            if (resBuff == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(resBuff.effect.prefab))
            {
                return;
            }

            IEffectPointer pointer = owner.core.transform;
            var point = pointer.GetPoint(resBuff.effect.pointType);
            if (!point.HasValue)
            {
                return;
            }

            effect = ObjectManager.Instance.Pop<CpEffectFollowTarget>(resBuff.effect.prefab);
            effect.SetPosition(point.Value);
            effect.SetTarget(owner.core.transform.unitObj);
            effect.SetLayer(resBuff.effect.layer);

            if (!resBuff.effect.IsInfinity())
            {
                effect.DelayInactive(resBuff.effect.time);
            }
        }
        else
        {
            if (effect == null)
            {
                return;
            }

            effect.SetActive(false);
            effect = null;
        }
    }

    public List<StatItem.Param> GetStatItemParams()
    {
        return statItemParams;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (tbuff != null)
        {
            tbuff.OnDisable();
            tbuff = null;
        }

        ClearBuffScript();
        ShowEffect(false);
    }

    public List<BuffScriptBase> GetBuffScripts(BuffScriptType type)
    {
        if (!buffScripts.TryGetValue(type, out var list))
        {
            return emptyScripts;
        }

        return list;
    }

    public long GetUntilAt()
    {
        return tbuff.untilAt;
    }

    public float GetAddDamageRatio(DamageType damageType, Unit to)
    {
        var value = 0f;
        foreach (var scripts in buffScripts.Values)
        {
            foreach (var script in scripts)
            {
                value += script.GetAddDamageRatio(damageType, to);
            }
        }

        return value;
    }

    public void HandleUseSkill(Unit to)
    {
        foreach (var scripts in buffScripts.Values)
        {
            foreach (var script in scripts)
            {
                script.HandleUseSkill(to);
            }
        }
    }

    public void Attack(ResourceSkillAttack resAttack, Unit to, float damage)
    {
        foreach (var scripts in buffScripts.Values)
        {
            foreach (var script in scripts)
            {
                script.Attack(resAttack, to, damage);
            }
        }
    }

    public void Attacked(ResourceSkillAttack resAttack, Unit from, float damage)
    {
        foreach (var scripts in buffScripts.Values)
        {
            foreach (var script in scripts)
            {
                script.Attacked(resAttack, from, damage);
            }
        }
    }

    public void Kill(Unit to)
    {
        foreach (var scripts in buffScripts.Values)
        {
            foreach (var script in scripts)
            {
                script.Kill(to);
            }
        }
    }

    public void Killed(Unit from)
    {
        foreach (var scripts in buffScripts.Values)
        {
            foreach (var script in scripts)
            {
                script.Killed(from);
            }
        }
    }
}