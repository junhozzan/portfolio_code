using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitComponent
{
    public class EnemyUnitDamagedComponent : UnitDamagedComponent
    {
        public EnemyUnitDamagedComponent(Unit owner) : base(owner)
        {

        }

        protected override void CallEvent(long damaged, bool firstHit, long newHp, long prevHp)
        {
            var arg = EnemyAttacked.Of(
                damaged,
                firstHit,
                owner.core.profile.tunit.uid,
                newHp,
                prevHp);

            GameEvent.Instance.AddEvent(GameEventType.ENEMY_ATTACKED, arg);
        }
    }
}