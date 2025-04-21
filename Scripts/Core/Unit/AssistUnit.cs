using UnitComponent;

public class AssistUnit : Unit
{
    public readonly new AssistUnitCoreComponent core = null;

    public static AssistUnit Of()
    {
        return new AssistUnit();
    }

    protected AssistUnit() : base()
    {
        this.core = base.core as AssistUnitCoreComponent;
    }

    protected override UnitCoreComponent CreateCoreComponent()
    {
        return AssistUnitCoreComponent.Of(this);
    }
}
