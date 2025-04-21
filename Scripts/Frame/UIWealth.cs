using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWealth : UIBase
{
    [SerializeField] UIImage icon = null;
    [SerializeField] UIText amountText = null;

    private ResourceItem resItem = null;

    protected override void Awake()
    {
        base.Awake();

        Cmd.Add(gameObject, eCmdTrigger.OnClick, Cmd_ShowToolTip);
    }

    public UIWealth SetWealthID(int wealthItemID)
    {
        resItem = ResourceManager.Instance.item.GetItem(wealthItemID);
        return this;
    }

    public UIWealth Refresh()
    {
        if (resItem == null)
        {
            gameObject.SetActive(false);
            return this;
        }

        RefreshIcon();
        RefreshAmountText();

        return this;
    }

    private void RefreshIcon()
    {
        if (icon == null || resItem == null)
        {
            return;
        }

        icon.SetSprite(resItem.appearance.atlas, resItem.appearance.sprite);
    }

    private void RefreshAmountText()
    {
        if (resItem == null)
        {
            return;
        }

        var amount = MyPlayer.Instance.core.item.GetAmount(resItem.id);
        SetAmountText(amount);
    }

    public void SetAmountText(long amount)
    {
        if (amountText == null)
        {
            return;
        }

        amountText.SetText(GetCostText(amount));
    }

    private void Cmd_ShowToolTip()
    {
        if (resItem == null)
        {
            return;
        }

        var iicon = resItem as IIcon;
        CpUI_ToolTip.Instance.On(iicon.GetToolTipString(), icon.transform.position);
    }
}
