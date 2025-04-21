namespace UnitComponent
{
    public class AssistUnitFoFComponent : UnitFoFComponent
    {
        public readonly new AssistUnit owner = null;

        public AssistUnitFoFComponent(Unit owner) : base(owner)
        {
            this.owner = owner as AssistUnit;
        }

        public override Team GetTeam()
        {
            if (owner.core.profile.summoner == null)
            {
                return Team.NONE;
            }

            return owner.core.profile.summoner.core.fof.GetTeam();
        }
    }
}