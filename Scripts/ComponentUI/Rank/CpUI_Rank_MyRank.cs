
namespace UIRank
{
    public class CpUI_Rank_MyRank : CpUI_Rank_Cell
    {
        public override void Refresh()
        {
            base.Refresh();

            if (rank == null)
            {
                return;
            }

            var resMode = ResourceManager.Instance.mode.GetMode(rank.modeID);
            if (resMode == null)
            {
                return;
            }

            var tmode = MyPlayer.Instance.core.mode.GetMode(resMode.id);
            nameText.SetText(MyPlayer.Instance.core.profile.info.nickName);
            recordText.SetText($"{recordText._strText}({GetRecordText(rank.modeID, tmode.GetBestScore())})");
            levelText.SetText($"{levelText._strText}({ResourceManager.Instance.level.GetExpToLevel(tmode.GetExp())})");
        }
    }
}