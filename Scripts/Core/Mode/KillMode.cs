using ModeComponent;

public partial class KillMode : Mode
{
    public readonly new KillModeCoreComponent core = null;

    public static KillMode Of(ResourceMode resMode)
    {
        return new KillMode(resMode);
    }

    private KillMode(ResourceMode resMode) : base(resMode)
    {
        this.core = base.core as KillModeCoreComponent;
    }

    protected override ModeCoreComponent CreateCoreComponent()
    {
        return KillModeCoreComponent.Of(this);
    }
}
