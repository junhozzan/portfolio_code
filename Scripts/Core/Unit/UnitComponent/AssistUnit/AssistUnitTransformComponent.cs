using UnityEngine;

namespace UnitComponent
{
    public class AssistUnitTransformComponent : UnitTransformComponent
    {
        private CpEffectFollowTarget auraEffect = null;
        private CpEffectFollowTarget trailEffect = null;

        public AssistUnitTransformComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            unitObj = null;
        }

        public override void CreateObject(Vector2 position)
        {
            base.CreateObject(position);

            //ReturnAuraEffect();
            //SetAuraEffect();

            ReturnTrailEffect();
            SetTrailEffect();

            SpawnEffect();
        }

        private void SetAuraEffect()
        {
            if (unitObj == null)
            {
                return;
            }

            auraEffect = ObjectManager.Instance.Pop<CpEffectFollowTarget>(GameData.PREFAB.DARK_UNIT_AURA);
            auraEffect.SetTarget(unitObj);
            auraEffect.SetLayer(unitObj._objLayer, unitObj._objLayerOrder);
        }

        private void ReturnAuraEffect()
        {
            if (auraEffect != null)
            {
                auraEffect.SetActive(false);
            }

            auraEffect = null;
        }

        public void SetTrailEffect()
        {
            if (unitObj == null)
            {
                return;
            }

            if (trailEffect != null)
            {
                return;
            }

            trailEffect = ObjectManager.Instance.Pop<CpEffectFollowTarget>(GameData.PREFAB.DARK_UNIT_TRAIL);
            trailEffect.SetTarget(unitObj);
            trailEffect.SetLayer(unitObj._objLayer, unitObj._objLayerOrder);
        }

        public void ReturnTrailEffect()
        {
            if (trailEffect != null)
            {
                trailEffect.SetActive(false);
            }

            trailEffect = null;
        }

        public void SpawnEffect()
        {
            var effect = ObjectManager.Instance.Pop<CpEffect>(GameData.PREFAB.SUMMON_DARK);
            effect.SetLayer(unitObj._objLayer, unitObj._objLayerOrder);
            effect.SetPosition(GetCenterPosition());
        }

        public override void OnDisable()
        {
            ReturnAuraEffect();
            ReturnTrailEffect();
            base.OnDisable();
        }
    }
}