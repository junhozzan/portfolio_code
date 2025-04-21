using UnityEngine;
using System;


public class UIMonoBehaviour : UIBase, IUI
{
    [SerializeField] RectTransform[] safeAreas = null;

    private UIManager.eCanvans myCanvas = UIManager.eCanvans.BASE;
    private bool isRefresh = false;

    protected void SetCanvas(UIManager.eCanvans canvans, bool isAutoParent)
    {
        myCanvas = canvans;
        UIManager.Instance.SetCanvas(this, canvans, isAutoParent);
    }

    public virtual void Init()
    {
        SetSafeArea();
    }

    private void SetSafeArea()
    {
        if (safeAreas == null)
        {
            return;
        }

        var areaCount = safeAreas.Length;
        var anchor = new Vector2(0.5f, 0.5f);
        for (int i = 0; i < areaCount; ++i)
        {
            var area = safeAreas[i];
            area.anchorMin = anchor;
            area.anchorMax = anchor;
            area.sizeDelta = DefineUI.safeAreaSize;
            area.localPosition = DefineUI.safeAreaCenter;
        }
    }

    public virtual void Show(bool show)
    {
        if (show == gameObject.activeSelf)
        {
            return;
        }

        gameObject.SetActive(show);
        UIManager.Instance.EnableCanvas(this);
    }

    public virtual void UpdateDt(float unDt, DateTime now)
    {
        UpdateRefresh();
    }

    private void UpdateRefresh()
    {
        if (!isRefresh)
        {
            return;
        }

        isRefresh = false;
        RefreshInternal();
    }

    protected virtual void RefreshInternal()
    {

    }

    protected void Refresh(object[] args = null)
    {
        isRefresh = true;
    }
    
    public virtual bool CanClose()
    {
        return true;
    }

    public virtual bool IsActive()
    {
        return gameObject.activeSelf;
    }

    protected void UsingUpdate()
    {
        UIManager.Instance.AddUpdateDt(this);
    }

    protected void UsingBlind(bool isTouchClose, bool isBlack = true, byte alpha = 200)
    {
        var blind = UIManager.CreateBlind(transform, isBlack, alpha);

        if (isTouchClose)
        {
            Cmd.Add(blind.gameObject, eCmdTrigger.OnClick, Cmd_Close);
        }
    }

    public void Off()
    {
        UIManager.Instance.CloseAt(this);
    }

    protected virtual void Cmd_Close()
    {
        Off();
        ClickSound();
    }

    public virtual UIManager.eCanvans GetCanvas()
    {
        return myCanvas;
    }

    public virtual bool IsFixed()
    {
        return false;
    }

    public virtual void CloseEvent()
    {

    }
}