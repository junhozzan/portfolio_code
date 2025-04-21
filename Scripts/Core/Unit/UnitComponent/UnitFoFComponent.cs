namespace UnitComponent
{
    public class UnitFoFComponent : UnitBaseComponent
    {
        private Team team = Team.NONE;

        public UnitFoFComponent(Unit owner) : base(owner)
        {

        }

        public void SetTeam(Team team)
        {
            this.team = team;
        }

        public virtual Team GetTeam()
        {
            return team;
        }

        public virtual bool IsEenemy(Unit other)
        {
            return GetTeam() != other.core.fof.GetTeam();
        }

        public virtual bool IsMySelf(Unit other)
        {
            return owner.core.profile.tunit.uid == other.core.profile.tunit.uid;
        }
    }
}