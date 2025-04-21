namespace ModeComponent
{
    public class KillModeEnterComponent : ModeEnterComponent
    {
        public KillModeEnterComponent(Mode mode) : base(mode)
        {

        }

        public override void OnEnable()
        {
            base.OnEnable();

            VirtualServer.Send(Packet.RESET_STAGE_MODE,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out RESET_STAGE_MODE tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MODE, tArg.tmode);
                });
        }
    }
}