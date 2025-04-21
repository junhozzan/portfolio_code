using System.Collections.Generic;

public class TInventory : TBase
{
    public List<int> equipedItemIDs { get; private set; } = null;

    public static TInventory Of()
    {
        return new TInventory();
    }

    private TInventory() : base()
    {

    }

    public override void DoReset()
    {
        base.DoReset();
        equipedItemIDs = null;
    }

    public TInventory SetInfo(UserInventory inventory)
    {
        equipedItemIDs = inventory.equipedItemIDs;

        return this;
    }
}
