using MyPlayerComponent;
using UnityEditor;
using UnityEngine;

namespace UIRank
{
    public class CpUI_Rank_Cell : MyOSABasicItem
    {
        [SerializeField] UIImage bg = null;
        [SerializeField] protected UIText rankText = null;
        [SerializeField] protected UIText numberText = null;
        [SerializeField] protected UIText nameText = null;
        [SerializeField] protected UIText levelText = null;
        [SerializeField] protected UIText recordText = null;

        protected Rank rank = null;

        public override void DoReset()
        {
            rank = null;

            bg.SetColor(GetRankColor(0));
            rankText.SetText("--");
            numberText.SetText("--");
            nameText.SetText("--");
            recordText.SetText("--");
            levelText.SetText("--");
        }

        public override void Refresh()
        {
            if (rank == null)
            {
                return;
            }

            bg.SetColor(GetRankColor(rank.rank));
            rankText.SetText(rank.rank > 0 ? rank.rank.ToString() : "--");
            numberText.SetText(!string.IsNullOrEmpty(rank.user) ? rank.user : "--");
            nameText.SetText(!string.IsNullOrEmpty(rank.name) ? rank.name : "--");
            recordText.SetText(GetRecordText(rank.modeID, rank.record));
            levelText.SetText(rank.level > 0 ? rank.level.ToString() : "--");
        }

        protected string GetRecordText(int modeID, long score)
        {
            if (score <= 0)
            {
                return "--";
            }

            var mode = ModeManager.Instance.GetMode(modeID);
            if (mode == null)
            {
                return "--";
            }

            return mode.core.ui.ScoreValueToText(score);
        }

        public void SetRank(Rank rank)
        {
            this.rank = rank;
            Refresh();
        }

        public override void UpdateViews(MyOSABasic.IOsaItem tOsaItem)
        {
            if (!(tOsaItem is CpUI_Rank.RankOsaItem osaItem))
            {
                return;
            }

            SetRank(osaItem.rank);
        }

        private static Color GetRankColor(int rank)
        {
            if (rank == 1)
            {
                return GameData.COLOR.RANK_1;
            }
            else if (rank == 2)
            {
                return GameData.COLOR.RANK_2;
            }
            else if (rank == 3)
            {
                return GameData.COLOR.RANK_3;
            }

            return GameData.COLOR.RANK_DEFAULT;
        }
    }
}