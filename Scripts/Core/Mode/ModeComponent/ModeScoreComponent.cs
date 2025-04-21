using System;

namespace ModeComponent
{
    public class ModeScoreComponent : ModeBaseComponent
    {
        public ModeScoreComponent(Mode mode) : base(mode)
        {

        }

        public virtual long GetScore()
        {
            var tmode = MyPlayer.Instance.core.mode.GetMode(mode.core.profile.resMode.id);
            return tmode.GetScore();
        }

        public virtual long GetBestScore()
        {
            var tmode = MyPlayer.Instance.core.mode.GetMode(mode.core.profile.resMode.id);
            return tmode.GetBestScore();
        }

        //public virtual string GetScoreText()
        //{
        //    return ScoreValueToText(GetScore());
        //}

        //public string GetBestScoreValueText()
        //{
        //    return ScoreValueToText(GetBestScore());
        //}

        //public virtual string ScoreValueToText(long score)
        //{
        //    return Util.ToComma(score);
        //}
    }
}