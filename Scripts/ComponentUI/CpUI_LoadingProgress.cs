using UnityEngine;

public class CpUI_LoadingProgress : UIBase
{
    [SerializeField] Transform image = null;

    private float time = 0f;
    private float angle = 0f;

    const float NEXT_TIME = 0.1f;

    private void OnEnable()
    {
        time = 0f;
        angle = 0f;
        image.rotation = Util.Rotate2D(angle);
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time < NEXT_TIME)
        {
            return;
        }
        time -= NEXT_TIME;

        angle += 45f;
        image.rotation = Util.Rotate2D(angle);
    }
}
