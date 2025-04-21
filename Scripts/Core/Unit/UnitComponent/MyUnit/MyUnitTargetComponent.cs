using System.Collections.Generic;

namespace UnitComponent
{
    public class MyUnitTargetComponent : UnitTargetComponent
    {
        private readonly List<long> ignoreTargetUIDs = new List<long>();

        public MyUnitTargetComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            ignoreTargetUIDs.Clear();
        }

        public override List<long> GetIgnoreTargetUIDs()
        {
            var enemy = ModeManager.Instance.mode.core.enemy;
            // 보스는 항상 하나.
            if (enemy.GetAliveEnemyCount() > 1)
            {
                var gradeUnit = enemy.GetUnitByGrade(UnitGrade.BOSS);
                if (UnitRule.IsValid(gradeUnit))
                {
                    ignoreTargetUIDs.Clear();
                    ignoreTargetUIDs.Add(gradeUnit.core.profile.tunit.uid);
                    return ignoreTargetUIDs;
                }
            }

            return base.GetIgnoreTargetUIDs();
        }
    }
}