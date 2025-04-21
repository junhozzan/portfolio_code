using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuffScript
{
    public class BuffScriptStatus : BuffScriptBase
    {
        private ResourceBuffScriptStatus resBuffScript = null;

        public static BuffScriptStatus Of()
        {
            return new BuffScriptStatus();
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptStatus;
        }

        public override void On()
        {
            Refresh();
        }

        public override void Off()
        {
            Refresh();
        }

        private void Refresh()
        {
            buff.owner.core.status.Refresh(resBuffScript.statusType);
        }

        public StatusType GetStatusType()
        {
            return resBuffScript.statusType;
        }
    }
}