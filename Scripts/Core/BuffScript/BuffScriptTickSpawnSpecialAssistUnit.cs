using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BuffScript
{
    public class BuffScriptTickSpawnSpecialAssistUnit : BuffScriptTickSpawnAssistUnit
    {
        private static readonly ReadOnlyCollection<ResourceSPUM> empty = new List<ResourceSPUM>().ReadOnly();

        public static new BuffScriptTickSpawnSpecialAssistUnit Of()
        {
            return new BuffScriptTickSpawnSpecialAssistUnit();
        }

        protected override void SpawnUnit()
        {
            if (!CheckConsumeMP(resBuffScript.consumMp))
            {
                return;
            }

            var resUnit = ResourceManager.Instance.unit.GetUnit(resBuffScript.unitID);
            var unit = UnitManager.Instance.Spawn(typeof(AssistSpecialUnit), resUnit, buff.owner.core.profile.tunit.GetLevel()) as AssistSpecialUnit;
            unit.core.profile.SetSummoner(buff.owner);
            unit.core.skin.SetCustom(null, 1f, GetCustomSkins());
            unit.core.transform.CreateObject(GetRoundPosition(buff.owner.core.transform.GetPosition()));
            unit.core.refresh.RefreshByCreate();

            spawnTimes[unit.core.profile.tunit.uid] = resBuffScript.GetSpawnTime(buff.tbuff.GetLevel());
         
            SpawnedUnit(unit, resBuffScript.consumMp);
        }

        private ICollection<ResourceSPUM> GetCustomSkins()
        {
            var mode = ModeManager.Instance.mode;
            if (mode == null)
            {
                return empty;
            }

            var equipedItemIDs = MyPlayer.Instance.core.mode.rank.GetTopRankerItemIDs(mode.core.profile.resMode.id);
            if (equipedItemIDs != null)
            {
                return empty;
            }

            return mode.core.ally.myUnit.core.skin.GetCustomSkins();
        }
    }
}
