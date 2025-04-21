using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeStage.AntiCheat.ObscuredTypes;

public class TMission : TBase
{
    public int resID { get; private set; } = 0;
    private ObscuredLong level = 0;
    private ObscuredLong value = 0;

    public static TMission Of()
    {
        return new TMission();
    }

    private TMission() : base()
    {

    }

    public override void DoReset()
    {
        base.DoReset();
        resID = 0;
        value = 0;
        level = 0;
    }

    public long GetLevel()
    {
        return level;
    }

    public long GetValue()
    {
        return value;
    }

    public TMission SetData(UserMission.MissionData data)
    {
        this.resID = data.id;
        this.value = data.value;
        this.level = data.level;

        return this;
    }

    public TMission SetResID(int resID)
    {
        this.resID = resID;

        return this;
    }
}
