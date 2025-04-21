public class TAdvertisement : TBase
{
    public int resID { get; private set; } = 0;
    public long coolTime { get; private set; } = 0;

    public static TAdvertisement Of() 
    {
        return new TAdvertisement();
    }

    private TAdvertisement() : base()
    {

    }

    public override void DoReset()
    {
        base.DoReset();
        resID = 0;
        coolTime = 0;
    }

    public TAdvertisement SetData(UserAdvertisement.AdvertisementData data)
    {
        this.resID = data.id;
        this.coolTime = data.coolTime;

        return this;
    }

    public TAdvertisement SetResID(int resID)
    {
        this.resID = resID;
        return this;
    }
}