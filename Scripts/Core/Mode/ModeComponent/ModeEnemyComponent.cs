using System.Collections.Generic;
using System;

namespace ModeComponent
{
    public class ModeEnemyComponent : ModeBaseComponent
    {
        private readonly List<Unit> enemys = new List<Unit>(64);
        private int totalSpawnEnemyCount = 0;

        public ModeEnemyComponent(Mode mode) : base(mode)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.MODE_START, Handle_MODE_START)
                .Add(GameEventType.CHANGE_UNIT_COUNT, Handle_CHANGE_UNIT_COUNT)
                .Add(GameEventType.REMOVE_UNIT, Handle_REMOVE_UNIT)
                ;
        }

        private void Handle_MODE_START(object[] args)
        {
            totalSpawnEnemyCount = 0;
            ClearAllUnit();
        }

        private void Handle_CHANGE_UNIT_COUNT(object[] args)
        {
            foreach (var unit in enemys)
            {
                unit.core.target.ResetTargets();
            }
        }

        private void Handle_REMOVE_UNIT(object[] args)
        {
            var unit = GameEvent.GetSafe<Unit>(args, 0);
            if (unit == null)
            {
                return;
            }

            if (!IsSpawnedEnemy(unit))
            {
                return;
            }

            RemoveUnit(unit);
        }

        public override void OnDisable()
        {
            ClearAllUnit();
            base.OnDisable();
        }

        public void HandleRestart()
        {
            ClearAllUnit();
        }

        public void SpawnEnemyUnit()
        {
            var info = GetAppearEnemyInfo();
            var resUnit = ResourceManager.Instance.unit.GetUnit(info.id);
            var unit = UnitManager.Instance.Spawn(EnemySpawnType(), resUnit, GetEnemyLevel());
            unit.core.transform.CreateObject(mode.core.field.GetRandomGroundPosition());
            unit.core.refresh.RefreshByCreate();
            unit.core.profile.SetGrade(info.grade);
            unit.core.fof.SetTeam(Team.ENEMY);
            
            AddUnit(unit);
        }

        private void AddUnit(Unit unit)
        {
            if (!enemys.Contains(unit))
            {
                enemys.Add(unit);
            }
            
            ++totalSpawnEnemyCount;
            GameEvent.Instance.AddEvent(GameEventType.CHANGE_UNIT_COUNT);
        }

        private void RemoveUnit(Unit unit)
        {
            if (enemys.Contains(unit))
            {
                enemys.Remove(unit);
            }

            unit.OnDisable();
            GameEvent.Instance.AddEvent(GameEventType.CHANGE_UNIT_COUNT);
        }

        private void ClearAllUnit()
        {
            foreach (var unit in enemys)
            {
                if (!UnitRule.IsValid(unit))
                {
                    continue;
                }

                unit.OnDisable();
            }

            enemys.Clear();
            GameEvent.Instance.AddEvent(GameEventType.CHANGE_UNIT_COUNT);
        }

        /// <summary>
        /// 실시간 몬스터 수
        /// </summary>
        public int GetAliveEnemyCount()
        {
            return enemys.Count;
        }

        public int GetTotalSpawnEnemyCount()
        {
            return totalSpawnEnemyCount;
        }

        public virtual long GetEnemyLevel()
        {
            return 1;
        }

        public virtual (int id, UnitGrade grade) GetAppearEnemyInfo()
        {
            return (GameData.UNIT_DATA.ID_DEFAULT_MONSTER, UnitGrade.NONE);
        }

        public virtual long GetMaxSpawnCount()
        {
            var index = mode.core.profile.StageToIndex();
            if (!mode.core.profile.resMode.stages.TryGetValue(index, out var stage))
            {
                return 0;
            }

            return stage.maxSpawn;
        }

        public virtual int GetMaxFieldSpawnCount()
        {
            return mode.core.profile.resMode.maxFieldSpawnCount;
        }

        public bool IsSpawnedEnemy(Unit unit)
        {
            return enemys.Contains(unit);
        }

        protected virtual Type EnemySpawnType()
        {
            return typeof(EnemyUnit);
        }

        public Unit GetUnitByGrade(UnitGrade grade)
        {
            foreach (var unit in enemys)
            {
                if (unit.core.profile.grade != grade)
                {
                    continue;
                }

                return unit;
            }

            return null;
        }
    }
}