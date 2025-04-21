using UnityEngine;
using System.Collections.Generic;

namespace BuffScript
{
    public class BuffScriptKillToSpawnAssistUnit : BuffScriptSpawnUnit
    {
        private ResourceBuffScriptKillToAssistUnit resBuffScript = null;
        
        public static BuffScriptKillToSpawnAssistUnit Of()
        {
            return new BuffScriptKillToSpawnAssistUnit();
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptKillToAssistUnit;
        }

        public override void UpdateDt(float dt)
        {
            UpdateSpawnUnit();
        }

        private void UpdateSpawnUnit()
        {
            var level = buff.tbuff.GetLevel();
            if (!CheckConsumeMP(resBuffScript.GetConsumeMP(level)))
            {
                return;
            }

            if (units.Count >= resBuffScript.GetMaxSpawn(level))
            {
                return;
            }

            var killInfo = buff.owner.core.kill.Pop();
            if (killInfo == null)
            {
                return;
            }

            if (((Vector2)buff.owner.core.transform.GetPosition() - killInfo.position).sqrMagnitude > resBuffScript.sqrSpawnDistance)
            {
                // 거리가 멀면 스폰하지 않는다.
                return;
            }

            SpawnUnit(killInfo.resSpum, killInfo.scale, killInfo.skins, killInfo.position);

            // 사용한 정보는 삭제 한다.
            killInfo.OnDisable();
        }

        private void SpawnUnit(ResourceSPUM resSpum, float scale, ICollection<ResourceSPUM> skins, Vector3 position)
        {
            var resUnit = ResourceManager.Instance.unit.GetUnit(resBuffScript.unitID);
            if (resUnit == null)
            {
                return;
            }

            var unit = UnitManager.Instance.Spawn(typeof(AssistUnit), resUnit, buff.owner.core.profile.tunit.GetLevel()) as AssistUnit;
            unit.core.profile.SetSummoner(buff.owner);
            unit.core.skin.SetCustom(resSpum, scale, skins);
            unit.core.transform.CreateObject(position);
            unit.core.refresh.RefreshByCreate();

            SpawnedUnit(unit, resBuffScript.GetConsumeMP(buff.tbuff.GetLevel()));
        }
    }
}