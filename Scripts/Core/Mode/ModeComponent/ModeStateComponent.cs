
namespace ModeComponent
{
    public class ModeStateComponent : ModeBaseComponent
    {
        public ModeState modeState { get; protected set; } = ModeState.NONE;

        public ModeStateComponent(Mode mode) : base(mode)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.MODE_ENTER, Handle_MODE_ENTER)
                .Add(GameEventType.MODE_START, Handle_MODE_START)
                .Add(GameEventType.MODE_SUCCESS, Handle_MODE_SUCCESS)
                .Add(GameEventType.MODE_FAIL, Handle_MODE_FAIL)
                ;
        }

        public override void OnDisable()
        {
            modeState = ModeState.NONE;

            base.OnDisable();
        }

        protected void Handle_MODE_ENTER(object[] args)
        {
            SetModeState(ModeState.JOIN_FIELD);
        }

        protected void Handle_MODE_START(object[] args)
        {
            SetModeState(ModeState.PLAYING);
        }

        protected virtual void Handle_MODE_SUCCESS(object[] args)
        {
            SetModeState(ModeState.WAITING);
        }

        protected void Handle_MODE_FAIL(object[] args)
        {
            SetModeState(ModeState.WAITING);
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            switch (modeState)
            {
                case ModeState.JOIN_FIELD:
                    GameEvent.Instance.AddEvent(GameEventType.JOIN_FIELD);
                    SetModeState(ModeState.JOIN_MY_UNIT);
                    break;

                case ModeState.JOIN_MY_UNIT:
                    mode.core.ally.SpawnMyUnit();
                    SetModeState(ModeState.WAITING);
                    break;
            }
        }

        public void SetModeState(ModeState state)
        {
            this.modeState = state;
        }

        public bool CheckState(ModeState state)
        {
            return modeState == state;
        }

        public enum ModeState
        {
            NONE,
            WAITING,
            JOIN_FIELD,
            JOIN_MY_UNIT,

            PLAYING,
            END
        }
    }
}