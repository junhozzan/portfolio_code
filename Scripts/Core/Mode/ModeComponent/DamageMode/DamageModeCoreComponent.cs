namespace ModeComponent
{
    public class DamageModeCoreComponent : ModeCoreComponent
    {
        public readonly new DamageModeFieldComponent field = null;
        public readonly DamageModeTimeComponent time = null;

        private DamageModeCoreComponent(Mode mode) : base(mode)
        {
            this.field = base.field as DamageModeFieldComponent;

            time = AddComponent<DamageModeTimeComponent>(mode);
        }

        public static new DamageModeCoreComponent Of(Mode mode)
        {
            return new DamageModeCoreComponent(mode);
        }

        protected override ModeEnterComponent AddEnterComponent()
        {
            return AddComponent<DamageModeEnterComponent>(mode);
        }

        protected override ModeEnemyComponent AddEnemyComponent()
        {
            return AddComponent<DamageModeEnemyComponent>(mode);
        }

        protected override ModeAIComponent AddAIComponent()
        {
            return AddComponent<DamageModeAIComponent>(mode);
        }

        protected override ModeScoreComponent AddScoreComponent()
        {
            return AddComponent<DamageModeScoreComponent>(mode);
        }

        protected override ModeUIComponent AddUIComponent()
        {
            return AddComponent<DamageModeUIComponent>(mode);
        }

        protected override ModeFieldComponent AddFieldComponent()
        {
            return AddComponent<DamageModeFieldComponent>(mode);
        }
    }
}