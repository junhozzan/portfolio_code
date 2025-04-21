
namespace UnitComponent
{
    public class UnitDamageComponent : UnitBaseComponent
    {
        public UnitDamageComponent(Unit owner) : base(owner)
        {

        }

        public virtual void Attack(ResourceSkillAttack resAttack, Unit to, float damage)
        {
            if (!UnitRule.IsValid(to))
            {
                return;
            }

            owner.core.buff.HandleAttack(resAttack, to, damage);
        }
    }
}