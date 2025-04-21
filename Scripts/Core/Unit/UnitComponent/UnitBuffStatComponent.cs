using System;
using System.Collections.Generic;

namespace UnitComponent
{
    public class UnitBuffStatComponent : UnitBaseComponent
    {
        private readonly List<StatItem.Param> statItemParams = new List<StatItem.Param>(128);
        private UnitBuffComponent buffCom = null;

        public UnitBuffStatComponent(Unit owner) : base(owner)
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
            statItemParams.Clear();
        }

        public override void OnDisable()
        {
            statItemParams.Clear();
            base.OnDisable();
        }

        public List<StatItem.Param> GetStatItemParams()
        {
            statItemParams.Clear();

            foreach (var unitBuff in buffCom.GetBuffs())
            {
                statItemParams.AddRange(unitBuff.GetStatItemParams());
            }

            return statItemParams;
        }
    }
}