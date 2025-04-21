
using BuffScript;
using System.Collections.Generic;

namespace UnitComponent
{
    public class AssistUnitProfileComponent : UnitProfileComponent
    {
        public Unit summoner { get; private set; } = null;

        private readonly Dictionary<AssistUnitOptionType, float> optionValues = new Dictionary<AssistUnitOptionType, float>();

        public AssistUnitProfileComponent(Unit owner) : base(owner)
        {

        }

        public void SetSummoner(Unit unit)
        {
            summoner = unit;
            optionValues.Clear();

            var buffs = summoner.core.buff.buffScript.GetBuffsByScriptType(BuffScriptType.ASSIST_UNIT_OPTION);
            for (AssistUnitOptionType e = AssistUnitOptionType.STAT_SYNC; e < AssistUnitOptionType.COUNT; ++e)
            {
                var value = 0f;
                foreach (var buff in buffs)
                {
                    foreach (var script in buff.GetBuffScripts(BuffScriptType.ASSIST_UNIT_OPTION))
                    {
                        if (script is BuffScriptAssistUnitOption tScript)
                        {
                            value += tScript.GetAddOptionValue(owner.core.profile.tunit.resUnit.id, e);
                        }
                    }
                }

                optionValues.Add(e, value);
            }
        }

        public float GetOptionValue(AssistUnitOptionType optionType)
        {
            return optionValues.TryGetValue(optionType, out var v) ? v : 0f;
        }
    }
}