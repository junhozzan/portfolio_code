using UnityEngine;
using UnityEngine.UI;

public class UITextFitter : MonoBehaviour
{
    [SerializeField] Vector2 min = Vector2.zero;
    [SerializeField] Vector2 max = Vector2.zero;

    public void SetFitter(Text text)
    {
        if (text == null)
        {
            return;
        }

        Util.TextSizeFitter(text, true, true);

        var size = text.rectTransform.rect.size;
        size.x = Mathf.Clamp(size.x, min.x, max.x);
        size.y = Mathf.Clamp(size.y, min.y, max.y);

        text.rectTransform.sizeDelta = size;
    }
}
