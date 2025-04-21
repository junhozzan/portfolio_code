using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    public class SkillProjectileTargetComponent : SkillBaseComponent
    {
        private readonly new SkillProjectile skill = null;
        private readonly List<Unit> targets = new List<Unit>();
        private readonly List<Unit> emptyTargets = new List<Unit>();

        private Unit target = null;
        private long targetUID = 0;
        private Vector3 targetPosition = Vector3.zero;

        public SkillProjectileTargetComponent(SkillProjectile skill) : base(skill)
        {
            this.skill = skill;

            GameEvent.Instance.CreateHandler(this, skill.IsUsed)
                .Add(GameEventType.CHANGE_UNIT_COUNT, Handle_UPDATE_UNIT_COUNT)
                ;
        }

        private void Handle_UPDATE_UNIT_COUNT(object[] args)
        {
            targets.Clear();
            targets.AddRange(FindTargets());
        }

        public override void DoReset()
        {
            base.DoReset();
            targets.Clear();
            target = null;
            targetUID = 0;
            targetPosition = Vector3.zero;
        }

        public void SetTarget()
        {
            switch (skill.core.profile.resScript.targetType)
            {
                case ResourceSkillProjectile.TargetType.TARGET:
                    target = skill.core.profile.skillInfo._target;
                    targetPosition = target.core.transform.GetCenterPosition();
                    targetUID = target.core.profile.tunit.uid;
                    break;

                case ResourceSkillProjectile.TargetType.TARGET_POSITION:
                    var tempTarget = skill.core.profile.skillInfo._target;
                    targetPosition = tempTarget.core.transform.GetCenterPosition();
                    targetUID = tempTarget.core.profile.tunit.uid;
                    break;
            }

            targets.Clear();
            targets.AddRange(FindTargets());
        }

        private List<Unit> FindTargets()
        {
            var from = skill.core.profile.skillInfo._from;
            if (!UnitRule.IsAlive(from))
            {
                return emptyTargets;
            }

            return UnitManager.Instance.FindTargets(
                from, 
                skill.core.profile.skillInfo.resFire.applyTargets,
                skill.core.hit.hitedTargetUIDs);
        }

        public Vector3 GetTargetPosition()
        {
            if (!UnitRule.IsAlive(target) || target.core.profile.tunit.uid != targetUID)
            {
                return targetPosition;
            }

            targetPosition = target.core.transform.GetCenterPosition();
            return targetPosition;
        }

        public List<Unit> GetTargets()
        {
            return targets;
        }

        public Unit GetTarget()
        {
            return target;
        }
    }
}