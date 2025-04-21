using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;
using System.Linq;


public class SpumCustom : MonoBehaviour
{
    [Header("BODY")]
    [SerializeField] SpriteRenderer body = null;
    [SerializeField] SpriteRenderer head = null;
    [SerializeField] SpriteRenderer arm_l = null;
    [SerializeField] SpriteRenderer arm_r = null;
    [SerializeField] SpriteRenderer foot_l = null;
    [SerializeField] SpriteRenderer foot_r = null;

    [Header("WEAPON")]
    [SerializeField] SpriteRenderer weapon_left = null;
    [SerializeField] SpriteRenderer weapon_right = null;
    [SerializeField] SpriteRenderer shield_left = null;
    [SerializeField] SpriteRenderer shield_right = null;

    [Header("HAIR")]
    [SerializeField] SpriteRenderer hair = null;
    [SerializeField] SpriteRenderer helmet = null;

    [Header("CLOTHES")]
    [SerializeField] SpriteRenderer clothes_body = null;
    [SerializeField] SpriteRenderer clothes_left = null;
    [SerializeField] SpriteRenderer clothes_right = null;
    
    [Header("PANTS")]
    [SerializeField] SpriteRenderer pant_left = null;
    [SerializeField] SpriteRenderer pant_right = null;

    [Header("BODY")]
    [SerializeField] SpriteRenderer armor_body = null;
    [SerializeField] SpriteRenderer armor_left = null;
    [SerializeField] SpriteRenderer armor_right = null;

    [Header("BACK")]
    [SerializeField] SpriteRenderer back = null;

    [Header("FACE")]
    [SerializeField] SpriteRenderer mustache = null;
    [SerializeField] SpriteRenderer[] eye_front = null;
    [SerializeField] SpriteRenderer[] eye_back = null;

    [Header("SHADOW")]
    [SerializeField] SpriteRenderer shadow = null;

    private ReadOnlyDictionary<SpriteRenderer, RendererInfo> allRenderer = null;

    private readonly static PartsFactory partsFactory = new PartsFactory();
    private readonly static ReadOnlyDictionary<ResourceSPUM.PartsType, Action<SpumCustom, ResourceSPUM, ResourceSPUM.PartsType>> setters =
        new Dictionary<ResourceSPUM.PartsType, Action<SpumCustom, ResourceSPUM, ResourceSPUM.PartsType>>()
        {
            [ResourceSPUM.PartsType.WEAPON_RIGHT] = (spum, res, type) => { spum.SetWeapon(res, type); },
            [ResourceSPUM.PartsType.WEAPON_LEFT] = (spum, res, type) => { spum.SetWeapon(res, type); },
            [ResourceSPUM.PartsType.SHIELD_RIGHT] = (spum, res, type) => { spum.SetShield(res, type); },
            [ResourceSPUM.PartsType.SHIELD_LEFT] = (spum, res, type) => { spum.SetShield(res, type); },

            [ResourceSPUM.PartsType.BODY] = (spum, res, type) => { spum.SetBody(res, type); },
            [ResourceSPUM.PartsType.CLOTH] = (spum, res, type) => { spum.SetClothes(res, type); },
            [ResourceSPUM.PartsType.PANT] = (spum, res, type) => { spum.SetPant(res, type); },
            [ResourceSPUM.PartsType.ARMOR] = (spum, res, type) => { spum.SetArmor(res, type); },
            [ResourceSPUM.PartsType.EYE] = (spum, res, type) => { spum.SetEye(res, type); },

            [ResourceSPUM.PartsType.HELMET] = (spum, res, type) => { spum.SetSimpleParts(spum.helmet, res, type); },
            [ResourceSPUM.PartsType.BACK] = (spum, res, type) => { spum.SetSimpleParts(spum.back, res, type); },
            [ResourceSPUM.PartsType.HAIR] = (spum, res, type) => { spum.SetSimpleParts(spum.hair, res, type); ; },
            [ResourceSPUM.PartsType.MUSTACHE] = (spum, res, type) => { spum.SetSimpleParts(spum.mustache, res, type); },
        }
        .ReadOnly();

    public void Init()
    {
        allRenderer = CreateAllRenderer();
    }

    private ReadOnlyDictionary<SpriteRenderer, RendererInfo> CreateAllRenderer()
    {
        var ignore = new List<SpriteRenderer>();
        ignore.Add(shadow);

        return transform.GetComponentsInChildren<SpriteRenderer>(true)
            .Where(x => !ignore.Contains(x))
            .ToDictionary(x => x, x => RendererInfo.Of(x))
            .ReadOnly();
    }

    public void DoReset()
    {
        foreach (var renderer in allRenderer.Keys)
        {
            renderer.sprite = null;
        }
    }

    public void SetSkin(ResourceSPUM res)
    {
        foreach (var type in res.allParts.Keys)
        {
            SetSkinType(type, res);
        }
    }

    public void SetSkinType(ResourceSPUM.PartsType type, ResourceSPUM res)
    {
        if (!setters.TryGetValue(type, out var set))
        {
            return;
        }

        set.Invoke(this, res, type);
    }

