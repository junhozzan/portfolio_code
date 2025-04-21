using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitComponent
{
    public class EnemyUnitDeadComponent : UnitDeadComponent
    {
        private readonly new EnemyUnit owner = null;

        public EnemyUnitDeadComponent(Unit owner) : base(owner)
        {
            this.owner = owner as EnemyUnit;
        }

        protected override void Dead()
        {
            base.Dead();
            owner.core.skin.BlackOut();
            owner.core.transform.DropObject();
        }

        protected override void DeadEnd()
        {
            base.DeadEnd();
            GameEvent.Instance.AddEvent(GameEventType.REMOVE_UNIT, owner);
        }
    }
}