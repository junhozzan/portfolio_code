using UnityEngine;

namespace ModeComponent
{
    public class ModeCameraComponent : ModeBaseComponent
    {
        public Camera_2D camera { get; protected set; } = null;
        public Vector3 followCameraPos { get; protected set; } = Vector3.zero;

        private static readonly Vector3 cameraOffset = new Vector3(0f, Env.Distance(60));

        public ModeCameraComponent(Mode mode) : base(mode)
        {

        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (camera == null)
            {
                camera = ObjectManager.Instance.Pop<Camera_2D>("pf_camera_2d");
            }

            followCameraPos = mode.core.saved.cameraFollowPosition;
            SetCameraPosition(mode.core.saved.cameraPosition);
        }

        public override void OnDisable()
        {
            SetToZero();

            if (camera != null)
            {
                camera.SetActive(false);
                camera = null;
            }

            base.OnDisable();
        }

        public void SetToZero()
        {
            var myUnit = mode.core.ally.myUnit;
            if (myUnit == null)
            {
                return;
            }

            var pos = GetCameraPosition();
            followCameraPos -= pos;
            SetCameraPosition(camera.GetPosition() - pos);
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            UpdateCamera(dt);
        }

        private void UpdateCamera(float dt)
        {
            if (camera == null)
            {
                return;
            }

            followCameraPos = Vector2.Lerp(followCameraPos, GetCameraPosition(), dt * 5.5f);
            SetCameraPosition(followCameraPos);
        }

        public void SetCameraPosition(Vector2 camPos)
        {
            camera.SetPosition(camPos);
        }

        public bool IsInView(Vector2 position, Vector2 offset)
        {
            if (camera == null)
            {
                return false;
            }

            return camera.GetViewRect(offset).Contains(position);
        }

        public virtual Vector3 GetCameraPosition()
        {
            var myUnit = mode.core.ally.myUnit;
            if (myUnit == null)
            {
                return Vector3.zero;
            }

            var pos = myUnit.core.transform.GetPosition();
            return pos + cameraOffset;
        }
    }
}