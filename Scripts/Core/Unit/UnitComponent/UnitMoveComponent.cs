using UnityEngine;

namespace UnitComponent
{
    public class UnitMoveComponent : UnitBaseComponent
    {
        private float moveSpeed = 0f;
        private float increaseSpeed = 1f;
        private Vector3 movePosition = Vector3.zero;
        private Vector2 moveDir = Vector2.left;

        protected virtual float _moveSpeed => moveSpeed;

        public UnitMoveComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            moveSpeed = 0f;
            increaseSpeed = 1f;
            movePosition = Vector3.zero;
            moveDir = Vector2.left;
        }

        public void Refresh()
        {
            moveSpeed = Env.Distance(owner.core.stat.GetValue(eAbility.MOVESPEED) * increaseSpeed);
        }

        public void SetIncreaseSpeed(float value)
        {
            increaseSpeed = value;
        }

        public void SetDirection(Vector2 dir)
        {
            moveDir = dir;
        }

        public void SetMovePoint(Vector2 pos)
        {
            movePosition = pos;
        }

        public void MoveTo(float dt)
        {
            owner.core.transform.SetPosition(Vector3.MoveTowards(owner.core.transform.GetPosition(), movePosition, _moveSpeed * dt));
        }

        public void Stop()
        {
            owner.core.transform.unitObj?.Handle_StopMove();
        }

        public Vector2 GetMoveDirection()
        {
            return moveDir;
        }
    }
}
