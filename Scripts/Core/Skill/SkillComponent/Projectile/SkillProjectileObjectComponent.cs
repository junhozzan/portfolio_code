using UnityEngine;

namespace Skill
{
    public class SkillProjectileObjectComponent : SkillBaseComponent
    {
        private readonly new SkillProjectile skill = null;
        private CpProjectile obj = null;

        public SkillProjectileObjectComponent(SkillProjectile skill) : base(skill)
        {
            this.skill = skill;
        }

        public override void DoReset()
        {
            base.DoReset();
            obj = null;
        }


        public override void OnDisable()
        {
            if (obj != null)
            {
                obj.DelayInactive(skill.core.profile.resScript.objectRemainTime);
                obj = null;
            }

            base.OnDisable();
        }

        public void CreateObject()
        {
            var res = skill.core.profile.resScript;
            var skillInfo = skill.core.profile.skillInfo;

            if (string.IsNullOrEmpty(res.prefabInfo.prefab))
            {
                return;
            }

            obj = ObjectManager.Instance.Pop<CpProjectile>(res.prefabInfo.prefab, active: false);
            obj.SetLayer(res.prefabInfo.layer);

            if (res.prefabInfo.atlas != Atlas.NONE)
            {
                obj.SetSprite(AtlasManager.Instance.GetSprite(res.prefabInfo.atlas, res.prefabInfo.sprite));
            }

            var shotPosition = skillInfo.shotPosition;
            switch (res.shotPointType)
            {
                case ShotPointType.CASTER_POSITION:
                    shotPosition = skillInfo._from.core.transform.GetCenterPosition();
                    break;

                case ShotPointType.TARGET_POSITION:
                    shotPosition = skill.core.target.GetTargetPosition();
                    break;
            }
            obj.SetPosition(shotPosition + Vector3.Lerp(res.customPositionA, res.customPositionB, Random.Range(0f, 1f)));

            if (skill.core.profile.resScript.isShotDirToTarget)
            {
                var getPosition = obj.GetPosition();
                var targetPosition = skill.core.target.GetTargetPosition();
                skill.core.profile.skillInfo.SetShotDir(targetPosition - getPosition);
            }

            if (skill.core.profile.resScript.isRotate)
            {
                obj.SetRotate2D(skill.core.profile.skillInfo.shotDir);
            }

            obj.SetActive(true);
        }

        public void SetPosition(Vector3 pos)
        {
            obj.SetPosition(pos);
        }

        public Vector3 GetPosition()
        {
            return obj.GetPosition();
        }

        public bool IsTrigger(Unit target)
        {
            return obj.IsTrigger(target.core.transform.GetMyCollider());
        }

        public void SetRotation(Vector2 dir)
        {
            obj.SetRotate2D(dir);
        }

        public void SetFlip(bool flip)
        {
            obj.SetFlip(flip);
        }
    }
}