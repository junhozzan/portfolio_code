namespace ModeComponent
{
    public class ModeCoreComponent : ModeBaseComponent
    {
        public readonly ModeSavedComponent saved = null;
        public readonly ModeProfileComponent profile = null;
        public readonly ModeEnterComponent enter = null;
        public readonly ModeStateComponent state = null;
        public readonly ModeAllyComponent ally = null;
        public readonly ModeEnemyComponent enemy = null;
        public readonly ModeFieldComponent field = null;
        public readonly ModeScoreComponent score = null;
        public readonly ModeAIComponent ai = null;
        public readonly ModeCameraComponent camera = null;
        public readonly ModeCardComponent card = null;
        public readonly ModeUIComponent ui = null;

        protected ModeCoreComponent(Mode mode) : base(mode)
        {
            // 컴포넌트 호출순서 중요
            this.saved = AddSavedComponent();
            this.profile = AddProfileComponent();
            this.enter = AddEnterComponent();
            this.state = AddStateComponent();
            this.ai = AddAIComponent();
            this.score = AddScoreComponent();
            this.camera = AddCameraComponent();
            this.field = AddFieldComponent();
            this.ally = AddAllyComponent();
            this.enemy = AddEnemyComponent();
            this.card = AddCardComponent();
            // ui는 가장 마지막에 등록
            this.ui = AddUIComponent();
        }

        public static ModeCoreComponent Of(Mode mode)
        {
            return new ModeCoreComponent(mode);
        }

        protected virtual ModeProfileComponent AddProfileComponent()
        {
            return AddComponent<ModeProfileComponent>(mode);
        }

        protected virtual ModeEnterComponent AddEnterComponent()
        {
            return AddComponent<ModeEnterComponent>(mode);
        }

        protected virtual ModeAllyComponent AddAllyComponent()
        {
            return AddComponent<ModeAllyComponent>(mode);
        }

        protected virtual ModeEnemyComponent AddEnemyComponent()
        {
            return AddComponent<ModeEnemyComponent>(mode);
        }

        protected virtual ModeCameraComponent AddCameraComponent()
        {
            return AddComponent<ModeCameraComponent>(mode);
        }

        protected virtual ModeFieldComponent AddFieldComponent()
        {
            return AddComponent<ModeFieldComponent>(mode);
        }

        protected virtual ModeAIComponent AddAIComponent()
        {
            return AddComponent<ModeAIComponent>(mode);
        }

        protected virtual ModeScoreComponent AddScoreComponent()
        {
            return AddComponent<ModeScoreComponent>(mode);
        }

        protected virtual ModeStateComponent AddStateComponent()
        {
            return AddComponent<ModeStateComponent>(mode);
        }

        protected virtual ModeCardComponent AddCardComponent()
        {
            return AddComponent<ModeCardComponent>(mode);
        }

        protected virtual ModeUIComponent AddUIComponent()
        {
            return AddComponent<ModeUIComponent>(mode);
        }

        protected virtual ModeSavedComponent AddSavedComponent()
        {
            return AddComponent<ModeSavedComponent>(mode);
        }
    }
}