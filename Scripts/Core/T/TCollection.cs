using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeStage.AntiCheat.ObscuredTypes;

public class TCollection : TBase
{
    public int resID { get; private set; } = 0;
    private ObscuredBool isComplete = false;

    public static TCollection Of()
    {
        return new TCollection();
    }

    private TCollection() : base()
    {

    }

    public override void DoReset()
    {
        base.DoReset();
        resID = 0;
        isComplete = false;
    }

    public bool IsComplete()
    {
        return isComplete;
    }

    public TCollection SetData(UserCollection.CollectionData data)
    {
        this.resID = data.id;
        this.isComplete = data.isComplete;
        return this;
    }

    public TCollection SetResID(int resID)
    {
        this.resID = resID;

        return this;
    }
}
