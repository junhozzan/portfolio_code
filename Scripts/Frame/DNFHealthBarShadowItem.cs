using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DNF_HEALTH_BAR
{
    public class DNFHealthBarShadowItem : MonoBehaviour
    {
        [SerializeField] Image img = null;
        [SerializeField] AnimationCurve animCurve = null;

        public float startRate { get; private set; } = 0f;

        private Color sColor = Color.white;
        private Color eColor = Color.white;
        private float fadeTime = 0f;


        public void Init(Vector2 frame, float start, float end, float prevRate, Color _sColor, Color _eColor)
        {
            fadeTime = 0f;
            startRate = prevRate;

            var rt = img.rectTransform;

            rt.sizeDelta = new Vector2(frame.x * Mathf.Abs(start - end), frame.y);
            rt.localPosition = new Vector2(frame.x * end, 0f);

            img.color = _sColor;

            sColor = Color.white;
            eColor = _eColor;
        }

        public void UpdateDt(float dt)
        {
            UpdateFade(dt);
            UpdateInactive();
        }

        private void UpdateFade(float dt)
        {
            if (!IsShadowFadeTime())
            {
                return;
            }

            fadeTime += dt;
            //var t = EasingFunctions.InOutSine(fadeTime / DNFHealthBar.SHADOW_FADE_TIME);
            var t = animCurve.Evaluate(fadeTime / DNFHealthBar.SHADOW_FADE_TIME);
            SetColor(Color.Lerp(sColor, eColor, Mathf.Lerp(0, 1f, t)));
        }

        private void UpdateInactive()
        {
            if (IsShadowFadeTime())
            {
                return;
            }

            gameObject.SetActive(false);
        }

        private void SetColor(Color color)
        {
            if (img == null)
            {
                return;
            }

            img.color = color;
        }

        public bool IsShadowFadeTime()
        {
            return fadeTime < DNFHealthBar.SHADOW_FADE_TIME;
        }
    }
}