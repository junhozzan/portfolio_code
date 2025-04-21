using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Rendering;

using DG.Tweening;

public class CpObject : MonoBehaviour
{
    [Header("[Base]")]
    [SerializeField] protected SortingGroup sortingGroup = null;
    [SerializeField] protected Transform trPosition = null; // 포지션
    [SerializeField] protected Transform trRotate = null;
    [SerializeField] protected Transform trScale = null;
    [SerializeField] protected Transform trFlip = null;
    [SerializeField] protected Transform trCenter = null;

    [SerializeField] protected List<GameObject> ignoreRenderers = new List<GameObject>();

    [Header("[Collision]")]
    [SerializeField] protected MyCollider myCollder = null;

    public virtual eObjectLocation _objectLocation => eObjectLocation.World;
    public virtual bool _useUpdate => false;

    protected CpAutoInactive autoInactive = null;
    protected List<Graphic> graphics = null; // UI
    protected List<SpriteRenderer> sprRenderers = null;

    private float alpha = 1f;

    protected static int SPRITE_FILL_PHASE_ID = Shader.PropertyToID("_FlashAmount");
    protected static int SPRITE_FILL_COLOR_ID = Shader.PropertyToID("_FlashColor");

    public string _objLayer
    {
        get
        {
            if (sortingGroup != null)
            {
                return sortingGroup.sortingLayerName;
            }

            return "Default";
        }
    }
    

    public int _objLayerOrder
    {
        get
        {
            if (sortingGroup != null)
            {
                return sortingGroup.sortingOrder;
            }

            return 0;
        }
    }

    public bool _isActive
    {
        get
        {
            return gameObject.activeSelf;
        }
        set
        {
            gameObject.SetActive(value);
        }
    }

    public static void OnCreateInit(CpObject obj)
    {
        obj.Init();
    }

    protected virtual void Init()
    {
        InitCollider();
        InitGraphic();
        InitRenderer();
        InitSortinGroup();
    }

    private void InitGraphic()
    {
        if (graphics != null)
        {
            return;
        }

        graphics = new List<Graphic>();
        var _graphics = GetComponentsInChildren<Graphic>(true);
        var ignoreRenderers = GetIgnoreRenderers();
        foreach (var graphic in _graphics)
        {
            if (ignoreRenderers.Contains(graphic.gameObject))
            {
                continue;
            }

            graphics.Add(graphic);
        }
    }

    protected virtual List<GameObject> GetIgnoreRenderers()
    {
        return ignoreRenderers;
    }

    protected virtual void InitRenderer()
    {
        if (sprRenderers != null)
        {
            return;
        }

        sprRenderers = new List<SpriteRenderer>();
        var renderers = GetComponentsInChildren<SpriteRenderer>(true);
        var ignoreRenderers = GetIgnoreRenderers();
        foreach (var renderer in renderers)
        {
            if (ignoreRenderers.Contains(renderer.gameObject))
            {
                continue;
            }

            sprRenderers.Add(renderer);
        }
    }

    protected virtual void InitCollider()
    {
        if (myCollder == null)
        {
            return;
        }

        myCollder.Init();
    }

    protected virtual void InitSortinGroup()
    {
        if (sortingGroup != null)
        {
            return;
        }

        sortingGroup = trPosition.gameObject.AddComponent<SortingGroup>();
    }

    public void DelayInactive(float delay)
    {
        if (delay <= 0f)
        {
            // 즉시 비활성화
            _isActive = false;
        }
        else
        {
            if (autoInactive == null)
            {
                if (gameObject.TryGetComponent(out CpAutoInactive cp))
                {
                    autoInactive = cp;
                }
                else
                {
                    autoInactive = gameObject.AddComponent<CpAutoInactive>();
                }
            }

            autoInactive.SetActTime(delay);
            autoInactive.enabled = true;
        }
    }

    public virtual void DoReset()
    {
        DOTween.Complete(this);

        if (autoInactive != null)
        {
            autoInactive.enabled = false;
        }
    }

    public virtual void UpdateDt(float dt)
    {
        // override
    }

    public virtual void SetLayer(string strLayer, int order = 0)
    {
        if (sortingGroup == null)
        {
            return;
        }
     
        sortingGroup.sortingLayerName = strLayer;
        sortingGroup.sortingOrder = order;
    }

    public virtual void SetSortingOrder(int order)
    {
        if (sortingGroup == null)
        {
            return;
        }

        sortingGroup.sortingOrder = order;
    }

    public void ResetCollider()
    {
        if (myCollder == null)
        {
            return;
        }

        myCollder.ResetPosition();
        UpdateCollider();
    }

