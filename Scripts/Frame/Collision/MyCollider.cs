using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyCollider : MonoBehaviour
{
    [SerializeField] protected Transform trCenter = null;
    [SerializeField] protected bool isDynamicVolume = false;

    public CollisionInfo collInfo { get; private set; } = new CollisionInfo();

    public virtual void Init()
    {
        if (trCenter == null)
        {
            trCenter = transform;
        }
    }

    public void ResetPosition()
    {
        collInfo.ResetPosition(trCenter.position);
    }

    public void UpdateInternal()
    {
        UpdatePosition();
        
        if (isDynamicVolume)
        {
            UpdateVolume();
        }
    }

    public void ForceUpdateVoluem()
    {
        UpdateVolume();
    }

    protected virtual void UpdatePosition()
    {
        collInfo.SetPosition(trCenter.position);
    }

    protected virtual void UpdateVolume()
    {

    }

    public abstract bool IsNearDistance(CollisionInfo targetCollInfo);
    public abstract bool IsTrigger(CollisionInfo targetCollInfo);
}

public class CollisionInfo
{
    public Vector2 currPos { get; private set; } = Vector2.zero;
    public Vector2 prevPos { get; private set; } = Vector2.zero;
    public float volume { get; private set; } = 0f;
    public float sqrVolume { get; private set; } = 0f;

    public float sqrMoveDelta { get { return (currPos - prevPos).sqrMagnitude; } }
    public Vector2 moveDirection { get { return (currPos - prevPos).normalized; } }

    public void DoReset()
    {
        currPos = Vector2.zero;
        prevPos = Vector2.zero;
        volume = 0f;
        sqrVolume = 0f;
    }

    public void ResetPosition(Vector2 position)
    {
        currPos = prevPos = position;
    }

    public void SetPosition(Vector2 position)
    {
        prevPos = currPos;
        currPos = position;
    }

    public void SetVolume(float v)
    {
        volume = v;
        sqrVolume = v * v;
    }

    public void Copy(CollisionInfo other)
    {
        currPos = other.currPos;
        prevPos = other.prevPos;
        volume = other.volume;
        sqrVolume = other.sqrVolume;
    }
}