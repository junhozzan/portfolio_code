namespace UnitComponent
{
    public class UnitRefreshComponent : UnitBaseComponent
    {
        protected bool isRefresh = false;

        public UnitRefreshComponent(Unit owner) : base(owner)
        {

        }

        public virtual void RefreshByCreate()
        {
            owner.core.ai.Refresh();
            owner.core.fade.Refresh();
            owner.core.skin.ResetRenderers();
            owner.core.skin.RefreshSkin();
            owner.core.skin.RefreshScale();
            owner.core.ui.CreateUnitUI();
            owner.core.kill.ClearInfos();

            RefreshAbility();
            owner.core.health.ResetHP(owner.core.stat.GetLongValue(eAbility.MAXHP));
            owner.core.mana.ResetMP(owner.core.stat.GetLongValue(eAbility.MAXMP));
        }

        public void NextRefresh()
        {
            isRefresh = true;
        }

        public void RefreshImmediate()
        {
            isRefresh = false;
            RefreshByUpdate();
        }

        private void RefreshByUpdate()
        {
            RefreshAbility();
            owner.core.health.RefreshHP(owner.core.stat.GetLongValue(eAbility.MAXHP));
            owner.core.mana.RefreshMP(owner.core.stat.GetLongValue(eAbility.MAXMP));
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            if (!isRefresh)
            {
                return;
            }

            isRefresh = false;
            RefreshByUpdate();
        }

        protected void RefreshAbility()
        {
            owner.core.buff.Refresh();
            owner.core.skill.Refresh();
            owner.core.stat.Refresh();
            owner.core.move.Refresh();

            foreach (var unit in owner.core.spawn.units)
            {
                unit.core.refresh.RefreshAbility();
            }
        }
    }
}