    public void UpdateCollider()
    {
        if (myCollder == null)
        {
            return;
        }
        myCollder.UpdateInternal();
    }

    public void ForceUpdateColliderVoluem()
    {
        if (myCollder == null)
        {
            return;
        }
        myCollder.ForceUpdateVoluem();
    }

    protected void UpdateZPos()
    {
        var pos = transform.position;
        var updateZ = Vector3.zero;
        updateZ.z = Mathf.LerpUnclamped(0f, 1f, pos.y * 0.00001f);

        trPosition.localPosition = updateZ;
    }
    public virtual void SetLayerOrder(int order)
    {
        if (sortingGroup == null)
        {
            return;
        }
     
        sortingGroup.sortingOrder = order;
    }

    public virtual void SetAlpha(float alpha)
    {
        Color col;
        if (sprRenderers != null && sprRenderers.Count > 0)
        {
            for (int i = 0, len = sprRenderers.Count; i < len; ++i)
            {
                col = sprRenderers[i].color;
                col.a = alpha;
                sprRenderers[i].color = col;
            }
        }

        if (graphics != null && graphics.Count > 0)
        {
            for (int i = 0, cnt = graphics.Count; i < cnt; ++i)
            {
                col = graphics[i].color;
                col.a = alpha;
                graphics[i].color = col;
            }
        }

        this.alpha = alpha;
    }

    public Tween Fade(float from, float to, float duration, float delay = 0f, Action<float> onUpdate = null)
    {
        if (DOTween.IsTweening(this))
        {
            DOTween.Complete(this);
            from = alpha;
        }

        var tween = DOTween.To(
            null,
            t =>
            {
                SetAlpha(t);

                if (onUpdate != null)
                {
                    onUpdate(t);
                }
            },
            to,
            duration)
            .SetId(this)
            .From(from);

        if (delay > 0f)
        {
            tween.SetDelay(delay);
        }

        return tween;
    }

    public Tween FadeOut(float dur, float fDelay = 0f, bool outInvisible = true)
    {
        Tween tween = Fade(1f, 0f, dur, fDelay);

        if (outInvisible)
        {
            tween.OnComplete(() => gameObject.SetActive(false));
        }

        return tween;
    }

    public virtual void SetScale(float scale)
    {
        if (trScale == null)
        {
            return;
        }

        trScale.localScale = Vector3.one * scale;
    }

    /// <summary>
    /// 2D 횐경 위치로 이동
    /// </summary>
    public virtual void SetPosition2D(Vector3 wPos)
    {
        var removeZ = wPos;
        removeZ.z = 0f;
        
        transform.position = removeZ;
        UpdateZPos();
        ResetCollider();
    }

    public virtual void SetPosition(Vector3 wPos)
    {
        var removeZ = wPos;
        removeZ.z = 0f;
        transform.position = removeZ;

        if (trPosition != null)
        {
            trPosition.position = wPos;
        }

        ResetCollider();
    }

    public void AddPosition(Vector3 addPos)
    {
        transform.position += addPos;
    }

    public void SetRotate2D(Vector2 dir)
    {
        if (trRotate == null)
        {
            return;
        }

        trRotate.rotation = Util.Rotate2D(dir);
    }

    public void SetAsLastSibling()
    {
        transform.SetAsLastSibling();
    }

    public void SetAsFirstSibling()
    {
        transform.SetAsFirstSibling();
    }

    public virtual void SetFlip(bool flip)
    {
        if (trFlip == null)
        {
            return;
        }

        var flipScale = trFlip.localScale;
        flipScale.x = flip ? -1f : 1f;

        trFlip.localScale = flipScale;
    }


    public bool IsTrigger(MyCollider targetCollider)
    {
        if (targetCollider == null)
        {
            return false;
        }

        if (IsTrigger(targetCollider.collInfo))
        {
            return true;
        }

        return false;
    }

    public bool IsTrigger(CollisionInfo targetCollInfo)
    {
        if (myCollder == null)
        {
            return false;
        }

        if (!myCollder.IsNearDistance(targetCollInfo))
        {
            return false;
        }

        if (!myCollder.IsTrigger(targetCollInfo))
        {
            return false;
        }

        return true;
    }

    public Vector3 GetPosition()
    {
        return trPosition.position;
    }

    public Vector3 GetCenterPosition()
    {
        return trCenter.position;
    }

    public MyCollider GetMyCollider()
    {
        return myCollder;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

#if USE_DEBUG
    protected const bool _DEBUG = true;
#else
    protected const bool _DEBUG = false;
#endif
}
