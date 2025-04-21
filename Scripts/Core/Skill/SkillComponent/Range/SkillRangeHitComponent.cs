using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    public class SkillRangeHitComponent : SkillBaseComponent
    {
        private readonly new SkillRange skill = null;
        public readonly List<long> hitedTargetUIDs = new List<long>(64);

        public SkillRangeHitComponent(SkillRange skill) : base(skill)
        {
            this.skill = skill;
        }

        public override void DoReset()
        {
            base.DoReset();
            hitedTargetUIDs.Clear();
        }

        public void HitTarget(Unit target)
        {
            hitedTargetUIDs.Add(target.core.profile.tunit.uid);

            PlayTail(target, target.core.transform.GetCenterPosition());
        }

        public bool IsMaxHitTarget()
        {
            return hitedTargetUIDs.Count >= skill.core.profile.resScript.maxHit;
        }
    }
}