namespace ModeComponent
{
    public class StageModeCoreComponent : ModeCoreComponent
    {
        public readonly new StageModeProfileComponent profile = null;
        public readonly new StageModeFieldComponent field = null;

        private StageModeCoreComponent(Mode mode) : base(mode)
        {
            this.profile = base.profile as StageModeProfileComponent;
            this.field = base.field as StageModeFieldComponent;
        }

        public static new StageModeCoreComponent Of(Mode mode)
        {
            return new StageModeCoreComponent(mode);
        }

        protected override ModeProfileComponent AddProfileComponent()
        {
            return AddComponent<StageModeProfileComponent>(mode);
        }

        protected override ModeEnterComponent AddEnterComponent()
        {
            return AddComponent<StageModeEnterComponent>(mode);
        }

        protected override ModeEnemyComponent AddEnemyComponent()
        {
            return AddComponent<StageModeEnemyComponent>(mode);
        }

        protected override ModeFieldComponent AddFieldComponent()
        {
            return AddComponent<StageModeFieldComponent>(mode);
        }

        protected override ModeAIComponent AddAIComponent()
        {
            return AddComponent<StageModeAIComponent>(mode);
        }

        protected override ModeUIComponent AddUIComponent()
        {
            return AddComponent<StageModeUIComponent>(mode);
        }

        protected override ModeCameraComponent AddCameraComponent()
        {
            return AddComponent<StageModeCameraComponent>(mode);
        }
    }
}