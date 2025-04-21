using CodeStage.AntiCheat.ObscuredTypes;

public class TSkill : TBase
{
    public int resID { get; private set; } = 0;
    public long level { get; private set; } = 0;
    private ObscuredFloat maxCoolTime = float.MaxValue;

    public static TSkill Of()
    {
        return new TSkill();
    }

    private TSkill() : base()
    {

    }

    public override void DoReset()
    {
        base.DoReset();
        resID = 0;
        maxCoolTime = float.MaxValue;
    }

    public float GetMaxCoolTime()
    {
        return maxCoolTime;
    }

    public TSkill Set(int resID)
    {
        this.resID = resID;
        return this;
    }

    public TSkill SetLevel(long level)
    {
        this.level = level;
        return this;
    }

    public TSkill SetMaxCoolTime(float coolTime)
    {
        this.maxCoolTime = coolTime;
        return this;
    }
}
