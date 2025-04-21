using CodeStage.AntiCheat.ObscuredTypes;

public class TLab : TBase
{
    public int resID { get; private set; } = 0;
    private ObscuredLong level = 0;

    public static TLab Of()
    {
        return new TLab();
    }

    private TLab() : base()
    {

    }

    public long GetLevel()
    {
        return level;
    }

    public TLab SetData(UserLab.LabData data)
    {
        this.resID = data.id;
        this.level = data.level;

        return this;
    }
}

