
namespace ModeComponent
{
    public class ModeEnterComponent : ModeBaseComponent
    {
        public ModeEnterComponent(Mode mode) : base(mode)
        {

        }

        public override void OnEnable()
        {
            base.OnEnable();

            SoundManager.Instance.PlayBgm(mode.core.profile.resMode.bgm);
            GameEvent.Instance.AddEvent(GameEventType.MODE_ENTER);
        }
    }
}