namespace UnitComponent
{
    public class EnemyUnitRefreshComponent : UnitRefreshComponent
    {
        private readonly new EnemyUnit owner = null;

        public EnemyUnitRefreshComponent(Unit owner) : base(owner)
        {
            this.owner = owner as EnemyUnit;
        }

        public override void RefreshByCreate()
        {
            owner.core.skin.customParts.Refresh();
            base.RefreshByCreate();
        }
    }
}