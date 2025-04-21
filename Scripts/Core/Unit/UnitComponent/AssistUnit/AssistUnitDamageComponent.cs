
namespace UnitComponent
{
    public class AssistUnitDamageComponent : UnitDamageComponent
    {
        private readonly new AssistUnit owner = null;

        public AssistUnitDamageComponent(Unit owner) : base(owner)
        {
            this.owner = owner as AssistUnit;
        }

        public override void Attack(ResourceSkillAttack resAttack, Unit to, float damage)
        {
            base.Attack(resAttack, to, damage);

            if (owner.core.profile.summoner is MyUnit myUnit)
            {
                myUnit.core.damage.AddTotalDamage(damage);
            }
        }
    }
}