using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuffScript
{
    public class BuffScriptEscarnor : BuffScriptBase
    {
        private ResourceBuffScriptEscanor resBuffScript = null;

        public static BuffScriptEscarnor Of()
        {
            return new BuffScriptEscarnor();
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptEscanor;
        }

        public override float GetAddDamageRatio(DamageType damageType, Unit to)
        {
            if (!resBuffScript.applyDamages.TryGetValue(damageType, out var v))
            {
                return 0f;
            }

            var now = Main.Instance.time.now;
            var nowHms = now.Hour * 3600 + now.Minute * 60 + now.Second;
            var m = (nowHms / 60) * 60; // 분단위로 적용

            return v.ratio * (1f - Mathf.Clamp01(Mathf.Abs(resBuffScript.at0_43200 - resBuffScript.Convert0_43200(m)) / 43200f));
        }
    }
}