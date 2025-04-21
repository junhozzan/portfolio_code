namespace BuffScript
{
    public class BuffScriptAddSkill : BuffScriptBase
    {
        public ResourceBuffScriptAddSkill resBuffScript = null;

        public static BuffScriptAddSkill Of()
        {
            return new BuffScriptAddSkill();
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptAddSkill;
        }

        public int GetSkillID(long level)
        {
            return resBuffScript.GetSkillID(level);
        }
    }
}
