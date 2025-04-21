using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitComponent
{
    public class AssistUnitBuffComponent : UnitBuffComponent
    {
        public AssistUnitBuffComponent(Unit owner) : base(owner)
        {

        }

        public override UnitBuff CreateUnitBuff(TBuff tbuff)
        {
            var resBuff = tbuff.resBuff;
            if (resBuff == null)
            {
                return null;
            }

            // 카피 불가 버프
            if (resBuff.isDisableShadow)
            {
                return null;
            }

            return base.CreateUnitBuff(tbuff);
        }
    }
}
