using UnityEngine;

namespace UnitComponent
{
    public class UnitTransformComponent : UnitBaseComponent, IEffectPointer
    {
        public CpUnit unitObj { get; protected set; } = null;

        private static Vector3 damageFontOffset = new Vector3(0f, Env.Distance(60f)); 

        public UnitTransformComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
        }

        public override void OnDisable()
        {
            unitObj?.SetActive(false);
            unitObj = null;

            base.OnDisable();
        }

        public void DropObject()
        {
            unitObj = null;
        }

        public virtual void CreateObject(Vector2 position)
        {
            if (owner.core.profile.tunit.resUnit == null)
            {
                return;
            }

            if (unitObj != null)
            {
                return;
            }

            unitObj = PopObject(owner.core.profile.tunit.resUnit.prefab);
            unitObj.SetLayer(GameData.DEFAULT.LAYER_FIELD);
            unitObj.SetPosition2D(position);
            unitObj.ResetCollider();
            unitObj.SetFlip(Random.Range(0, 2) == 0);
#if UNITY_EDITOR
            unitObj.transform.name = $"unit_[{owner.GetType()}][{owner.core.profile.tunit.uid}]";
#endif
        }

        public void UpdateFlip()
        {
            unitObj?.SetFlip(owner.core.move.GetMoveDirection().x > 0f);
        }

        public void SetPosition(Vector3 position)
        {
            unitObj?.SetPosition2D(position);
        }

        public Vector3 GetPosition()
        {
            if (unitObj == null)
            {
                return Vector3.zero;
            }

            return unitObj.GetPosition();
        }

        public Vector3 GetCenterPosition()
        {
            if (unitObj == null)
            {
                return Vector3.zero;
            }

            return unitObj.GetCenterPosition();
        }

        public Vector3 GetDamageFontPosition()
        {
            return GetCenterPosition() + damageFontOffset;
        }

        public MyCollider GetMyCollider()
        {
            return unitObj?.GetMyCollider();
        }

        Vector3? IEffectPointer.GetPoint(ResourceEffect.EffectPointType type)
        {
            if (unitObj == null)
            {
                return null;
            }

            Vector3? point = null;

            switch (type)
            {
                case ResourceEffect.EffectPointType.UNIT_CENTER:
                    point = unitObj?.GetCenterPosition();
                    break;

                case ResourceEffect.EffectPointType.UNIT_POSITION:
                    point = unitObj?.GetPosition();
                    break;
            }

            if (!point.HasValue)
            {
                return null;
            }

            // 이펙트를 앞으로 노출
            return point.Value + new Vector3(0f, 0f, -0.001f);
        }

#if UNITY_EDITOR
        public void DebugStateView(string s)
        {
            unitObj?.UpdateStateView(s);
        }

#endif

        private static CpUnit PopObject(string path)
        {
            return ObjectManager.Instance.Pop<CpUnit>(path);
        }
    }
}