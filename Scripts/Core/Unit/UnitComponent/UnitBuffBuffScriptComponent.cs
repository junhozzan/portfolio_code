using System.Collections.Generic;
using UnityEngine;

namespace UnitComponent
{
    public class UnitBuffBuffScriptComponent : UnitBaseComponent
    {
        private readonly Dictionary<BuffScriptType, List<UnitBuff>> buffsByScriptType = new Dictionary<BuffScriptType, List<UnitBuff>>();
        private static readonly List<UnitBuff> emptyList = new List<UnitBuff>();

        public UnitBuffBuffScriptComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            ResetBuffScript();
        }

        public override void OnDisable()
        {
            ResetBuffScript();
            base.OnDisable();
        }

        private void ResetBuffScript()
        {
            foreach (var list in buffsByScriptType.Values)
            {
                list.Clear();
            }
        }

        public void AddBuffScript(UnitBuff buff)
        {
            foreach (var resBuffScript in buff.tbuff.resBuff.resBuffScripts)
            {
                if (!buffsByScriptType.TryGetValue(resBuffScript.type, out var list))
                {
                    buffsByScriptType.Add(resBuffScript.type, list = new List<UnitBuff>());
                }

                if (list.Contains(buff))
                {
                    continue;
                }

                list.Add(buff);
            }
        }

        public void RemoveBuffScript(UnitBuff buff)
        {
            foreach (var resBuffScript in buff.tbuff.resBuff.resBuffScripts)
            {
                if (!buffsByScriptType.TryGetValue(resBuffScript.type, out var list))
                {
                    continue;
                }

                if (!list.Contains(buff))
                {
                    continue;
                }

                list.Remove(buff);
            }
        }

        public IEnumerable<UnitBuff> GetBuffsByScriptType(BuffScriptType type)
        {
            if (!buffsByScriptType.TryGetValue(type, out var list))
            {
                return emptyList;
            }

            return list;
        }
    }
}