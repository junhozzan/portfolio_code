using System.Collections.Generic;
using System;

namespace UnitComponent
{
    public class UnitBuffRemoveComponent : UnitBaseComponent
    {
        private readonly List<int> removeBuffIDs = new List<int>();
        private readonly List<UnitBuff> removeBuffs = new List<UnitBuff>(4);
        private UnitBuffComponent buffCom = null;

        public UnitBuffRemoveComponent(Unit owner) : base(owner)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            buffCom = GetComponent<UnitBuffComponent>();
        }

        public override void DoReset()
        {
            base.DoReset();
            removeBuffs.Clear();
        }

        public override void OnDisable()
        {
            removeBuffs.Clear();
            base.OnDisable();
        }

        public void AddRemoveBuffID(int id)
        {
            if (removeBuffIDs.Contains(id))
            {
                return;
            }

            removeBuffIDs.Add(id);
        }

        public bool UpdateRemove()
        {
            if (removeBuffIDs.Count > 0)
            {
                foreach (var id in removeBuffIDs)
                {
                    var buff = buffCom.RemoveUnitBuff(id);
                    if (buff == null)
                    {
                        continue;
                    }

                    removeBuffs.Add(buff);
                }

                foreach (var buff in removeBuffs)
                {
                    buff.Off();
                }

                removeBuffIDs.Clear();
                removeBuffs.Clear();

                return true;
            }

            return false;
        }
    }
}