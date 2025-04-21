using UnityEngine;

public class CpRotate : MonoBehaviour
{
    [SerializeField] float speed = 360f;
    [SerializeField] bool randomAngle = true;

    private float angle = 0f;

    private void OnEnable()
    {
        angle = randomAngle ? Random.Range(0f, 360f) : 0f;
        transform.localRotation = Util.Rotate2D(angle);
    }

    private void Update()
    {
        angle += Time.deltaTime * speed;
        if (angle >= 360f)
        {
            angle -= 360f;
        }

        transform.localRotation = Util.Rotate2D(angle);
    }
}
