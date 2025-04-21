using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CpUI_Blind : UIBase
{
    [SerializeField] Image blind = null;

    public void Fade(eFade fade, bool bRaycast, float duration)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (blind == null)
        {
            return;
        }

        DOTween.Kill(blind);
        blind.gameObject.SetActive(true);
        blind.raycastTarget = bRaycast;

        var isFadeIn = fade == eFade.In;

        var start = isFadeIn ? Color.clear : Color.black;
        var end = isFadeIn ? Color.black : Color.clear;

        var tween = DOTween.To(
            null,
            t =>
            {
                blind.color = Color.Lerp(start, end, t);
            },
            1f,
            duration)
            .SetUpdate(true)
            .From(0f)
            .SetId(blind);

        if (!isFadeIn)
        {
            tween.OnComplete(Off);
        }
    }

    public void On(bool visible)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (blind == null)
        {
            return;
        }

        Image img = blind;

        DOTween.Kill(blind);
        blind.gameObject.SetActive(true);
        img.raycastTarget = true;
        img.color = visible ? Color.black : Color.clear;
    }

    public void Off()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (blind == null)
        {
            return;
        }

        DOTween.Kill(blind);
        blind.gameObject.SetActive(false);
    }

    public enum eFade
    {
        Out,
        In
    }
}
