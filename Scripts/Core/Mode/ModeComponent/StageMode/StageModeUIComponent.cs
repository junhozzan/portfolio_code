namespace ModeComponent
{
    public class StageModeUIComponent : ModeUIComponent
    {
        public StageModeUIComponent(StageMode mode) : base(mode)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.MODE_FAIL, Handle_MODE_FAIL)
                ;
        }

        protected virtual void Handle_MODE_FAIL(object[] args)
        {
            Main.Instance.ShowFloatingMessage("key_stage_fail".L());
        }

        public override string GetScoreText()
        {
            return $"{"key_stage".L()} {GetScoreValueText()}";
        }
    }
}