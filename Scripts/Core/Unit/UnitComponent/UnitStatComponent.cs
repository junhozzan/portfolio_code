using System.Collections.Generic;

namespace UnitComponent
{
    public class UnitStatComponent : UnitBaseComponent
    {
        protected readonly List<StatItem.Param> statParams = new List<StatItem.Param>();
        protected readonly Stat stat = Stat.Of();
        protected readonly Ability ability = Ability.Of();

        private readonly UnitStatPenaltyComponent penalty = null;

        public UnitStatComponent(Unit owner) : base(owner)
        {
            penalty = AddComponent<UnitStatPenaltyComponent>(owner);
        }

        public override void DoReset()
        {
            base.DoReset();
            stat.DoReset();
            ability.DoReset();
        }

        protected virtual void BuildStatParams()
        {
            statParams.Clear();
            statParams.AddRange(owner.core.profile.stat.GetStatItemParams());
            statParams.AddRange(owner.core.buff.stat.GetStatItemParams());
            statParams.AddRange(owner.core.item.stat.GetStatItemParams());
        }

        public virtual void Refresh()
        {
            BuildStatParams();

            stat.DoReset();
            ability.DoReset();

            foreach (var statParam in statParams)
            {
                stat.AddStat(statParam.statItem, statParam.riseParam, statParam.rate);
            }

            ability.CalcAbilty(stat);
            penalty.Refresh();
        }

        public Stat GetStat()
        {
            return stat;
        }

        public Ability GetAbility()
        {
            return ability;
        }

        public virtual float GetValue(eAbility e)
        {
            return ability.Get(e) * penalty.GetApplyValue(e);
        }

        public long GetLongValue(eAbility e)
        {
            return (long)GetValue(e);
        }

#if UNITY_EDITOR
        private static System.Text.StringBuilder sbDebugState = new System.Text.StringBuilder();
        public string DebugStatString()
        {
            sbDebugState.Clear();

            foreach (var type in Stat.types)
            {
                sbDebugState.AppendFormat($"{type}:[{stat.Get(type).ToString("0.##")}]\n");
            }

            return sbDebugState.ToString();
        }

        public string DebugAbilityString()
        {
            sbDebugState.Clear();
            for (eAbility e = eAbility.POWER; e < eAbility.CNT; ++e)
            {
                sbDebugState.AppendFormat($"{e}:[{GetValue(e).ToString("0.##")}]\n");
            }

            return sbDebugState.ToString();
        }

        public string DebugPenaltyString()
        {
            return $"penalty:[{(1f - penalty.GetApplyValue(eAbility.NONE)) * 100f}%]\n";
        }
#endif
    }
}