using UnityEngine;

namespace ModeComponent
{
    public class StageModeCameraComponent : ModeCameraComponent
    {
        private readonly new StageMode mode = null;

        public StageModeCameraComponent(StageMode mode) : base(mode)
        {
            this.mode = mode;
        }

        public override Vector3 GetCameraPosition()
        {
            var pos = base.GetCameraPosition();
            pos += mode.core.field.GetGuideVector() * 2f;
            pos.y -= 3f;

            return pos;
        }
    }
}