using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuffScript;

namespace UnitStatusInternal
{
    public class UnitStatusFreezingComponent : UnitStatusInternalComponent
    {
        public static new UnitStatusFreezingComponent Of(Unit owner, StatusType type)
        {
            return new UnitStatusFreezingComponent(owner, type);
        }

        private UnitStatusFreezingComponent(Unit owner, StatusType type) : base(owner, type)
        {

        }

        public override void Refresh(UnitBuff buff)
        {
            base.Refresh(buff);

            var speed = GetSpeed(buff);

            //owner.core.anim.SetAnimationSpeed(speed);
            owner.core.move.SetIncreaseSpeed(speed);
            owner.core.move.Refresh();
        }

        public override float Compare(UnitBuff buff)
        {
            return GetSpeed(buff);
        }

        private static float GetSpeed(UnitBuff buff)
        {
            var speed = 1f;

            if (buff == null)
            {
                return speed;
            }

            foreach (var script in buff.GetBuffScripts(BuffScriptType.FREEZING_OPTION))
            {
                if (script is BuffScriptFreezingOption tScript)
                {
                    speed = Mathf.Min(tScript.GetSpeed(), speed);
                }
            }

            return speed;
        }
    }
}