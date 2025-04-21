using UnityEngine;

namespace UnitComponent
{
	public class UnitJumpComponent : UnitBaseComponent
	{
        private float time = 0f;
        private float speed = 0f;
        private Vector3 point = Vector3.zero;

        public UnitJumpComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            time = 0f;
        }

        public void SetJump(float time, Vector3 point)
        {
            this.time = time;
            this.point = point;
            this.speed = Vector2.Distance(point, owner.core.transform.GetPosition()) / time;
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            UpdateJump(dt);
        }

        private void UpdateJump(float dt)
        {
            if (!IsJumpState())
            {
                return;
            }

            var pos = owner.core.transform.GetPosition();
            owner.core.transform.SetPosition(Vector3.MoveTowards(pos, point, dt * speed));

            time -= dt;
        }

        public void Stop()
        {
            time = 0f;
        }

        public bool IsJumpState()
        {
            return time > 0f;
        }
    }
}