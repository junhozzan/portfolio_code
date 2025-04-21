namespace UnitComponent
{
    public class EnemyUnitTransformComponent : UnitTransformComponent
    {

        public EnemyUnitTransformComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            unitObj = null;
        }
    }
}