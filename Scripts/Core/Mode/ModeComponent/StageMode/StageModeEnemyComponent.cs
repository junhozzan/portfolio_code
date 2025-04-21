using System.Collections.Generic;
using UnityEngine;

namespace ModeComponent
{
    public class StageModeEnemyComponent : ModeEnemyComponent
    {
        protected readonly Queue<(int, UnitGrade)> popEnemyUnits = new Queue<(int, UnitGrade)>();

        public StageModeEnemyComponent(StageMode mode) : base(mode)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.MODE_START, Handle_MODE_START)
                ;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            popEnemyUnits.Clear();
        }

        protected void Handle_MODE_START(object[] args)
        {
            RefreshPopMonsterUnitIDs();
        }

        protected virtual void RefreshPopMonsterUnitIDs()
        {
            popEnemyUnits.Clear();

            var index = mode.core.profile.StageToIndex();
            if (!mode.core.profile.resMode.stages.TryGetValue(index, out var stage))
            {
                return;
            }

            foreach (var units in stage.units)
            {
                for (int i = 0; i < units.Value.Item1; ++i)
                {
                    popEnemyUnits.Enqueue((units.Key, units.Value.Item2));
                }
            }
        }

        public override long GetEnemyLevel()
        {
            var score = mode.core.score.GetScore();
            return (long)Mathf.Max(Mathf.Pow(score, GameData.DEFAULT.STAGE_ENEMY_LEVEL_VALUE), 1);
        }

        public override (int id, UnitGrade grade) GetAppearEnemyInfo()
        {
            if (popEnemyUnits.Count == 0)
            {
                return base.GetAppearEnemyInfo();
            }

            var unit = popEnemyUnits.Dequeue();
            var idx = (int)mode.core.profile.GetStage() % mode.core.profile.resMode.overrideUnits.Count;

            var overrideID = unit.Item1;
            if (mode.core.profile.resMode.overrideUnits.Count > 0
                && mode.core.profile.resMode.overrideUnits[idx].TryGetValue(unit.Item1, out var v)
                && v.Count > 0)
            {
                overrideID = v[Random.Range(0, v.Count)];
            }

            return (overrideID, unit.Item2);
        }
    }
}