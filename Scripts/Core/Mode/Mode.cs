using ModeComponent;

public abstract partial class Mode
{
    public readonly ModeCoreComponent core = null;
    public readonly ResourceMode resMode = null;

    protected Mode(ResourceMode _resMode)
    {
        resMode = _resMode;
        core = CreateCoreComponent();
    }

    protected virtual ModeCoreComponent CreateCoreComponent()
    {
        return ModeCoreComponent.Of(this);
    }

    public void DoReset()
    {
        core.DoReset();
    }

    public void Initialize()
    {
        core.Initialize();
    }

    public void OnEnable()
    {
        core.OnEnable();
    }

    public void OnDisable()
    {
        core.OnDisable();
    }

    public void UpdateDt(float dt)
    {
        core.UpdateDt(dt);
    }

    public bool IsLoading()
    {
        if (core.field.IsLoading())
        {
            return true;
        }

        if (core.ally.IsLoading())
        {
            return true;
        }

        return false;
    }

#if USE_DEBUG
    protected const bool _DEBUG = true;
#else
    protected const bool _DEBUG = false;
#endif
}