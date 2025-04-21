using UnityEngine;
using System;

namespace ModeComponent
{
    public class DamageModeEnemyComponent : ModeEnemyComponent
    {
        public DamageModeEnemyComponent(Mode mode) : base(mode)
        {

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

        protected override Type EnemySpawnType()
        {
            return typeof(OtherWorldUnit);
        }
    }
}