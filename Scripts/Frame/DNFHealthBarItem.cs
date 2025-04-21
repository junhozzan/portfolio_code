using UnityEngine;
using UnityEngine.UI;

namespace DNF_HEALTH_BAR
{
    public class DNFHealthBarItem : MonoBehaviour
    {
        [SerializeField] Image hp = null;
        [SerializeField] DNFHealthBarShadow shadow = null;

        public int idx = 0;

        public void DoReset()
        {
            shadow.DoReset();
        }

        public void Init(int _idx, float prevFill, Color _color)
        {
            idx = _idx;
            hp.fillAmount = prevFill;
            hp.color = _color;
            shadow.Init(_color, prevFill);

            transform.localPosition = Vector3.zero;
            transform.SetAsFirstSibling();
        }

        public void UpdateShadow(float dt, float fill)
        {
            if (IsZero())
            {
                gameObject.SetActive(false);
                return;
            }

            shadow.UpdateShadow(dt, fill);
        }

        public void SetFill(float currFill, float prevRate)
        {
            if (currFill < hp.fillAmount)
            {
                shadow.PopHitObject(hp.fillAmount, currFill, prevRate);
            }

            hp.fillAmount = currFill;
        }

        public float GetStopRate()
        {
            return shadow.GetStopRate();
        }

        private bool IsZero()
        {
            return hp.fillAmount == 0f && shadow.IsZero();
        }

        public bool IsShadowState()
        {
            return shadow.IsShadowState();
        }
    }
}