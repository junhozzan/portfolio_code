public struct GetInfo
{
    public readonly int itemID;
    public readonly long amount;

    public static GetInfo Of(int itemID, long amount)
    {
        return new GetInfo(itemID, amount);
    }

    private GetInfo(int itemID, long amount)
    {
        this.itemID = itemID;
        this.amount = amount;
    }
}
