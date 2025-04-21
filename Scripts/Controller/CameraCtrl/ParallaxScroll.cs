using UnityEngine;

public class ParallaxScroll : MonoBehaviour
{
    private Vector3 spot = Vector3.zero;
    private Transform target = null;
    
    public void SetTarget(Vector3 spot, Transform target)
    {
        this.spot = spot;
        this.target = target;
        RefreshPosition();
    }
    
    private void Update()
    {
        RefreshPosition();
    }

    private void RefreshPosition()
    {
        if (target == null)
        {
            return;
        }

        transform.localPosition = Vector2.Lerp(spot, target.localPosition, 1.0f);
    }
}
