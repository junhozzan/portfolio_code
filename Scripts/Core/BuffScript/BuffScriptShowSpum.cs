namespace BuffScript
{
    public class BuffScriptShowSpum : BuffScriptBase
    {
        public ResourceBuffScriptShowSpum resBuffScript = null;

        public static BuffScriptShowSpum Of()
        {
            return new BuffScriptShowSpum();
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptShowSpum;
        }

        public override void On()
        {
            buff.owner.core.skin.RefreshSkin();

            var resSpum = ResourceManager.Instance.spum.GetSPUM(resBuffScript.GetSpumID(buff.tbuff.GetLevel()));
            if (resSpum == null)
            {
                return;
            }

            foreach (var type in resSpum.allParts.Keys)
            {
                buff.owner.core.skin.SetSkin(type, resSpum);
            }
        }

        public override void Off()
        {
            buff.owner.core.skin.RefreshSkin();
        }
    }
}