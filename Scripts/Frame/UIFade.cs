using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFade : MonoBehaviour
{
    private CanvasGroup canvasGroup = null;

    [SerializeField] float fadeTime = 1f;
    [SerializeField] bool fadeOut = true;
    [SerializeField] bool loop = false;

    [SerializeField] float start = 0;
    [SerializeField] float end = 0;
    [SerializeField] float time = 0f;

    private void Awake()
    {
        if (!gameObject.TryGetComponent(out CanvasGroup cg))
        {
            cg = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup = cg;
    }

    private void Update()
    {
        if (canvasGroup == null)
        {
            return;
        }

        time += Time.deltaTime;

        var rate = time / fadeTime;
        canvasGroup.alpha = Mathf.Lerp(start, end, rate);

        if (!loop)
        {
            return;
        }

        if (rate < 1f)
        {
            return;
        }

        time = 0f;

        var temp = start;
        start = end;
        end = temp;
    }

    private void OnEnable()
    {
        time = 0f;

        if (fadeOut)
        {
            start = 1f;
            end = 0f;
        }
        else
        {
            start = 0f;
            end = 1f;
        }
    }
}
