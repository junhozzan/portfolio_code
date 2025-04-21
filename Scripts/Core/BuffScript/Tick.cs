using System;
using UnityEngine;

namespace BuffScript
{
    public class Tick
    {
        private float startDelay = 0f;
        private float interval = 0f;

        private float startDelayTimer = 0f;
        private float intervaTimer = 0f;

        private Action call = null;

        public Tick() { }

        public void SetCall(Action call)
        {
            this.call = call;
        }

        public void DoReset()
        {
            startDelay = 0f;
            interval = 0f;
        }

        public void SetResource(float startDelay, float interval)
        {
            this.startDelay = startDelay;
            this.interval = interval;
        }

        public void On()
        {
            startDelayTimer = startDelay;
            intervaTimer = 0f; // startDelay로 초기 딜레이 설정
        }

        public void UpdateDt(float dt)
        {
            if (startDelayTimer > 0f)
            {
                startDelayTimer = Mathf.Max(startDelayTimer - dt, 0f);
                return;
            }

            if (intervaTimer > 0f)
            {
                intervaTimer = intervaTimer - dt;
                return;
            }

            intervaTimer += interval;
            call?.Invoke();
        }
    }
}