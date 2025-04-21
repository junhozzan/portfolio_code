using UnityEngine;

namespace ModeComponent
{
    public class DamageModeTimeComponent : ModeBaseComponent
    {
        private float endTime = 0f;

        public DamageModeTimeComponent(Mode mode) : base(mode)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.MODE_START, Handle_MODE_START)
                ;
        }

        private void Handle_MODE_START(object[] args)
        {
            endTime = Main.Instance.time.realtimeSinceStartup + 10f;
        }

        public float GetRemainTime()
        {
            return Mathf.Max(endTime - Main.Instance.time.realtimeSinceStartup, 0f);
        }

        public bool IsEnd()
        {
            return Main.Instance.time.realtimeSinceStartup >= endTime;
        }
    }
}