using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BuffScript
{
    public class BuffScriptFreezingOption : BuffScriptBase
    {
        private ResourceBuffScriptFreezingOption resBuffScript = null;

        public static BuffScriptFreezingOption Of()
        {
            return new BuffScriptFreezingOption();
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptFreezingOption;
        }

        public float GetSpeed()
        {
            return resBuffScript.speed;
        }
    }
}