using System.Collections.Generic;
using UnityEngine;

namespace BuffScript
{
    public class BuffScriptTickSpawnAssistUnit : BuffScriptSpawnUnit
    {
        protected ResourceBuffScriptTickSpawnAssistUnit resBuffScript = null;

        protected readonly List<Unit> removeUnits = new List<Unit>();
        protected readonly Dictionary<long, float> spawnTimes = new Dictionary<long, float>();
        protected readonly Tick tick = new Tick();

        public static BuffScriptTickSpawnAssistUnit Of()
        {
            return new BuffScriptTickSpawnAssistUnit();
        }

        public override void Initialize()
        {
            base.Initialize();
            tick.SetCall(SpawnUnit);
        }

        public override void DoReset()
        {
            base.DoReset();
            tick.DoReset();
            spawnTimes.Clear();
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptTickSpawnAssistUnit;

            tick.SetResource(this.resBuffScript.startDelay, this.resBuffScript.interval);
        }

        public override void On()
        {
            tick.On();
        }

        public override void UpdateDt(float dt)
        {
            UpdateEndSpawn(dt);
            tick.UpdateDt(dt);
        }

        private void UpdateEndSpawn(float dt)
        {
            removeUnits.Clear();

            foreach (var unit in units)
            {
                spawnTimes[unit.core.profile.tunit.uid] -= dt;
                if (spawnTimes[unit.core.profile.tunit.uid] > 0)
                {
                    continue;
                }

                removeUnits.Add(unit);
            }

            if (removeUnits.Count > 0)
            {
                foreach (var unit in removeUnits)
                {
                    RemoveUnit(unit);
                }

                removeUnits.Clear();
            }
        }

        protected virtual void SpawnUnit()
        {
            if (!CheckConsumeMP(resBuffScript.consumMp))
            {
                return;
            }

            var resUnit = ResourceManager.Instance.unit.GetUnit(resBuffScript.unitID);
            var unit = UnitManager.Instance.Spawn(typeof(AssistUnit), resUnit, buff.owner.core.profile.tunit.GetLevel()) as AssistUnit;
            unit.core.profile.SetSummoner(buff.owner);
            unit.core.transform.CreateObject(GetRoundPosition(buff.owner.core.transform.GetPosition()));
            unit.core.refresh.RefreshByCreate();

            spawnTimes[unit.core.profile.tunit.uid] = resBuffScript.GetSpawnTime(buff.tbuff.GetLevel());

            SpawnedUnit(unit, resBuffScript.consumMp);
        }

        public static Vector3 GetRoundPosition(Vector3 position)
        {
            var angle = Util.AddAngle(Vector3.right, Random.Range(0f, 360f));
            return position + (Vector3)(angle * Env.Distance(40f));
        }
    }
}
