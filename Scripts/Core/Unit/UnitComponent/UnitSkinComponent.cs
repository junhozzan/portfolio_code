using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace UnitComponent
{
    public class UnitSkinComponent : UnitBaseComponent
    {
        private readonly HashSet<ResourceSPUM> cachedItemToSpums = new HashSet<ResourceSPUM>();
        private readonly ReadOnlyCollection<ResourceSPUM> empty = new List<ResourceSPUM>().ReadOnly();

        public UnitSkinComponent(Unit owner) : base(owner)
        {

        }

        public void RefreshSkin()
        {
            RefreshOriginSkin();
            RefreshCustomSkin();
        }

        private void RefreshOriginSkin()
        {
            var resSpum = GetOriginSkin();
            if (resSpum == null)
            {
                return;
            }

            foreach (var type in resSpum.allParts.Keys)
            {
                SetSkin(type, resSpum);
            }
        }

        private void RefreshCustomSkin()
        {
            var customSkins = GetCustomSkins();
            if (customSkins == null)
            {
                return;
            }
         
            foreach (var resSpum in customSkins)
            {
                foreach (var type in resSpum.allParts.Keys)
                {
                    SetSkin(type, resSpum);
                }
            }
        }

        public void SetSkin(ResourceSPUM.PartsType type, ResourceSPUM resSpum)
        {
            owner.core.transform.unitObj?.SetSkin(type, resSpum);
        }

        public void RefreshScale()
        {
            owner.core.transform.unitObj?.SetScale(GetScale());
        }

        public void ResetRenderers()
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

                renderer.color = info.color;
            }
        }

        protected virtual ResourceSPUM GetOriginSkin()
        {
            return ResourceManager.Instance.spum.GetSPUM(owner.core.profile.tunit.resUnit.spumID);
        }

        protected virtual float GetScale()
        {
            return owner.core.profile.tunit.resUnit.scale;
        }

        public virtual ICollection<ResourceSPUM> GetCustomSkins()
        {
            return empty;
        }

        protected void ShowDeathEffect(ResourceUnit resUnit, Vector3 position, float scale)
        {
            if (string.IsNullOrEmpty(resUnit.deathEffect))
            {
                return;
            }

            var effect = ObjectManager.Instance.Pop<CpEffect>(resUnit.deathEffect);
            effect.SetLayer(GameData.DEFAULT.LAYER_EFFECT);
            effect.SetScale(scale);
            effect.SetPosition(position);
        }

        protected HashSet<ResourceSPUM> GetItemToSPUMs(ICollection<int> itemIDs)
        {
            cachedItemToSpums.Clear();

            foreach (var itemID in itemIDs)
            {
                var resItem = ResourceManager.Instance.item.GetItem(itemID);
                if (resItem == null)
                {
                    continue;
                }

                var spumID = resItem.GetSpumID();
                if (spumID <= 0)
                {
                    continue;
                }

                var resSpum = ResourceManager.Instance.spum.GetSPUM(spumID);
                if (resSpum == null)
                {
                    continue;
                }

                cachedItemToSpums.Add(resSpum);
            }

            return cachedItemToSpums;
        }
    }
}
