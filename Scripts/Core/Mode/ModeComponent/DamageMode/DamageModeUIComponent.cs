namespace ModeComponent
{
    public class DamageModeUIComponent : ModeUIComponent
    {
        private readonly DamageMode damageMode = null;

        public DamageModeUIComponent(Mode mode) : base(mode)
        {
            this.damageMode = mode as DamageMode;
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.MODE_START, Handle_MODE_START)
                ;
        }

        private void Handle_MODE_START(object[] args)
        {
            RefreshTimeText();
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            RefreshTimeText();
        }

        private void RefreshTimeText()
        {
            ui.SetTimeText($"{"key_remain_time".L()} {Util.SecToTimer(damageMode.core.time.GetRemainTime())}");
        }

        public override string GetScoreText()
        {
            return $"{"key_damage_amount".L()} {GetScoreValueText()}";
        }
    }
}