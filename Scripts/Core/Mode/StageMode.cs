using ModeComponent;

public partial class StageMode : Mode
{
    public readonly new StageModeCoreComponent core = null;

    public static StageMode Of(ResourceMode resMode)
    {
        return new StageMode(resMode);
    }

    private StageMode(ResourceMode resMode) : base(resMode)
    {
        this.core = base.core as StageModeCoreComponent;
    }

    protected override ModeCoreComponent CreateCoreComponent()
    {
        return StageModeCoreComponent.Of(this);
    }
}