using UnityEngine;

namespace Skill
{
    public class SkillBaseComponent : XComponent
    {
        protected readonly SkillBase skill = null;

        public SkillBaseComponent(SkillBase skill)
        {
            this.skill = skill;
        }

        public void PlayTail(Unit target, Vector3 hitPosition)
        {
            var skillInfo = skill.core.profile.skillInfo;
            if (skillInfo.resFire.tail == null)
            {
                return;
            }

            if (!UnitRule.IsValid(skillInfo._from))
            {
                return;
            }

            var tailInfo = SkillManager.Instance.PopCopyInfo(skillInfo);
            tailInfo.SetFire(skillInfo.resFire.tail);
            tailInfo.SetTarget(target);
            tailInfo.SetHitPosition(hitPosition);

            if (!SkillManager.Instance.Play(tailInfo))
            {
                tailInfo.OnDisable();
            }
        }
    }
}