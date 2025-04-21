using System.Collections.Generic;

namespace UnitComponent
{
    public class UnitBuffAddComponent : UnitBaseComponent
    {
        private readonly List<UnitBuff> newBuffs = new List<UnitBuff>(4);
        private readonly List<TBuff> newTBuffs = new List<TBuff>(4);
        private UnitBuffComponent buffCom = null;

        public UnitBuffAddComponent(Unit owner) : base(owner)
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
            AllClear();
        }

        public override void OnDisable()
        {
            AllClear();
            base.OnDisable();
        }

        private void AllClear()
        {
            foreach (var tbuff in newTBuffs)
            {
                tbuff.OnDisable();
            }

            newTBuffs.Clear();
            newBuffs.Clear();
        }

        public bool UpdateAdd()
        {
            if (newTBuffs.Count > 0)
            {
                foreach (var tbuff in newTBuffs)
                {
                    var unitBuff = buffCom.CreateUnitBuff(tbuff);
                    if (unitBuff == null)
                    {
                        tbuff.OnDisable();
                        continue;
                    }

                    newBuffs.Add(unitBuff);
                }
                newTBuffs.Clear();

                foreach (var unitBuff in newBuffs)
                {
                    var from = UnitManager.Instance.GetUnitByUID(unitBuff.tbuff.fromUnitUID);
                    unitBuff.On(from, owner);
                }
                newBuffs.Clear();

                return true;
            }

            return false;
        }

        private void AddNewTBuff(TBuff tbuff)
        {
            newTBuffs.Add(tbuff);
        }

        public void AddBuff(Unit from, int buffID, ResourceBase res, long level)
        {
            var now = Main.Instance.time.nowToEpochSecond();

            var resBuff = ResourceManager.Instance.buff.GetBuff(buffID);
            if (resBuff == null)
            {
                return;
            }

            var tbuff = TManager.Instance.Get<TBuff>()
                .Set(resBuff.id)
                .SetLevel(level)
                .SetFromRes(res)
                .SetFromUnit(from.core.profile.tunit.uid)
                .SetUntilAt(now, resBuff.isInfinity, resBuff.GetTime(level));

            AddNewTBuff(tbuff);
        }
    }
}