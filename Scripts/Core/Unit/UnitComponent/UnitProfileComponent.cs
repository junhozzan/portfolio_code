using UnityEngine;

namespace UnitComponent
{
    public class UnitProfileComponent : UnitBaseComponent
    {
        public TUnit tunit { get; private set; } = null;
        public UnitGrade grade { get; private set; } = 0;

        public readonly UnitProfileStatComponent stat = null;

        public UnitProfileComponent(Unit owner) : base(owner)
        {
            stat = AddComponent<UnitProfileStatComponent>(owner);
        }

        public override void DoReset()
        {
            base.DoReset();
            tunit = null;
            grade = UnitGrade.NONE;
        }

        public override void OnDisable()
        {
            tunit?.OnDisable();
            tunit = null;

            base.OnDisable();
        }

        public void SetTUnit(TUnit _tunit)
        {
            if (tunit != null)
            {
                tunit.OnDisable();
            }

            tunit = _tunit;
            owner.core.item.HandleTUnit();
        }

        public void SetGrade(UnitGrade grade)
        {
            this.grade = grade;
        }
    }
}