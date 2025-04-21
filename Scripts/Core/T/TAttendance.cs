using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeStage.AntiCheat.ObscuredTypes;

public class TAttendance : TBase
{
    public int resID { get; private set; } = 0;
    private ObscuredInt count = 0;
    private ObscuredLong flag = 0;
    private ObscuredLong completeDate = 0; 

    public static TAttendance Of()
    {
        return new TAttendance();
    }

    private TAttendance() : base()
    {

    }

    public override void DoReset()
    {
        base.DoReset();
        resID = 0;
        flag = 0;
        count = 0;
        completeDate = 0;
    }

    public int GetCount()
    {
        return count;
    }

    public long GetFlag()
    {
        return flag;
    }

    public long GetCompleteDate()
    {
        return completeDate;
    }

    public TAttendance SetData(UserAttendance.AttendanceData data)
    {
        this.resID = data.id;
        this.flag = data.flag;
        this.count = data.count;
        this.completeDate = data.completeDate;

        return this;
    }

    public TAttendance SetResID(int resID)
    {
        this.resID = resID;

        return this;
    }
}
