public class TCard : TBase
{
    public int resID { get; private set; } = 0;
    public int count { get; private set; } = 0;
    public int bonusCount { get; private set; } = 0;

    public static TCard Of()
    {
        return new TCard();
    }

    public override void DoReset()
    {
        base.DoReset();
        resID = 0;
        count = 0;
        bonusCount = 0;
    }

    public int GetLevel()
    {
        return count + bonusCount;
    }

    public TCard SetData(UserCard.CardData data)
    {
        resID = data.id;
        count = data.count;
        bonusCount = data.bonusCount;

        return this;
    }

    public TCard SetResID(int id)
    {
        resID = id;
        count = 0;
        bonusCount = 0;

        return this;
    }
}
