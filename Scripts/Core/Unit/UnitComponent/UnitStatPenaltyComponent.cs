using BuffScript;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace UnitComponent
{
    public class UnitStatPenaltyComponent : UnitBaseComponent
    {
        private float applyValue = 0f;

        private static readonly ReadOnlyCollection<eAbility> penaltyAbilities = new List<eAbility>()
        {
            eAbility.NONE,
            eAbility.POWER,
            eAbility.POWER_HOLY,
            eAbility.POWER_DARK,
            eAbility.DEFENCE,
            eAbility.MAXHP,
            eAbility.MAXMP,
            eAbility.CRITICAL_CHANCE,
            eAbility.CRITICAL_DAMAGE,
            eAbility.CRITICAL_RESIST,
        }.ReadOnly();

        public UnitStatPenaltyComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            applyValue = 0f;
        }

        public void Refresh()
        {
            var mode = ModeManager.Instance.mode;
            if (mode == null)
            {
                return;
            }

            var penalty = 0f;
            foreach (var buff in owner.core.buff.GetBuffs())
            {
                foreach (var script in buff.GetBuffScripts(BuffScriptType.MODE_PENALTY))
                {
                    if (script is BuffScriptModePenalty tScript)
                    {
                        if (mode.core.profile.resMode.id != tScript.resBuffScript.modeID)
                        {
                            continue;
                        }

                        penalty += tScript.resBuffScript.penalty;
                    }
                }
            }

            var recovery = 0f;
            applyValue = Mathf.Clamp(1f - (penalty * (1f - recovery)), 0f, 1f);
        }

        public float GetApplyValue(eAbility e)
        {
            if (!penaltyAbilities.Contains(e))
            {
                return 1f;
            }

            return applyValue;
        }
    }
}