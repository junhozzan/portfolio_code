using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace UnitComponent
{
    public class AssistUnitSkinComponent : UnitSkinComponent
    {
        private readonly new AssistUnit owner = null;

        private ResourceSPUM copySpum = null;
        private float copyScale = 1f;
        private readonly List<ResourceSPUM> copySkins = new List<ResourceSPUM>();

        public AssistUnitSkinComponent(Unit owner) : base(owner)
        {
            this.owner = owner as AssistUnit;
        }

        public override void DoReset()
        {
            base.DoReset();
            copySkins.Clear();
            copySpum = null;
            copyScale = 1f;
        }

        public void SetCustom(ResourceSPUM copySpum, float copyScale, ICollection<ResourceSPUM> skins)
        {
            this.copySpum = copySpum;
            this.copyScale = copyScale;

            copySkins.Clear();
            copySkins.AddRange(skins);
        }

        protected override ResourceSPUM GetOriginSkin()
        {
            if (copySpum == null)
            {
                return base.GetOriginSkin();
            }

            return copySpum;
        }

        protected override float GetScale()
        {
            return copyScale;
        }

        public override ICollection<ResourceSPUM> GetCustomSkins()
        {
            return copySkins;
        }

        public void SetBlack()
        {
            var renderers = owner.core.transform.unitObj?.GetSpumRenderers();
            if (renderers == null)
            {
                return;
            }

            foreach (var kv in renderers)
            {
                var renderer = kv.Key;
                var info = kv.Value;
                if (renderer.sprite == null)
                {
                    continue;
                }

                renderer.color = Color.Lerp(info.color, GameData.COLOR.ASSIST_UNIT, 0.8f);
            }
        }

        public void Out()
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
                0.5f)
                .OnComplete(() =>
                {
                    cachedObj.SetActive(false);
                    ShowDeathEffect(cachedResUnit, cachedPosition, scale);
                });
        }
    }
}