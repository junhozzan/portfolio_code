using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitComponent
{
    public class EnemyUnitTargetComponent : UnitTargetComponent
    {
        private readonly List<long> ignoreTargetUIDs = new List<long>();

        public EnemyUnitTargetComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            ignoreTargetUIDs.Clear();
        }

        // 추후 옵션으로 빼자
        public override List<long> GetIgnoreTargetUIDs()
        {
            var ally = ModeManager.Instance.mode.core.ally;
            var myUnit = ally.myUnit;
            if (UnitRule.IsValid(myUnit))
            {
                if (myUnit.core.spawn.units.Count > 0)
                {
                    ignoreTargetUIDs.Clear();
                    ignoreTargetUIDs.Add(myUnit.core.profile.tunit.uid);
                    return ignoreTargetUIDs;
                }
            }

            return base.GetIgnoreTargetUIDs();
        }
    }
}