using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuffScript
{
    public class BuffScriptAttackToDamageUp : BuffScriptBase
    {
        private ResourceBuffScriptAttackToDamageUp resBuffScript = null;
        private int count = 0;

        public static BuffScriptAttackToDamageUp Of()
        {
            return new BuffScriptAttackToDamageUp();
        }

        public override void DoReset()
        {
            base.DoReset();
            count = 0;
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptAttackToDamageUp;
        }

        public override void Attack(ResourceSkillAttack resAttack, Unit to, float damage)
        {
            if (count >= resBuffScript.maxCount)
            {
                return;
            }

            ++count;
        }

        public override float GetAddDamageRatio(DamageType damageType, Unit to)
        {
            if (!resBuffScript.applyDamages.TryGetValue(damageType, out var v))
            {
                return 0f;
            }

            return count / resBuffScript.maxCount * v.ratio;
        }
    }
}