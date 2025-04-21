namespace UnitComponent
{
    public class MyUnitRefreshComponent : UnitRefreshComponent
    {
        private readonly new MyUnit owner = null;

        public MyUnitRefreshComponent(Unit owner) : base(owner)
        {
            this.owner = owner as MyUnit;
        }

        public void RefreshByStart()
        {
            owner.core.fade.ResetFadeState();
            owner.core.skin.ResetRenderers();
            owner.core.anim.Resurrection();
            owner.core.dead.Resurrection();
            owner.core.damage.ClearInfos();
            owner.core.kill.ClearInfos();

            RefreshAbility();

            var isRecovery = true;
            var mode = ModeManager.Instance.mode;
            if (mode != null)
            {
                isRecovery = mode.core.ally.IsStartRecovery();
            }

            if (isRecovery)
            {
                owner.core.health.ResetHP(owner.core.stat.GetLongValue(eAbility.MAXHP));
                owner.core.mana.ResetMP(owner.core.stat.GetLongValue(eAbility.MAXMP));
                foreach (var unit in owner.core.spawn.units)
                {
                    unit.core.health.ResetHP(unit.core.stat.GetLongValue(eAbility.MAXHP));
                    unit.core.mana.ResetMP(unit.core.stat.GetLongValue(eAbility.MAXMP));
                }
            }
            else
            {
                owner.core.health.RefreshHP(owner.core.stat.GetLongValue(eAbility.MAXHP));
                owner.core.mana.RefreshMP(owner.core.stat.GetLongValue(eAbility.MAXMP));
                foreach (var unit in owner.core.spawn.units)
                {
                    unit.core.health.RefreshHP(unit.core.stat.GetLongValue(eAbility.MAXHP));
                    unit.core.mana.RefreshMP(unit.core.stat.GetLongValue(eAbility.MAXMP));
                }
            }
        }
    }
}