    private void SetSimpleParts(SpriteRenderer renderer, ResourceSPUM res, ResourceSPUM.PartsType type)
    {
        SetSprite(renderer, partsFactory.Get(res, type), partsFactory.GetColor(res, type));
    }

    private void SetWeapon(ResourceSPUM res, ResourceSPUM.PartsType type)
    {
        SpriteRenderer renderer = null;
        switch (type)
        {
            case ResourceSPUM.PartsType.WEAPON_RIGHT:
                renderer = weapon_right;
                break;

            case ResourceSPUM.PartsType.WEAPON_LEFT:
                renderer = weapon_left;
                break;
        }

        if (renderer == null)
        {
            return;
        }

        var color = partsFactory.GetColor(res, type);
        SetSprite(renderer, partsFactory.Get(res, type), color);
    }

    private void SetShield(ResourceSPUM res, ResourceSPUM.PartsType type)
    {
        SpriteRenderer renderer = null;
        switch (type)
        {
            case ResourceSPUM.PartsType.SHIELD_RIGHT:
                renderer = shield_right;
                break;

            case ResourceSPUM.PartsType.SHIELD_LEFT:
                renderer = shield_left;
                break;
        }

        if (renderer == null)
        {
            return;
        }

        var color = partsFactory.GetColor(res, type);
        SetSprite(renderer, partsFactory.Get(res, type), color);
    }

    private void SetBody(ResourceSPUM res, ResourceSPUM.PartsType type)
    {
        var color = partsFactory.GetColor(res, type);
        var sprites = partsFactory.GetBodys(res);
        SetSprite(body, sprites[0], color);
        SetSprite(head, sprites[1], color);
        SetSprite(arm_l, sprites[2], color);
        SetSprite(arm_r, sprites[3], color);
        SetSprite(foot_l, sprites[4], color);
        SetSprite(foot_r, sprites[5], color);
    }

    private void SetEye(ResourceSPUM res, ResourceSPUM.PartsType type)
    {
        var color = partsFactory.GetColor(res, type);
        var sprites = partsFactory.GetEyes(res);
        // 안구만 변경
        SetSprites(eye_front, sprites[0], color);
    }

    private void SetArmor(ResourceSPUM res, ResourceSPUM.PartsType type)
    {
        var color = partsFactory.GetColor(res, type);
        var sprites = partsFactory.GetTops(res, type);
        SetSprite(armor_body, sprites[0], color);
        SetSprite(armor_left, sprites[1], color);
        SetSprite(armor_right, sprites[2], color);
    }

    private void SetClothes(ResourceSPUM res, ResourceSPUM.PartsType type)
    {
        var color = partsFactory.GetColor(res, type);
        var sprites = partsFactory.GetTops(res, type);
        SetSprite(clothes_body, sprites[0], color);
        SetSprite(clothes_left, sprites[1], color);
        SetSprite(clothes_right, sprites[2], color);
    }

    private void SetPant(ResourceSPUM res, ResourceSPUM.PartsType type)
    {
        var color = partsFactory.GetColor(res, type);
        var sprites = partsFactory.GetPants(res);
        SetSprite(pant_left, sprites[0], color);
        SetSprite(pant_right, sprites[1], color);
    }

    private void SetSprite(SpriteRenderer renderer, Sprite sprite, Color color)
    {
        if (renderer == null)
        {
            return;
        }

        renderer.sprite = null;
        renderer.sprite = sprite;
        renderer.color = color;

        if (allRenderer.TryGetValue(renderer, out var info))
        {
            info.SetColor(color);
        }
    }

    private void SetSprites(SpriteRenderer[] renderers, Sprite sprite, Color color)
    {
        if (renderers == null || renderers.Length == 0)
        {
            return;
        }

        foreach (var renderer in renderers)
        {
            SetSprite(renderer, sprite, color);
        }
    }

    public ReadOnlyDictionary<SpriteRenderer, RendererInfo> GetAllRenderer()
    {
        return allRenderer;
    }

    public GameObject GetShadow()
    {
        return shadow.gameObject;
    }

    public class RendererInfo
    {
        public Color color { get; private set; } = Color.white;

        public static RendererInfo Of(SpriteRenderer renderer)
        {
            return new RendererInfo(renderer);
        }

        private RendererInfo(SpriteRenderer renderer)
        {
            color = renderer.color;
        }

        public void SetColor(Color color)
        {
            this.color = color;
        }
    }

    private class PartsFactory
    {
        private readonly Sprite[] sprites = new Sprite[6];
        private readonly Sprite[] emptySprites = new Sprite[6];

        private Sprite GetSprite(string name)
        {
            return AtlasManager.Instance.GetSprite(Atlas.SPUM, name, ignoreNull: true);
        }

        public Sprite Get(ResourceSPUM res, ResourceSPUM.PartsType type)
        {
            if (res == null)
            {
                return null;
            }

            if (!res.TryGetParts(type, out var parts))
            {
                return null;
            }

            var name = parts.name;
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return GetSprite(name);
        }

