using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuffScript
{
    public class BuffScriptAttackToSkill : BuffScriptBase
    {
        private ResourceBuffScriptAttackToSkill resBuffScript = null;

        public static BuffScriptAttackToSkill Of()
        {
            return new BuffScriptAttackToSkill();
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptAttackToSkill;
        }

        public override void Attack(ResourceSkillAttack resAttack, Unit to, float damage)
        {
            var buffLevel = buff.tbuff.GetLevel();
            var resSkill = ResourceManager.Instance.skill.GetSkill(resBuffScript.GetSkillID(buffLevel));
            if (resSkill == null)
            {
                return;
            }

            if (resAttack.parent.id == resSkill.id)
            {
                // 같은 스킬인 경우 리턴
                return;
            }

            if (resAttack.parent.skillType != SkillType.DEFAULT_ATTACK)
            {
                // 스킬 공격이면 리턴
                return;
            }

            if (!Util.IsChance(resBuffScript.GetChance(buffLevel)))
            {
                return;
            }

            buff.owner.core.skill.UseSkill(to, resSkill);
        }
    }
}