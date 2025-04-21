using System;
using UnityEngine;

namespace UnitComponent
{
    public class UnitManaComponent : UnitBaseComponent
    {
        public long maxMp { get; private set; } = 0L;
        public long mp { get; private set; } = 0L;

        public UnitManaComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            maxMp = 0L;
            mp = 0L;
        }

        public virtual void RefreshMP(long max)
        {
            // 최대 체력이 늘어나면 늘어난 만큼 체력 상승
            var prevMax = maxMp;
            maxMp = max;
            mp = Math.Min(mp + (maxMp - prevMax), maxMp);
        }

        public virtual void ResetMP(long max)
        {
            maxMp = mp = max;
        }

        public virtual void SetMP(long value)
        {
            mp = Math.Max(Math.Min(value, maxMp), 0L);
            owner.core.ui.SetMpSlider(GetMpRate());
        }

        public void AddMp(long value)
        {
            SetMP(mp + value);
        }

        public float GetMpRate()
        {
            return maxMp > 0 ? mp / (float)maxMp : 0f;
        }
    }
}