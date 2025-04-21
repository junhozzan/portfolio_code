using System.Collections.Generic;

namespace BuffScript
{
    public class BuffScriptAddStat : BuffScriptBase
    {
        public ResourceBuffScriptAddStat resBuffScript = null;

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptAddStat;
        }

        public static BuffScriptAddStat Of()
        {
            return new BuffScriptAddStat();
        }

        public override ICollection<ResourceTargetAbility> GetTargetAbilities()
        {
            return resBuffScript.targetAbilities;
        }
    }
}
