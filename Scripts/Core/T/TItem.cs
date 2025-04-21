using CodeStage.AntiCheat.ObscuredTypes;

public class TItem : TBase
{
    public int id => resItem != null ? resItem.id : 0;
    public ResourceItem resItem { get; private set; } = null;
    private ObscuredLong exp = 0;
    private ObscuredLong amount = 0;
    private ObscuredInt awaken = 0;
    private ObscuredFloat[] options = new ObscuredFloat[0];
    private float[] optionValues = new float[0];
    public UpdateFlag updateFlag = UpdateFlag.NONE;

    public static TItem Of()
    {
        return new TItem();
    }

    private TItem() : base()
    {

    }

    public override void DoReset()
    {
        base.DoReset();
        resItem = null;
        exp = 0;
        amount = 0;
        awaken = 0;
        options = Util.InitArray(options, GameData.DEFAULT.MAX_ITEM_OPTION_COUNT, 0f);
        optionValues = Util.InitArray(optionValues, GameData.DEFAULT.MAX_ITEM_OPTION_COUNT, 0f);
        updateFlag = UpdateFlag.NONE;
    }

    public long GetLevel()
    {
        return resItem.GetExpToLevel(exp);
    }

    public long GetAmount()
    {
        return amount;
    }

    public int GetAwaken()
    {
        return awaken;
    }

    public long GetExp()
    {
        return exp;
    }

    public float[] GetOptions()
    {
        Util.InitArray(optionValues, options.Length, 0f);
        for (int i = 0; i < optionValues.Length; ++i)
        {
            optionValues[i] = options[i];
        }

        return optionValues;
    }

    public TItem SetData(UserItem.ItemData data)
    {
        this.resItem = ResourceManager.Instance.item.GetItem(data.id);
        this.exp = data.exp;
        this.amount = data.amount;
        this.awaken = data.awaken;

        for (int i = 0; i < data.options.Length; ++i)
        {
            this.options[i] = data.options[i];
        }

        return this;
    }

    public TItem SetResID(int resID)
    {
        this.resItem = ResourceManager.Instance.item.GetItem(resID);
        return this;
    }

    public TItem AddUpdateFlag(UpdateFlag flag)
    {
        updateFlag |= flag;
        return this;
    }

    public enum UpdateFlag
    {
        NONE = 0,
        NEW = 1 << 0,
        AMOUNT = 1 << 1,
        ALL = 1 << 2,
    }
}
