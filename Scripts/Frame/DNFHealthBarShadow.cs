using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DNF_HEALTH_BAR
{

    public class DNFHealthBarShadow : MonoBehaviour
    {
        [SerializeField] Image shadow = null;
        [SerializeField] RectTransform fillMask = null;
        [SerializeField] DNFHealthBarShadowItem item = null;

        private Color hpColor = Color.white;
        private ObjectPool<DNFHealthBarShadowItem> pool = null;

        // Shadow를 계속 생성하지 않기 위한 변수
        private float cachedStart = -1f;

        private void Awake()
        {
            pool = ObjectPool<DNFHealthBarShadowItem>.Of(item, fillMask);
        }

        public void DoReset()
        {
            pool.Clear();
            cachedStart = -1f;
        }

        public void Init(Color _hpColor, float fill)
        {
            hpColor = _hpColor;
            shadow.fillAmount = fill;
            shadow.color = Color.Lerp(_hpColor, Color.black, 0.5f);

            SetMaskFill(0f);
        }

        public void UpdateShadow(float dt, float fill)
        {
            shadow.fillAmount = fill;
            SetMaskFill(fill);

            foreach (var item in pool.GetPool())
            {
                if (!item.gameObject.activeSelf)
                {
                    continue;
                }

                item.UpdateDt(dt);
            }
        }

        private void SetMaskFill(float fill)
        {
            var frame = shadow.rectTransform.rect.size;
            fillMask.sizeDelta = new Vector2(frame.x * fill, frame.y);
        }

        public void PopHitObject(float start, float end, float prevRate)
        {
            if (cachedStart > 0 && cachedStart == start)
            {
                return;
            }

            cachedStart = start;
            pool.Pop().Init(shadow.rectTransform.rect.size, start, end, prevRate, hpColor, shadow.color);
        }

        public bool IsZero()
        {
            return shadow.fillAmount == 0f;
        }

        public float GetStopRate()
        {
            var rate = 0f;
            var i = 0;
            foreach (var item in pool.GetPool())
            {
                if (!item.gameObject.activeSelf)
                {
                    continue;
                }

                if (!item.IsShadowFadeTime())
                {
                    continue;
                }

                rate = Mathf.Max(item.startRate, rate);
                ++i;
            }

            return rate;
        }

        public bool IsShadowState()
        {
            foreach (var item in pool.GetPool())
            {
                if (!item.gameObject.activeSelf)
                {
                    continue;
                }

                if (!item.IsShadowFadeTime())
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}