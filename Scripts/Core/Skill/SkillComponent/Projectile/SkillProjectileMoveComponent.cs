using System;
using UnityEngine;

namespace Skill
{
    public class SkillProjectileMoveComponent : SkillBaseComponent
    {
        private readonly new SkillProjectile skill = null;

        private Vector2 moveDir = Vector2.zero;
        private float lerpSpeed = 0f;
        public bool isEndMove { get; private set; } = false;
        private float moveDistance = 0f;

        private float moveSpeed { get { return skill.core.profile.resScript.moveSpeed; } }
        private Action<float> onMove = null;

        public SkillProjectileMoveComponent(SkillProjectile skill) : base(skill)
        {
            this.skill = skill;
        }

        public override void DoReset()
        {
            base.DoReset();
            onMove = null;
            isEndMove = false;
            lerpSpeed = 5f;
            moveDistance = 0f;
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            onMove?.Invoke(dt);
            UpdateDisable();
        }

        public void SetOnMove()
        {
            var angle = Util.ToAngle(skill.core.profile.skillInfo.shotDir);
            moveDir = Util.ToUnitVector(angle);
            skill.core.obj.SetFlip(moveDir.x > 0);

            switch (skill.core.profile.resScript.moveType)
            {
                case ResourceSkillProjectile.MoveType.DIRECT:
                    onMove = OnMove_DIRECT;
                    break;
                case ResourceSkillProjectile.MoveType.GUIDE:
                    onMove = OnMove_GUIDE;
                    break;

                case ResourceSkillProjectile.MoveType.GUIDE_LERP:
                    onMove = OnMove_GUIDE_LERP;
                    break;
            }
        }

        private void OnMove_GUIDE(float dt)
        {
            if (dt == 0f)
            {
                return;
            }

            var objPos = skill.core.obj.GetPosition();
            var nextPos = Vector3.MoveTowards(objPos, skill.core.target.GetTargetPosition(), moveSpeed * dt);
            MoveProjectile(nextPos);

            if (!isEndMove && (nextPos - objPos).sqrMagnitude <= 0.004f)
            {
                isEndMove = true;
            }
        }

        private void OnMove_GUIDE_LERP(float dt)
        {
            if (dt == 0f)
            {
                return;
            }

            var moveAngle = Util.ToAngle(moveDir);
            var targetAngle = Util.ToAngle(skill.core.target.GetTargetPosition() - skill.core.obj.GetPosition());
            var lerpAngle = Mathf.Lerp(moveAngle, targetAngle, dt * lerpSpeed);

            lerpSpeed += dt;
            moveDir = Util.ToUnitVector(lerpAngle);
            MoveProjectile(skill.core.obj.GetPosition() + (Vector3)(dt * moveSpeed * moveDir));
        }

        private void OnMove_DIRECT(float dt)
        {
            if (dt == 0f)
            {
                return;
            }

            MoveProjectile(skill.core.obj.GetPosition() + (Vector3)(dt * moveSpeed * moveDir));
        }

        private void UpdateDisable()
        {
            if (moveDistance < skill.core.profile.resScript.disableDistance)
            {
                return;
            }

            skill.core.finish.Finish();
        }

        private void MoveProjectile(Vector3 nextPos)
        {
            if (skill.core.profile.resScript.isRotate)
            {
                skill.core.obj.SetRotation(moveDir);
            }

            var pos = skill.core.obj.GetPosition();
            skill.core.obj.SetPosition(nextPos);

            moveDistance += (nextPos - pos).magnitude;
        }
    }
}