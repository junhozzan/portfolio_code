using System.Collections;
using UnityEngine;

namespace UnitComponent
{
    public class MyUnitManaComponent : UnitManaComponent
    {
        public MyUnitManaComponent(Unit owner) : base(owner)
        {

        }

        public override void RefreshMP(long max)
        {
            base.RefreshMP(max);
            owner.core.ui.SetMpSlider(GetMpRate());
            Changed();
        }

        public override void ResetMP(long max)
        {
            base.ResetMP(max);
            owner.core.ui.SetMpSlider(GetMpRate());
            Changed();
        }

        public override void SetMP(long value)
        {
            base.SetMP(value);
            Changed();
        }

        private void Changed()
        {
            GameEvent.Instance.AddEvent(GameEventType.CHANGE_MYUNIT_MP);
        }
    }
}