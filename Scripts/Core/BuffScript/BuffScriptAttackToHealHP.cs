namespace BuffScript
{
    public class BuffScriptAttackToHealHP : BuffScriptBase
    {
        private ResourceBuffScriptAttackToHealHP resBuffScript = null;

        public static BuffScriptAttackToHealHP Of()
        {
            return new BuffScriptAttackToHealHP();
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptAttackToHealHP;
        }

        public override void Attack(ResourceSkillAttack resAttack, Unit to, float damage)
        {
            if (!UnitRule.IsAlive(buff.owner))
            {
                return;
            }

            var addValue = (long)(buff.owner.core.health.maxHp * resBuffScript.rate);
            buff.owner.core.health.AddHP(addValue);
        }
    }
}