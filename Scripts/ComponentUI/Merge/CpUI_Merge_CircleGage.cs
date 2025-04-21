using UnityEngine;

public class CpUI_Merge_CircleGage : UIBase
{
    [SerializeField] UIImage gage = null;
    [SerializeField] UIImage cellGage = null;
    [SerializeField] GameObject cellFull = null;
    [SerializeField] Transform originLine = null;

    private ObjectPool<Transform> linePool = null;

    public void Init()
    {
        linePool = ObjectPool<Transform>.Of(originLine, originLine.parent);
    }

    public void SetCell(long count, long max)
    {
        linePool.Clear();
        var offsetAngle = 360f / max;
        var startAngle = 90f;
        for (long i = 0; i < max; ++i)
        {
            var angle = startAngle + (i * offsetAngle);
            var line = linePool.Pop();
            line.localPosition = Vector3.zero;
            line.localRotation = Util.Rotate2D(angle);
        }

        cellGage.SetFillAmount(count / (float)max);
        cellFull.SetActive(count == max);
    }

    public void SetGageFill(float fill)
    {
        gage.SetFillAmount(fill);
    }
}
