using UnitComponent;

public class OtherWorldUnit : EnemyUnit
{
    public static new OtherWorldUnit Of()
    {
        return new OtherWorldUnit();
    }

    private OtherWorldUnit() : base()
    {

    }

    protected override UnitCoreComponent CreateCoreComponent()
    {
        return OtherWorldUnitCoreComponent.Of(this);
    }
}
