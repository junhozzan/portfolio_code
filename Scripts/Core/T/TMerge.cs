using System.Collections.Generic;

public class TMerge : TBase
{
    public int id => resMerge != null ? resMerge.id : 0;
    public ResourceMerge resMerge { get; private set; } = null;
    public int count { get; private set; } = 0;
    public readonly List<int> locateList = new List<int>();

    public static TMerge Of()
    {
        return new TMerge();
    }

    public override void DoReset()
    {
        base.DoReset();
        resMerge = null;
        count = 0;
        locateList.Clear();
    }

    public TMerge SetData(UserMerge.MergeData data)
    {
        resMerge = ResourceManager.Instance.merge.GetMerge(data.id);
        count = data.count;

        locateList.Clear();
        locateList.AddRange(data.locateList);

        return this;
    }
}
