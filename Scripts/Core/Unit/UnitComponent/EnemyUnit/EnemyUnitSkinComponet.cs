using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace UnitComponent
{
    public class EnemyUnitSkinComponet : UnitSkinComponent
    {
        public readonly EnemyUnitSkinCustomPartsComponent customParts = null;

        public EnemyUnitSkinComponet(Unit owner) : base(owner)
        {
            customParts = AddComponent<EnemyUnitSkinCustomPartsComponent>(owner);
        }

        public override ICollection<ResourceSPUM> GetCustomSkins()
        {
            return customParts.GetSkins();
        }

        public void BlackOut()
        {
            var cachedObj = owner.core.transform.unitObj;
            if (cachedObj == null)
            {
                return;
            }

            var renderers = cachedObj.GetSpumRenderers();
            if (renderers == null)
            {
                return;
            }

            var cachedResUnit = owner.core.profile.tunit.resUnit;
            var cachedPosition = owner.core.transform.GetPosition();
            var scale = GetScale();

            DOTween.To(
                () => 0f,
                t =>
                {
                    foreach (var kv in renderers)
                    {
                        var renderer = kv.Key;
                        var info = kv.Value;
                        if (renderer.sprite == null)
                        {
                            continue;
                        }

                        renderer.color = Color.Lerp(info.color, Color.black, t);
                    }
                },
                1f,
                0.4f)
                .OnComplete(() =>
                {
                    cachedObj.SetActive(false);
                    ShowDeathEffect(cachedResUnit, cachedPosition, scale);
                });
        }

    }
}