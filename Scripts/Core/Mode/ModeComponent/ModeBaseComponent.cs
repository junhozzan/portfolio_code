using UnityEngine;

namespace ModeComponent
{
    public class ModeBaseComponent : XComponent
    {
        protected readonly Mode mode = null;

        public ModeBaseComponent(Mode mode)
        {
            this.mode = mode;
            
            CreateHandler();
        }

        protected virtual EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(mode, IsCallHandler);
        }

        private bool IsCallHandler()
        {
            return ModeManager.Instance.mode == mode;
        }
    }
}