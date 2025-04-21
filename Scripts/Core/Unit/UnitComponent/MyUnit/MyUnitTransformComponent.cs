using UnityEngine;

namespace UnitComponent
{
    public class MyUnitTransformComponent : UnitTransformComponent
    {
        private CpEffectFollowTarget trailEffect = null;

        public MyUnitTransformComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            ReturnTrailEffect();
        }

        public override void CreateObject(Vector2 position)
        {
            base.CreateObject(position);

            ReturnTrailEffect();
            //SetTrailEffect();
        }


        private void SetTrailEffect()
        {
            if (unitObj == null)
            {
                return;
            }

            trailEffect = ObjectManager.Instance.Pop<CpEffectFollowTarget>(GameData.PREFAB.DARK_TRAIL);
            trailEffect.SetTarget(unitObj);
            trailEffect.SetLayer(unitObj._objLayer, unitObj._objLayerOrder);
        }

        private void ReturnTrailEffect()
        {
            if (trailEffect != null)
            {
                trailEffect.SetActive(false);
            }

            trailEffect = null;
        }

        public override void OnDisable()
        {
            ReturnTrailEffect();
            base.OnDisable();
        }
    }
}