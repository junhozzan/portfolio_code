using System;

namespace UnitComponent
{
    public class MyUnitDamageComponent : UnitDamageComponent
    {
        private float totalDamage = 0f;
        private float totalSec = 0f;
        private long savedDps = 0;

        public MyUnitDamageComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            savedDps = 0;
            ClearInfos();
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            totalSec += dt;
        }

        public void ClearInfos()
        {
            totalDamage = 0f;
            totalSec = 0f;
        }

        public override void Attack(ResourceSkillAttack resAttack, Unit to, float damage)
        {
            base.Attack(resAttack, to, damage);
            AddTotalDamage(damage);
        }

        public void AddTotalDamage(float damage)
        {
            totalDamage += damage;

            if (totalSec >= 1f)
            {
                savedDps = (long)(totalDamage / Math.Max(totalSec, 1f));
            }
        }

        public string GetDpsString()
        {
            return Util.ToComma(savedDps);
        }
    }
}