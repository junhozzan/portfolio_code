namespace UnitComponent
{
    public class MyUnitHealthComponent : UnitHealthComponent
    {
        public MyUnitHealthComponent(Unit owner) : base(owner)
        {

        }

        public override void RefreshHP(long max)
        {
            base.RefreshHP(max);
            owner.core.ui.SetHpSlider(GetHpRate());
            Changed();
        }

        public override void ResetHP(long max)
        {
            base.ResetHP(max);
            owner.core.ui.SetHpSlider(GetHpRate());
            Changed();
        }

        public override void SetHP(long value)
        {
            base.SetHP(value);
            Changed();
        }

        private void Changed()
        {
            GameEvent.Instance.AddEvent(GameEventType.CHANGE_MYUNIT_HP);
        }
    }
}