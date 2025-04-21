using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuffScript
{
    /// <summary>
    /// 적을 처치할수록 대미지 증가
    /// </summary>
    public class BuffScriptKillToDamageUp : BuffScriptBase
    {
        private ResourceBuffScriptKillToDamageUp resBuffScript = null;
        private int count = 0;

        public static BuffScriptKillToDamageUp Of()
        {
            return new BuffScriptKillToDamageUp();
        }

        public override void DoReset()
        {
            base.DoReset();
            count = 0;
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptKillToDamageUp;
        }

        public override void Kill(Unit to)
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