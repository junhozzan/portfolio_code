using System.Collections.Generic;
using UnityEngine;

namespace UnitComponent
{
    public class UnitSpawnComponent : UnitBaseComponent
    {
        public readonly List<Unit> units = new List<Unit>();

        public UnitSpawnComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            units.Clear();
        }

        public void SetToZero(Vector3 pos)
        {
            foreach (var unit in units)
            {
                unit.core.transform.SetPosition(unit.core.transform.GetPosition() - pos);
            }
        }

        public void AddUnit(Unit unit)
        {
            if (units.Contains(unit))
            {
                return;
            }

            units.Add(unit);
        }

        public void AddUnits(ICollection<Unit> units)
        {
            foreach (var unit in units)
            {
                AddUnit(unit);
            }
        }

        public void RemoveUnit(Unit unit)
        {
            if (!units.Contains(unit))
            {
                return;
            }

            units.Remove(unit);
        }
    }
}