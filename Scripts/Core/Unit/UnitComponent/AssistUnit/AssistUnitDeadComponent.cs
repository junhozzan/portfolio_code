using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitComponent
{
    public class AssistUnitDeadComponent : UnitDeadComponent
    {
        private readonly new AssistUnit owner = null;

        public AssistUnitDeadComponent(Unit owner) : base(owner)
        {
            this.owner = base.owner as AssistUnit;
        }

        protected override void Dead()
        {
            base.Dead();
            owner.core.skin.Out();
            owner.core.transform.DropObject();
        }

        protected override void DeadEnd()
        {
            base.DeadEnd();
            GameEvent.Instance.AddEvent(GameEventType.REMOVE_UNIT, owner);
        }
    }
}