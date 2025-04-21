namespace UnitComponent
{
    public class ShadowSpecialUnitUIComponent : UnitUIComponent
    {
        public ShadowSpecialUnitUIComponent(Unit owner) : base(owner)
        {

        }

        public override void CreateUnitUI()
        {
            base.CreateUnitUI();
            owner.core.ui.SetNameText(GetNameText());
        }

        private string GetNameText()
        {
            var mode = ModeManager.Instance.mode;
            if (mode == null)
            {
                return string.Empty;
            }

            var rankInfo = MyPlayer.Instance.core.mode.rank.GetRankInfo(mode.core.profile.resMode.id);
            if (rankInfo.top50.Count == 0)
            {
                return MyPlayer.Instance.core.profile.info.nickName;
            }

            return rankInfo.top50[0].name;
        }
    }
}