using System.Collections;
using UnityEngine;

namespace UnitComponent
{
    public class AssistUnitRefreshComponent : UnitRefreshComponent
    {
        private readonly new AssistUnit owner = null;

        public AssistUnitRefreshComponent(Unit owner) : base(owner)
        {
            this.owner = owner as AssistUnit;
        }

        public override void RefreshByCreate()
        {
            base.RefreshByCreate();
            owner.core.skin.SetBlack();
        }
    }
}