namespace UnitComponent
{
    public class OtherWorldUnitTargetComponent : UnitTargetComponent
    {
        public OtherWorldUnitTargetComponent(Unit owner) : base(owner)
        {

        }

        public override Unit GetPriorityTarget()
        {
            var mode = ModeManager.Instance.mode;
            if (mode == null)
            {
                return null;
            }

            return mode.core.ally.myUnit;
        }
    }
}