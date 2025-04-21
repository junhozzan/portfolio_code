using System.Collections.Generic;

namespace BuffScript
{
    public class BuffScriptSkillBonus : BuffScriptBase
    {
        private ResourceBuffScriptSkillBonus resBuffScript = null;

        public static BuffScriptSkillBonus Of()
        {
            return new BuffScriptSkillBonus();
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptSkillBonus;
        }

        public override ICollection<ResourceTargetAbility> GetTargetAbilities()
        {
            return resBuffScript.targetAbilities;
        }
    }
}
