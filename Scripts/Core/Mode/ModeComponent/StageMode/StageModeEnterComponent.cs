namespace ModeComponent
{
    public class StageModeEnterComponent : ModeEnterComponent
    {
        public StageModeEnterComponent(StageMode mode) : base(mode)
        {

        }

        //protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        //{
        //    return base.CreateHandler()
        //        .Add(GameEventType.MODE_START, Handle_MODE_START)
        //        ;
        //}

        //protected void Handle_MODE_START(object[] args)
        //{
        //    var myUnit = mode.core.ally.myUnit;
        //    if (!UnitRule.IsValid(myUnit))
        //    {
        //        return;
        //    }

        //    var breakChance = myUnit.core.stat.GetValue(eAbility.DUN_BREAK_APPEAR_CHANCE);
        //    if (!Util.IsChance(breakChance))
        //    {
        //        return;
        //    }

        //    ModeManager.Instance.Enter(GameData.MODE_DATA.MODE_4_ID);
        //}
    }
}