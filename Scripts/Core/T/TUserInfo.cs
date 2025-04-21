using System;

public class TUserInfo : TBase
{
    public string serial { get; private set; } = string.Empty;
    public string nickName { get; private set; } = string.Empty;
    public DateTime lastLink { get; private set; } = DateTime.MinValue;
    public decimal createVersion { get; private set; } = 0m;
    public long guideFlag { get; private set; } = 0;

    public static TUserInfo Of()
    {
        return new TUserInfo();
    }

    private TUserInfo() : base()
    {

    }

    public override void DoReset()
    {
        base.DoReset();

        serial = string.Empty;
        nickName = string.Empty;
        lastLink = DateTime.MinValue;
        createVersion = 0m;
        guideFlag = 0;
    }

    public TUserInfo SetInfo(UserInfo info)
    {
        serial = info.serial;
        nickName = info.nickName;
        lastLink = info.lastLink;
        createVersion = info.createVersion;
        guideFlag = info.guideFlag;

        return this;
    }
}
