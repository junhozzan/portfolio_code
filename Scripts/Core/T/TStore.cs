using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public class TStore : TBase
{
    public int resID { get; private set; } = 0;
    private ObscuredInt purchaseCount = 0;
    
    public static TStore Of()
    {
        return new TStore();
    }

    private TStore() : base()
    {

    }

    public override void DoReset()
    {
        base.DoReset();
        resID = 0;
        purchaseCount = 0;
    }

    public long GetPurchaseCount()
    {
        return purchaseCount;
    }

    public TStore SetResID(int resID)
    {
        this.resID = resID;

        return this;
    }

    public TStore SetData(UserStore.StoreData data)
    {
        this.resID = data.id;
        this.purchaseCount = data.purchaseCount;

        return this;
    }
}
