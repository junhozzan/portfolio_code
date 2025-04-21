using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModeComponent
{
    public class KillModeEnemyComponent : ModeEnemyComponent
    {
        public KillModeEnemyComponent(Mode mode) : base(mode)
        {

        }

        public override long GetEnemyLevel()
        {
            return GetTotalSpawnEnemyCount() + 1;
        }

        public override (int id, UnitGrade grade) GetAppearEnemyInfo()
        {
            var index = mode.core.profile.StageToIndex();
            if (!mode.core.profile.resMode.stages.TryGetValue(index, out var stage))
            {
                return base.GetAppearEnemyInfo();
            }

            foreach (var units in stage.units)
            {
                return (units.Key, 0);
            }

            return base.GetAppearEnemyInfo();
        }
    }
}
