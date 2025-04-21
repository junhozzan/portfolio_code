using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpGroundBlock : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Transform pos = null;

    private Vector3 dropStartPos = Vector3.zero;
    private Vector3 dropEndPos = Vector3.zero;

    public void Init()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = FindRenderer();
        }
    }

    public void DoReset()
    {
        pos.localPosition = Vector3.zero;
        spriteRenderer.sprite = null;

        SetAlpha(1f);
    }

    public void Activate()
    {
        pos.localPosition = Vector3.up * Env.Distance(Random.Range(-16f, 16f));
    }

    public void Drop()
    {
        dropStartPos = pos.localPosition;
        dropEndPos = dropStartPos + (Vector3)Util.ToUnitVector(Random.Range(0f, 180f)) * Env.Distance(40f);
    }

    public void DoDrop(float t)
    {
        SetAlpha(1f - EasingFunctions.OutCubic(t));
        pos.localPosition = Vector3.Lerp(dropStartPos, dropEndPos, EasingFunctions.OutExpo(t));
    }

    public void SetSortingOrder(int order)
    {
        if (spriteRenderer == null)
        {
            return;
        }
     
        spriteRenderer.sortingOrder = order;
    }

    private void SetAlpha(float a)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        var color = spriteRenderer.color;
        color.a = a;
        spriteRenderer.color = color;
    }

    public void SetSprite(Sprite sprite)
    {
        if (spriteRenderer == null)
        {
            return;
        }
        
        spriteRenderer.sprite = sprite;
    }

    private SpriteRenderer FindRenderer()
    {
        var renderers = transform.GetComponentsInChildren<SpriteRenderer>(true);
        if (renderers.Length > 0)
        {
            return renderers[0];
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

        spriteRenderer = FindRenderer();
    }
#endif
}
