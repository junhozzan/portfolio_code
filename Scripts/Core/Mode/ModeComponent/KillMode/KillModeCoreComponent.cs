namespace ModeComponent
{
    public class KillModeCoreComponent : ModeCoreComponent
    {
        public readonly new KillModeFieldComponent field = null;

        private KillModeCoreComponent(Mode mode) : base(mode)
        {
            this.field = base.field as KillModeFieldComponent;
        }

        public static new KillModeCoreComponent Of(Mode mode)
        {
            return new KillModeCoreComponent(mode);
        }

        protected override ModeEnterComponent AddEnterComponent()
        {
            return AddComponent<KillModeEnterComponent>(mode);
        }

        protected override ModeEnemyComponent AddEnemyComponent()
        {
            return AddComponent<KillModeEnemyComponent>(mode);
        }

        protected override ModeAIComponent AddAIComponent()
        {
            return AddComponent<KillModeAIComponent>(mode);
        }

        protected override ModeScoreComponent AddScoreComponent()
        {
            return AddComponent<KillModeScoreComponent>(mode);
        }

        protected override ModeUIComponent AddUIComponent()
        {
            return AddComponent<KillModeUIComponent>(mode);
        }

        protected override ModeFieldComponent AddFieldComponent()
        {
            return AddComponent<KillModeFieldComponent>(mode);
        }
    }
}