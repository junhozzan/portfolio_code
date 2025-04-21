using UnityEngine;

namespace UnitComponent
{
    public class AssistUnitStatComponent : UnitStatComponent
    {
        private readonly new AssistUnit owner = null;

        public AssistUnitStatComponent(Unit owner) : base(owner)
        {
            this.owner = owner as AssistUnit;
        }

        protected override void BuildStatParams()
        {
            // empty
        }

        public override float GetValue(eAbility e)
        {
            var value = owner.core.profile.summoner.core.stat.GetValue(e);
            switch (e)
            {
                case eAbility.POWER_DARK:
                case eAbility.DEFENCE:
                case eAbility.MAXHP:
                    return Mathf.Lerp(0f, value, Mathf.Clamp01(owner.core.profile.GetOptionValue(AssistUnitOptionType.STAT_SYNC)));

                case eAbility.MOVESPEED:
                    // 그림자 몬스터는 군주보다 빠르게 이동
                    return value * 1.2f;
            }

            return 0f;
        }
    }
}