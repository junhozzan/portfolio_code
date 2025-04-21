namespace UnitComponent
{
    public class MyUnitSpawnComponent : UnitSpawnComponent
    {
        public MyUnitSpawnComponent(Unit owner) : base(owner)
        {

        }

        public void SetTrailEffect()
        {
            foreach (var unit in units)
            {
                if (unit is AssistUnit assistUnit)
                {
                    assistUnit.core.transform.SetTrailEffect();
                }
            }
        }

        public void RemoveTrailEffect()
        {
            foreach (var unit in units)
            {
                if (unit is AssistUnit assistUnit)
                {
                    assistUnit.core.transform.ReturnTrailEffect();
                }
            }
        }
    }
}