using CodeStage.AntiCheat.ObscuredTypes;

public class TUnit : TBase
{
    public long uid { get; private set; } = 0;
    public int id => resUnit != null ? resUnit.id : 0;
    public ResourceUnit resUnit { get; private set; } = null;
    private ObscuredLong level = 0;

    public static TUnit Of()
    {
        return new TUnit();
    }

    private TUnit() : base()
    {

    }

    public override void DoReset()
    {
        base.DoReset();
        uid = 0;
        resUnit = null;
        level = 0;
    }

    public long GetLevel()
    {
        return level;
    }

    public TUnit Set(int id)
    {
        this.uid = CreateUID();
        this.resUnit = ResourceManager.Instance.unit.GetUnit(id);

        return this;
    }

    public TUnit SetLevel(long level)
    {
        this.level = level;
        return this;
    }

    private static long increaseUID = 0;
    private static long CreateUID()
    {
        if (increaseUID >= long.MaxValue)
        {
            increaseUID = 0;
        }

        return ++increaseUID;
    }
}