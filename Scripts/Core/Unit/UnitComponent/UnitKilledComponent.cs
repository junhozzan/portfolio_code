using System.Collections;
using UnityEngine;

namespace UnitComponent
{
    public class UnitKilledComponent : UnitBaseComponent
    {
        public UnitKilledComponent(Unit owner) : base(owner)
        {

        }

        public void Add(Unit unit)
        {
            owner.core.buff.HandleKilled(unit);
        }
    }
}