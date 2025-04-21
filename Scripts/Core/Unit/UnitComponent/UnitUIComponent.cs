using System.Collections;
using UnityEngine;

namespace UnitComponent
{
    public class UnitUIComponent : UnitBaseComponent
    {
        protected CpUI_UnitUI unitUI = null;

        public UnitUIComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            unitUI = null;
        }

        public override void OnDisable()
        {
            unitUI?.SetActive(false);
            unitUI = null;

            base.OnDisable();
        }

        public virtual void CreateUnitUI()
        {
            if (unitUI != null)
            {
                return;
            }

            unitUI = ObjectManager.Instance.Pop<CpUI_UnitUI>(GameData.PREFAB.UNIT_UI);
            unitUI.SetPosition2D(owner.core.transform.GetPosition());
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);

            Follow();
        }

        private void Follow()
        {
            SetPosition(owner.core.transform.GetPosition());
        }

        public void SetPosition(Vector3 position)
        {
            unitUI?.SetPosition2D(position);
        }

        public void SetHpSlider(float fill)
        {
            unitUI?.SetHpSlider(fill);
        }

        public void SetMpSlider(float fill)
        {
            unitUI?.SetMpSlider(fill);
        }

        public void SetNameText(string s)
        {
            unitUI?.SetName(s);
        }
    }
}