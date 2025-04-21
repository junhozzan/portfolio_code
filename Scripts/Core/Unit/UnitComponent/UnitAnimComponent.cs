using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnitComponent
{
    public class UnitAnimComponent : UnitBaseComponent
    {
        private const float ATTACK_LENGTH = 0.417f;

        public UnitAnimComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            //SetAnimationSpeed(1f);
            SetImmeidateAnimation(UnitAni.IDLE);
        }

        public void Resurrection()
        {
            //SetAnimationSpeed(1f);
            SetAnimation(UnitAni.IDLE);
            SetImmeidateAnimation(UnitAni.IDLE);
        }

        public void Idle()
        {
            //SetAnimationSpeed(1f);
            SetAnimation(UnitAni.IDLE);
        }

        public void Run()
        {
            //SetAnimationSpeed(0.7f);
            SetAnimation(UnitAni.RUN);
        }

        public virtual void Attack(UnitSkill skill)
        {
            var speed = 1f;
            if (skill != null)
            {
                var cool = Math.Max(skill.GetMaxCoolTime(), 0.001f);
                speed = cool > ATTACK_LENGTH ? speed : ATTACK_LENGTH / cool;
            }

            SetAnimationSpeed(speed);
            SetImmeidateAnimation(UnitAni.ATTACK);
        }

        public void Dead()
        {
            //SetAnimationSpeed(0.6f);
            SetImmeidateAnimation(UnitAni.DEAD_2);
        }

        protected void SetAnimation(UnitAni ani)
        {
            owner.core.transform.unitObj?.SetAnimation(ani);
        }

        protected void SetImmeidateAnimation(UnitAni ani)
        {
            owner.core.transform.unitObj?.SetImmediateAnimation(ani);
        }

        public void SetAnimationSpeed(float speed)
        {
            owner.core.transform.unitObj?.SetAnimationSpeed(speed);
        }

        public bool IsAttackMotion()
        {
            return owner.core.transform.unitObj?.IsAttackMotion() ?? false;
        }
    }
}