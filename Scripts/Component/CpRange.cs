using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CpRange : CpObject
{
    [SerializeField] Transform obj = null;
    [SerializeField] float activeTime = 0f;

    public float _activeTime 
    { 
        get 
        { 
            return activeTime; 
        } 
    }

    public void Set(Vector3 shotPos, float scale)
    {
        SetScale(scale);
        SetPosition(shotPos);
    }

    public void TweenDynamicScale(float from, float to, float duration)
    {
        if (from == to)
        {
            return;
        }

        if (obj == null)
        {
            if (_DEBUG)
            {
                Debug.Log(S.Red($"## range object transform is null prefab name({transform.name})"));
            }
            return;
        }

        DOTween.To(
            null,
            t =>
            {
                obj.localScale = Vector3.one * Mathf.Lerp(from, to, t);
            },
            1f,
            duration)
            .From(0f);
    }
}
