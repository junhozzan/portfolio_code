namespace ModeComponent
{
    public class KillModeUIComponent : ModeUIComponent
    {
        public KillModeUIComponent(Mode mode) : base(mode)
        {

        }

        public override string GetScoreText()
        {
            return $"{"key_kill_count".L()} {GetScoreValueText()}";
        }
    }
}