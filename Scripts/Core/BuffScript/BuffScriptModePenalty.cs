namespace BuffScript
{
    public class BuffScriptModePenalty : BuffScriptBase
    {
        public ResourceBuffScriptModePenalty resBuffScript = null;

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptModePenalty;
        }

        public static BuffScriptModePenalty Of()
        {
            return new BuffScriptModePenalty();
        }
    }
}