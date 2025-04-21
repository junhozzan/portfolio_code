

namespace UnitComponent
{
    public class MyUnitStatComponent : UnitStatComponent
    {
        public readonly new MyUnit owner = null;

        public MyUnitStatComponent(Unit owner) : base(owner)
        {
            this.owner = owner as MyUnit;
        }

        protected override void BuildStatParams()
        {
            base.BuildStatParams();

            // 플레이어 기준 관련 스탯
            statParams.AddRange(MyPlayer.Instance.core.lab.GetStatItemParams(owner));
            statParams.AddRange(MyPlayer.Instance.core.collection.GetStatItemParams(owner));
        }

        public override void Refresh()
        {
            base.Refresh();
            GameEvent.Instance.AddEvent(GameEventType.MY_UNIT_STAT_UPDATED);
        }
    }
}