        public Sprite[] GetBodys(ResourceSPUM res)
        {
            if (!res.TryGetParts(ResourceSPUM.PartsType.BODY, out var parts))
            {
                return emptySprites;
            }

            var name = parts.name;
            if (string.IsNullOrEmpty(name))
            {
                sprites[0] = null;
                sprites[1] = null;
                sprites[2] = null;
                sprites[3] = null;
                sprites[4] = null;
                sprites[5] = null;
                return sprites;
            }

            sprites[0] = GetSprite(string.Format("{0}_0", name)); // Body
            sprites[1] = GetSprite(string.Format("{0}_1", name)); // Head
            sprites[2] = GetSprite(string.Format("{0}_2", name)); // Arm_L
            sprites[3] = GetSprite(string.Format("{0}_3", name)); // Arm_R
            sprites[4] = GetSprite(string.Format("{0}_4", name)); // Foot_L
            sprites[5] = GetSprite(string.Format("{0}_5", name)); // Foot_R
            return sprites;
        }

        public Sprite[] GetTops(ResourceSPUM res, ResourceSPUM.PartsType type)
        {
            if (!res.TryGetParts(type, out var parts))
            {
                return emptySprites;
            }

            var name = parts.name;
            if (string.IsNullOrEmpty(name))
            {
                sprites[0] = null;
                sprites[1] = null;
                sprites[2] = null;
                return sprites;
            }

            sprites[0] = GetSprite(string.Format("{0}_0", name)); // Body
            sprites[1] = GetSprite(string.Format("{0}_1", name)); // Left
            sprites[2] = GetSprite(string.Format("{0}_2", name)); // Right
            return sprites;
        }

        public Sprite[] GetPants(ResourceSPUM res)
        {
            if (!res.TryGetParts(ResourceSPUM.PartsType.PANT, out var parts))
            {
                return emptySprites;
            }

            var name = parts.name;
            if (string.IsNullOrEmpty(name))
            {
                sprites[0] = null;
                sprites[1] = null;
                return sprites;
            }

            sprites[0] = GetSprite(string.Format("{0}_0", name)); // Left
            sprites[1] = GetSprite(string.Format("{0}_1", name)); // Right
            return sprites;
        }

        public Sprite[] GetEyes(ResourceSPUM res)
        {
            if (!res.TryGetParts(ResourceSPUM.PartsType.EYE, out var parts))
            {
                return emptySprites;
            }

            var name = parts.name;
            if (string.IsNullOrEmpty(name))
            {
                sprites[0] = null;
                sprites[1] = null;
                return sprites;

            }

            sprites[0] = GetSprite(string.Format("{0}_0", name)); // Front
            sprites[1] = GetSprite(string.Format("{0}_1", name)); // Back
            return sprites;
        }

        public Color GetColor(ResourceSPUM res, ResourceSPUM.PartsType type)
        {
            if (!res.TryGetParts(type, out var parts))
            {
                return Color.white;
            }

            return parts.color;
        }
    }

    private SpriteRenderer Find(string name)
    {
        var renderers = transform.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var r in renderers)
        {
            if (r.transform.name != name)
            {
                continue;
            }

            // 망토 체크
            if (name == "Back" && r.transform.parent.name != "P_Back")
            {
                continue;
            }

            return r;
        }

        return null;
    }

    private SpriteRenderer Find(string parent, string name)
    {
        var transforms = transform.GetComponentsInChildren<Transform>(true);
        foreach (var t in transforms)
        {
            if (t.name != parent)
            {
                continue;
            }

            var renderers = t.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var r in renderers)
            {
                if (r.transform.name != name)
                {
                    continue;
                }

                return r;
            }

            break;
        }

        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            return;
        }

        body = Find("Body");
        head = Find("5_Head");
        arm_l = Find("20_L_Arm");
        arm_r = Find("-20_R_Arm");
        foot_l = Find("_3L_Foot");
        foot_r = Find("_12R_Foot");

        weapon_left = Find("L_Weapon");
        weapon_right = Find("R_Weapon");
        shield_left = Find("L_Shield");
        shield_right = Find("R_Shield");
        hair = Find("7_Hair");
        helmet = Find("11_Helmet1");
        mustache = Find("6_FaceHair");
        clothes_body = Find("ClothBody");
        clothes_left = Find("21_LCArm");
        clothes_right = Find("-19_RCArm");
        pant_left = Find("_2L_Cloth");
        pant_right = Find("_11R_Cloth");
        armor_body = Find("BodyArmor");
        armor_left = Find("25_L_Shoulder");
        armor_right = Find("-15_R_Shoulder");
        back = Find("P_Back", "Back");
        shadow = Find("Shadow");

        eye_front = new SpriteRenderer[2]
        {
            Find("P_REye", "Front"),
            Find("P_LEye", "Front")
        };

        eye_back = new SpriteRenderer[2]
        {
            Find("P_REye", "Back"),
            Find("P_LEye", "Back")
        };
    }
#endif

#if USE_DEBUG
    protected const bool _DEBUG = true;
#else
    protected const bool _DEBUG = false;
#endif
}
