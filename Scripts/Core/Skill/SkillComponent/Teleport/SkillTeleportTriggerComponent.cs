using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Skill
{
    public class SkillTeleportTriggerComponent : SkillBaseComponent
    {
        private readonly new SkillTeleport skill = null;
        private readonly static ReadOnlyCollection<SkillType> ignoreSkillTypes = new SkillType[]
        {
            SkillType.SKILL_TELEPORT
        }.ReadOnly();

        public SkillTeleportTriggerComponent(SkillTeleport skill) : base(skill)
        {
            this.skill = skill;
        }

        public bool Exec()
        {
            var skillInfo = skill.core.profile.skillInfo;
            var caster = skillInfo._from;
            var target = skillInfo._target;
            if (!UnitRule.IsAlive(caster) || !UnitRule.IsValid(target))
            {
                skill.core.finish.Finish();
                return false;
            }

            var casterPos = caster.core.transform.GetPosition();
            var targetPos = target.core.transform.GetPosition();
            var offset = targetPos - casterPos;
            var dist = offset.magnitude;
            var minSkillRange = caster.core.skill.enroll.GetMinSkillRange(ignoreSkillTypes);

            if (dist <= minSkillRange)
            {
                skill.core.finish.Finish();
                return false;
            }

            var resScript = skill.core.profile.resScript;
            var ranVec = (Vector3)Util.AddAngle(-offset.normalized, Random.Range(-resScript.boundAngle, resScript.boundAngle));
            var movePos = targetPos + 0.9f * minSkillRange * ranVec; // 0.9:좀더 안쪽으로 이동

            caster.core.move.SetDirection(movePos - casterPos);
            caster.core.transform.UpdateFlip();
            caster.core.transform.SetPosition(movePos);
            caster.core.skill.PlayEffect(resScript.effect, caster.core.transform, 10);

            PlayTail(target, movePos);
            skill.core.finish.Finish();

            return true;
        }
    }
}