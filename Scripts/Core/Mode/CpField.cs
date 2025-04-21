using UnityEngine;

public class CpField : CpObject
{
    [SerializeField] ParallaxScroll bg = null;

    public void SetParallaxTarget(Transform target)
    {
        bg.SetTarget(transform.position, target);
    }
}
