using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitComponent
{
    public class UnitBuffLabBuffComponent : UnitBaseComponent
    {
        private readonly Dictionary<int, (ResourceLab, long)> addBuffs = new Dictionary<int, (ResourceLab, long)>();
        private UnitBuffComponent buffCom = null;

        private static readonly Type resLabType = typeof(ResourceLab);

        public UnitBuffLabBuffComponent(Unit owner) : base(owner)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            buffCom = GetComponent<UnitBuffComponent>();
        }

        private void Clear()
        {
            addBuffs.Clear();
        }

        public void Refresh()
        {
            Clear();

            var labs = MyPlayer.Instance.core.lab.GetLabs();

            foreach (var lab in labs)
            {
                var resLab = ResourceManager.Instance.lab.GetLab(lab.resID);
                if (resLab == null)
                {
                    continue;
                }

                foreach (var ability in resLab.targetAbilities)
                {
                    if (ability.buffIDs.Count == 0)
                    {
                        continue;
                    }

                    if (!UnitRule.IsTargetable(owner, owner, ability.applyTargets))
                    {
                        continue;
                    }

                    foreach (var buffID in ability.buffIDs)
                    {
                        addBuffs[buffID] = (resLab, lab.GetLevel());
                    }
                }
            }

            var addBuffIDs = addBuffs.Keys.ToList();
            foreach (var buff in buffCom.GetBuffs())
            {
                if (buff.tbuff.fromResType != resLabType || addBuffIDs.Contains(buff.tbuff.id))
                {
                    continue;
                }

                buffCom.remove.AddRemoveBuffID(buff.tbuff.id);
            }

            foreach (var addBuff in addBuffs)
            {
                var res = addBuff.Value.Item1;
                var level = addBuff.Value.Item2;
                var buff = buffCom.GetBuffByID(addBuff.Key);
                if (buff != null && buff.tbuff.GetLevel() >= level)
                {
                    continue;
                }

                buffCom.add.AddBuff(owner, addBuff.Key, res, level);
            }

            Clear();
        }
    }
}