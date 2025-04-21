using CodeStage.AntiCheat.ObscuredTypes;

public class TMode : TBase
{
    public int id => resMode != null ? resMode.id : 0;
    public ResourceMode resMode { get; private set; } = null;
    private ObscuredLong exp = 0;
    private ObscuredLong score = 1;
    private ObscuredLong bestScore = 0;
    private ObscuredLong addCardCount = 0;

    public static TMode Of()
    {
        return new TMode();
    }

    public long GetExp()
    {
        return exp;
    }

    public long GetScore()
    {
        return score;
    }

    public long GetBestScore()
    {
        return bestScore;
    }

    public long GetAddCardCount()
    {
        return addCardCount;
    }

    public override void DoReset()
    {
        base.DoReset();
        resMode = null;
        exp = 0;
        score = 1;
        bestScore = 0;
        addCardCount = 0;
    }

    public TMode SetData(UserMode.ModeData data)
    {
        resMode = ResourceManager.Instance.mode.GetMode(data.id);
        exp = data.exp;
        score = data.score;
        bestScore = data.bestScore;
        addCardCount = data.addCardCount;

        return this;
    }

    public TMode SetResID(int id)
    {
        resMode = ResourceManager.Instance.mode.GetMode(id);
        return this;
    }
}
