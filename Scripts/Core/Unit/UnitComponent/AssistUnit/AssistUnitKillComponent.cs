namespace UnitComponent
{
    public class AssistUnitKillComponent : UnitKillComponent
    {
        private readonly new AssistUnit owner = null;

        public AssistUnitKillComponent(Unit owner) : base(owner)
        {
            this.owner = owner as AssistUnit;
        }

        public override void Add(Unit unit)
        {
            base.Add(unit);

            owner.core.profile.summoner.core.kill.Add(unit);
        }
    }
}