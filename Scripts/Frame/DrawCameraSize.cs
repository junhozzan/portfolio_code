using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DrawCameraSize : MonoBehaviour
{
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (!TryGetComponent(out Camera camera))
        {
            return;
        }

        var w = (float)Screen.currentResolution.width;
        var h = (float)Screen.currentResolution.height;
        var vec = Vector2.one * camera.orthographicSize;

        if (w > h)
        {
            vec.x *= w / h;
        }
        else
        {
            vec.y *= w / h;
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(vec, new Vector3(vec.x, -vec.y));
        Gizmos.DrawLine(vec, new Vector3(-vec.x, vec.y));
        Gizmos.DrawLine(-vec, new Vector3(vec.x, -vec.y));
        Gizmos.DrawLine(-vec, new Vector3(-vec.x, vec.y));
    }
#endif
}
