using UnityEngine;

public class CpUI_UnitUI : CpObject
{
    [SerializeField] CanvasGroup fadeCanvas = null;
    [SerializeField] UISlider hpSlider = null;
    [SerializeField] UISlider mpSlider = null;
    [SerializeField] UIText nameText = null;

    private float showTime = 0f;
    public override eObjectLocation _objectLocation => eObjectLocation.UI_Information;
    public override bool _useUpdate => true;

    protected override void InitSortinGroup()
    {
        // empty
    }

    public override void DoReset()
    {
        base.DoReset();

        fadeCanvas.alpha = 0f;
        hpSlider.gameObject.SetActive(false);
        mpSlider.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
    }

    public void SetHpSlider(float fill)
    {
        if (hpSlider == null)
        {
            return;
        }

        showTime = 1f;
        hpSlider.gameObject.SetActive(fill > 0f);
        hpSlider.SetFill(fill);
    }

    public void SetMpSlider(float fill)
    {
        if (mpSlider == null)
        {
            return;
        }

        showTime = 1f;
        mpSlider.gameObject.SetActive(fill > 0f);
        mpSlider.SetFill(fill);
    }

    public bool IsShowUI()
    {
        if (fadeCanvas == null)
        {
            return false;
        }

        return fadeCanvas.alpha > 0f;
    }

    public override void UpdateDt(float dt)
    {
        UpdateFade(dt);
    }

    private void UpdateFade(float dt)
    {
        if (showTime > 0)
        {
            showTime -= dt;
            fadeCanvas.alpha = 1f;
            return;
        }
        
        if (fadeCanvas.alpha <= 0f)
        {
            return;
        }
     
        fadeCanvas.alpha = Mathf.Max(0f, fadeCanvas.alpha - dt);
    }

    public void SetName(string s)
    {
        nameText.gameObject.SetActive(true);
        nameText.SetText(s);
    }
}
