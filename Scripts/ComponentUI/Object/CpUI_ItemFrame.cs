using UnityEngine;

public class CpUI_ItemFrame : UIBase
{
    [SerializeField] UIImage bg = null;
    [SerializeField] UIImage bgFrame = null;
    [SerializeField] UIImage icon = null;
    [SerializeField] GameObject disable = null;
    [SerializeField] UIText amountText = null;
    [SerializeField] UIText levelText = null;

    private IIcon iicon = null;
    private Cmd cmdShowToolTip = null;

    protected override void Awake()
    {
        base.Awake();
        cmdShowToolTip = Cmd.Add(bg.gameObject, eCmdTrigger.OnClick, Cmd_ShowToolTip);
    }

    public void SetDefault()
    {
        levelText.gameObject.SetActive(false);
        amountText.gameObject.SetActive(false);
        disable.SetActive(false);
    }

    public CpUI_ItemFrame Set(IIcon iicon, bool isShowTooltip)
    {
        this.iicon = iicon;
        cmdShowToolTip.Use(isShowTooltip);
        cmdShowToolTip.SetRaycast(isShowTooltip);

        RefreshIcon();
        RefreshBg();
        //RefreshAmountText();

        return this;
    }

    public CpUI_ItemFrame SetLevelText(string text)
    {
        levelText.gameObject.SetActive(true);
        levelText.SetText(text);

        return this;
    }

    private void RefreshIcon()
    {
        if (iicon == null)
        {
            return;
        }

        if (icon == null)
        {
            return;
        }

        icon.SetSprite(iicon.GetSpriteAtlas(), iicon.GetSpriteName());
        icon.SetColor(Color.white);
    }

    private void RefreshAmountText()
    {
        if (iicon == null)
        {
            return;
        }

        if (amountText == null)
        {
            return;
        }

        var s = iicon.GetAmountText();
        if (string.IsNullOrEmpty(s))
        {
            amountText.gameObject.SetActive(false);
            return;
        }

        amountText.gameObject.SetActive(true);
        amountText.SetText(s);
    }

    private void RefreshBg()
    {
        if (iicon == null)
        {
            return;
        }

        var color = Color.Lerp(Color.black, iicon.GetBackgroundColor(), 0.8f);

        if (bg != null)
        {
            bg.SetGradient(Color.black, color);
        }

        if (bgFrame != null)
        {
            bgFrame.SetColor(color);
        }
    }

    private void Cmd_ShowToolTip()
    {
        ClickSound();

        if (iicon.IsShowToolTip())
        {
            CpUI_ToolTip.Instance.On(iicon.GetToolTipString(), transform.position);
        }
        else if (iicon is ResourceItem resItem)
        {
            PopupExtend.Instance.ShowItemInfo(resItem.id);
        }
        else if (iicon is ResourcePack resPack)
        {
            var popup = PopupExtend.Instance.ShowByPack(resPack.id, false);
            popup.SetButton().SetTextKey("key_ok".L());
        }
    }
}
