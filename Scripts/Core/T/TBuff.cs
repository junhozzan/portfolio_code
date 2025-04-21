using CodeStage.AntiCheat.ObscuredTypes;
using System;

public class TBuff : TBase
{
    public int id => resBuff != null ? resBuff.id : 0;
    public ResourceBuff resBuff { get; private set; } = null;
    public long fromUnitUID { get; private set; } = 0;
    public Type fromResType { get; private set; } = null;
    public int fromResID { get; private set; } = 0;
    public long startAt { get; private set; } = 0;
    public long untilAt { get; private set; } = 0;

    private ObscuredLong level = 0;

    public static TBuff Of()
    {
        return new TBuff();
    }

    private TBuff() : base()
    {

    }

    public override void DoReset()
    {
        base.DoReset();
        resBuff = null;
        level = 0;
        fromUnitUID = 0;
        fromResType = null;
        fromResID = 0;
        startAt = 0;
        untilAt = 0;
    }

    public long GetLevel()
    {
        return level;
    }

    public TBuff Set(int resID)
    {
        this.resBuff = ResourceManager.Instance.buff.GetBuff(resID);
        return this;
    }

    public TBuff SetLevel(long level)
    {
        this.level = level;
        return this;
    }

    public TBuff SetFromUnit(long uid)
    {
        this.fromUnitUID = uid;
        return this;
    }

    public TBuff SetFromRes(ResourceBase res)
    {
        this.fromResType = res.GetType();
        this.fromResID = res.id;
        return this;
    }

    public TBuff SetUntilAt(long now, bool isInfinity, long duration)
    {
        this.startAt = now;
        this.untilAt = isInfinity ?  -1 : startAt + duration;

        return this;
    }
}
