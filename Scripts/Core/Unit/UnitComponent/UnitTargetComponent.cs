using System.Collections.Generic;
using UnityEngine;

namespace UnitComponent
{
    public class UnitTargetComponent : UnitBaseComponent
    {
        private readonly Dictionary<SkillType, Unit> dicTarget = new Dictionary<SkillType, Unit>();
        private readonly Dictionary<SkillType, long> dicTargetUID = new Dictionary<SkillType, long>();
        private KeyValuePair<SkillType, Unit> followTarget = new KeyValuePair<SkillType, Unit>(SkillType.NONE, null);

        private static SkillType[] priorityFollowTypes =
        {
            SkillType.SKILL_TELEPORT,
            SkillType.DEFAULT_ATTACK,
            SkillType.SKILL_ATTACK,
            SkillType.SKILL_BUFF,
        };

#if UNITY_EDITOR
        public ICollection<Unit> _targets => dicTarget.Values;
#endif

        public UnitTargetComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            ResetTargets();
        }

        public void ResetTargets()
        {
            dicTarget.Clear();
            followTarget = new KeyValuePair<SkillType, Unit>(SkillType.NONE, null);
        }

        public void SetTarget(SkillType key, Unit target)
        {
            if (key == SkillType.NONE)
            {
                return;
            }

            if (!dicTarget.ContainsKey(key))
            {
                dicTarget.Add(key, null);
            }

            dicTarget[key] = target;

            if (!dicTargetUID.ContainsKey(key))
            {
                dicTargetUID.Add(key, -1);
            }

            var isValidTarget = UnitRule.IsValid(target);
            dicTargetUID[key] = isValidTarget ? target.core.profile.tunit.uid : -1;

            var isNewFollowTarget = isValidTarget;
            if (!isValidTarget && followTarget.Key == key)
            {
                followTarget = new KeyValuePair<SkillType, Unit>(SkillType.NONE, null);
                isNewFollowTarget = true;
            }

            if (isNewFollowTarget)
            {
                foreach (var type in priorityFollowTypes)
                {
                    if (!dicTarget.TryGetValue(type, out var v))
                    {
                        continue;
                    }

                    followTarget = new KeyValuePair<SkillType, Unit>(type, v);
                    break;
                }
            }
        }

        public Unit GetTarget(SkillType key)
        {
            return dicTarget.TryGetValue(key, out var v) ? v : null;
        }

        public long GetTargetUID(SkillType key)
        {
            return dicTargetUID.TryGetValue(key, out var v) ? v : -1;
        }

        public Unit GetFollowTarget()
        {
            return followTarget.Value;
        }

        public virtual Unit GetPriorityTarget()
        {
            return null;
        }

        public virtual List<long> GetIgnoreTargetUIDs()
        {
            return null;
        }
    }
}