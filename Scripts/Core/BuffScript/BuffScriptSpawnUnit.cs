using System.Collections.Generic;

namespace BuffScript
{
    public abstract class BuffScriptSpawnUnit : BuffScriptBase
    {
        protected readonly List<Unit> units = new List<Unit>();

        public override void Initialize()
        {
            CreateHandler();
        }

        public override void DoReset()
        {
            base.DoReset();
            foreach (var unit in units)
            {
                if (!UnitRule.IsValid(unit))
                {
                    continue;
                }

                unit.OnDisable();
            }

            units.Clear();
        }

        protected virtual EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsUsed)
                .Add(GameEventType.REMOVE_UNIT, Handle_REMOVE_UNIT)
                ;
        }

        private void Handle_REMOVE_UNIT(object[] args)
        {
            var unit = GameEvent.GetSafe<Unit>(args, 0);
            if (unit == null)
            {
                return;
            }

            if (!units.Contains(unit))
            {
                return;
            }

            RemoveUnit(unit);
        }

        protected bool CheckConsumeMP(long consumeMp)
        {
            return buff.owner.core.mana.mp >= consumeMp;
        }

        protected void SpawnedUnit(Unit unit, long consumeMp)
        {
            units.Add(unit);
            buff.owner.core.spawn.AddUnit(unit);
            buff.owner.core.mana.AddMp(-consumeMp);

            GameEvent.Instance.AddEvent(GameEventType.CHANGE_UNIT_COUNT);
        }

        protected void RemoveUnit(Unit unit)
        {
            unit.OnDisable();

            buff.owner.core.spawn.RemoveUnit(unit);
            units.Remove(unit);
        }

        public override void Off()
        {
            ClearUnits();
        }

        public override void OnDisable()
        {
            ClearUnits();
            base.OnDisable();
        }

        private void ClearUnits()
        {
            foreach (var unit in units)
            {
                buff.owner.core.spawn.RemoveUnit(unit);

                if (!UnitRule.IsValid(unit))
                {
                    continue;
                }

                unit.OnDisable();
            }

            units.Clear();
        }
    }
}