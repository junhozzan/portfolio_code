
namespace BuffScript
{
    public class BuffScriptAssistUnitOption : BuffScriptBase
    {
        private ResourceBuffScriptAssistUnitOption resBuffScript = null;

        public static BuffScriptAssistUnitOption Of()
        {
            return new BuffScriptAssistUnitOption();
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptAssistUnitOption;
        }

        public float GetAddOptionValue(int unitID, AssistUnitOptionType optionType)
        {
            if (!resBuffScript.unitIDs.Contains(unitID))
            {
                return 0f;
            }

            if (resBuffScript.optionType != optionType)
            {
                return 0f;
            }

            return resBuffScript.GetAddValue(buff.tbuff.GetLevel());
        }
    }
}