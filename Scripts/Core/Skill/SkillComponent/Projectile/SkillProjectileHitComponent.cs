using System.Collections.Generic;

namespace Skill
{
    public class SkillProjectileHitComponent : SkillBaseComponent
    {
        private readonly new SkillProjectile skill = null;
        public readonly List<long> hitedTargetUIDs = new List<long>(64);

        public SkillProjectileHitComponent(SkillProjectile skill) : base(skill)
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
            target.core.skill.PlayEffect(skill.core.profile.resScript.effect, target.core.transform);

            PlayTail(target, target.core.transform.GetCenterPosition());
        }

        public bool IsMaxHitTarget()
        {
            return hitedTargetUIDs.Count >= skill.core.profile.resScript.maxHit;
        }
    }
}