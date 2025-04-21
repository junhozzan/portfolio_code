using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitComponent
{
    public class UnitFadeComponent : UnitBaseComponent
    {
        private float deathFade = 1f;
        private float distanceFade = 1f;

        public UnitFadeComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            ResetFadeState();
        }

        public void ResetFadeState()
        {
            deathFade = 1f;
            distanceFade = 1f;
        }

        public void Refresh()
        {
            owner.core.transform.unitObj?.SetAlpha(GetFadeValue());
        }

        public void SetDistanceFade(float a)
        {
            distanceFade = a;
            Refresh();
        }

        public void SetDeathFade(float a)
        {
            deathFade = a;
            Refresh();
        }

        private float GetFadeValue()
        {
            return 1f * deathFade * distanceFade;
        }
    }
}