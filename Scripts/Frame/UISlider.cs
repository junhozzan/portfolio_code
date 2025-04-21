using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : UIBase
{
    [SerializeField] protected Image fillImage = null;
    [SerializeField] protected UIText fillText = null;
    [SerializeField] protected Transform ratePoint = null; // 현재 슬라이더 양 표시 오브젝트

    private float offset = 0f;
    private Vector2 edgePos = Vector2.zero;
    private bool isExistPoint = false;

    public virtual void Init()
    {
        if (fillImage != null)
        {
            offset = fillImage.rectTransform.rect.width * 0.5f;
        }

        if (ratePoint != null)
        {
            isExistPoint = ratePoint != null;
        }
    }

    public virtual void DoReset()
    {
        SetFill(0f);
        if (fillText != null)
        {
            fillText.gameObject.SetActive(false);
        }
    }

    public void SetFill(float fill)
    {
        if (fillImage != null)
        {
            fillImage.gameObject.SetActive(true);
            fillImage.fillAmount = fill;
        }

        if (isExistPoint)
        {
            edgePos.x = Mathf.Lerp(-offset, offset, fill);
            edgePos.y = 0f;

            if (ratePoint != null)
            {
                ratePoint.localPosition = edgePos;
            }
        }
    }

    public virtual void SetFillColor(Color color)
    {
        if (fillImage == null)
        {
            return;
        }
     
        fillImage.color = color;
    }

    public void SetText(string str)
    {
        if (fillText == null)
        {
            return;
        }

        fillText.gameObject.SetActive(true);
        fillText.SetText(str);
    }
}
