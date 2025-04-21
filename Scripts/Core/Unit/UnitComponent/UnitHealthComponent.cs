using System;

namespace UnitComponent
{
    public class UnitHealthComponent : UnitBaseComponent
    {
        public long maxHp { get; private set; } = 0L;
        public long hp { get; private set; } = 0L;

        public UnitHealthComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            maxHp = 0L;
            hp = 0L;
        }

        public virtual void RefreshHP(long max)
        {
            // 최대 체력이 늘어나면 늘어난 만큼 체력 상승
            var prevMax = maxHp;
            maxHp = max;
            hp = Math.Min(hp + (maxHp - prevMax), maxHp);
        }

        public virtual void ResetHP(long max)
        {
            maxHp = hp = max;
        }

        public virtual void SetHP(long value)
        {
            hp = Math.Max(Math.Min(value, maxHp), 0L);
            owner.core.ui.SetHpSlider(GetHpRate());
        }

        public void AddHP(long value)
        {
            SetHP(hp + value);
        }

        public float GetHpRate()
        {
            return maxHp > 0 ? hp / (float)maxHp : 0f;
        }

    }
}