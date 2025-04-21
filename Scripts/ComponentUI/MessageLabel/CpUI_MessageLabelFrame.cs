using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class CpUI_MessageLabelFrame : UIBase
{
    [SerializeField] UIText text = null;

    private float activeTime = 0f;

    public void SetText(string s)
    {
        text.SetText(s);

        activeTime = Main.Instance.time.realtimeSinceStartup + 2f;
    }

    public string GetText()
    {
        return text._strText;
    }

    private void Update()
    {
        if (activeTime > Main.Instance.time.realtimeSinceStartup)
        {
            return;
        }

        gameObject.SetActive(false);
    }
}