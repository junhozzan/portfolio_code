using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectPointer 
{
    public Vector3? GetPoint(ResourceEffect.EffectPointType type);
}
