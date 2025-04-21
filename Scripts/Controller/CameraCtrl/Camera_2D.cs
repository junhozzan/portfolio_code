using UnityEngine;
using DG.Tweening;

public class Camera_2D : CpObject
{
    [SerializeField] Camera cam = null;
    [SerializeField] Transform trShake = null;

    public Camera _cam { get { return cam; } }
    private Rect viewRect = Rect.zero;

    protected override void Init()
    {
        DefineUI.ResizeCamera2D(cam, cam.orthographicSize);
        viewRect = Util.GetScreenRect(cam);
    }

    public override void DoReset()
    {
        base.DoReset();
        trShake.localPosition = Vector3.zero;
    }

    public Rect GetViewRect(Vector2 offset)
    {
        var rect = viewRect;
        rect.size += offset;
        rect.center = cam.transform.position;
        return rect;
    }

    public Tween Shake(float maxPower, float duration, bool isFadeOut)
    {
        var power = maxPower;
        DOTween.To(
            null,
            t =>
            {
                if (t < 1f)
                {
                    if (isFadeOut)
                    {
                        power = Mathf.Lerp(maxPower, 0f, t);
                    }

                    trShake.localPosition = Random.insideUnitCircle * power;
                }
                else
                {
                    trShake.localPosition = Vector3.zero;
                }
            },
            0f,
            duration)
            .From(1f);

        return null;
    }

    public void Copy(Camera_2D origin)
    {
        if (origin == null)
        {
            return;
        }

        transform.position = origin.transform.position;
    }
}
