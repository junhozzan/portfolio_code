using Skill;

public class SkillBase : SimplePoolItem
{
    public readonly SkillCoreComponent core = null;

    protected virtual SkillCoreComponent CreateCoreComponent()
    {
        return SkillCoreComponent.Of(this);
    }

    public SkillBase()
    {
        core = CreateCoreComponent();
    }

    public override void DoReset()
    {
        base.DoReset();     
        core.DoReset();
    }

    public virtual bool Play(SkillInfo skillInfo)
    {
        core.profile.Set(skillInfo);
        return true;
    }

    public override void Initialize()
    {
        base.Initialize();
        core.Initialize();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        core.OnEnable();
    }

    public override void OnDisable()
    {
        core.OnDisable();
        base.OnDisable();
    }

    public void UpdateNow()
    {
        UpdateDt(0f);
    }

    public virtual void UpdateDt(float dt)
    {
        core.UpdateDt(dt);
    }
}