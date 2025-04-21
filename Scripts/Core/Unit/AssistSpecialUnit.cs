using UnitComponent;

public class AssistSpecialUnit : AssistUnit
{
    public static new AssistSpecialUnit Of()
    {
        return new AssistSpecialUnit();
    }

    protected AssistSpecialUnit() : base()
    {

    }

    protected override UnitCoreComponent CreateCoreComponent()
    {
        return ShadowSpecialUnitCoreComponent.Of(this);
    }
}
