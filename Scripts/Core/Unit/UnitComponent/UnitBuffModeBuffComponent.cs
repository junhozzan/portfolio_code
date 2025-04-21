using System;
using System.Collections.Generic;

namespace UnitComponent
{
    public class UnitBuffModeBuffComponent : UnitBaseComponent
    {
        private readonly List<int> addBuffIDs = new List<int>();
        private UnitBuffComponent buffCom = null;

        private static readonly Type resModeType = typeof(ResourceMode);

        public UnitBuffModeBuffComponent(Unit owner) : base(owner)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            buffCom = GetComponent<UnitBuffComponent>();
        }

        private void Clear()
        {
            addBuffIDs.Clear();
        }

        public void Refresh()
        {
            Clear();

            var mode = ModeManager.Instance.mode;
            if (mode == null)
            {
                return;
            }
            
            var from = mode.core.ally.myUnit;
            if (from == null)
            {
                return;
            }

            var resMode = mode.core.profile.resMode;
            foreach (var ability in resMode.targetAbilities)
            {
                if (ability.buffIDs.Count == 0)
                {
                    continue;
                }

                if (!UnitRule.IsTargetable(owner, owner, ability.applyTargets))
                {
                    continue;
                }

                addBuffIDs.AddRange(ability.buffIDs);
            }

            foreach (var buff in buffCom.GetBuffs())
            {
                if (buff.tbuff.fromResType != resModeType || addBuffIDs.Contains(buff.tbuff.id))
                {
                    continue;
                }

                buffCom.remove.AddRemoveBuffID(buff.tbuff.id);
            }

            foreach (var buffID in addBuffIDs)
            {
                var buff = buffCom.GetBuffByID(buffID);
                if (buff != null)
                {
                    continue;
                }

                buffCom.add.AddBuff(owner, buffID, resMode, 0);
            }

            Clear();
        }
    }
}