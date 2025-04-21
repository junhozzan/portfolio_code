using UnityEngine;
using UnityEngine.UI;
using System;

public class UIButton : MonoBehaviour
{
    [SerializeField] protected Button button = null;
    [SerializeField] protected UIText text = null;
    [SerializeField] protected GameObject notice = null;
    [SerializeField] protected GameObject lock_ = null;

    private Func<bool> onNotice = null;
    private Func<bool> onLock = null;
    private Cmd cmd = null;

    public void Init(Action<int> onCmd, Func<bool> onNotice = null, Func<bool> onLock = null, int key = -1)
    {
        this.cmd = Cmd.Add(button.gameObject, eCmdTrigger.OnClick, onCmd, key);
        this.onNotice = onNotice;
        this.onLock = onLock;
    }

    public void Init(Action onCmd, Func<bool> onNotice = null, Func<bool> onLock = null)
    {
        this.cmd = Cmd.Add(button.gameObject, eCmdTrigger.OnClick, onCmd);
        this.onNotice = onNotice;
        this.onLock = onLock;
    }

    public void SetKey(int key)
    {
        if (cmd == null)
        {
            return;
        }

        cmd.SetKey(key);
    }

    public void UpdateNotice()
    {
        if (notice == null)
        {
            return;
        }

        if (onNotice != null)
        {
            notice.SetActive(onNotice());
        }
        else
        {
            notice.SetActive(false);
        }
    }

    public void UpdateLock()
    {
        if (lock_ == null)
        {
            return;
        }

        if (onLock != null)
        {
            var bLock = onLock();
            cmd.Use(!bLock);
            lock_.SetActive(bLock);
        }
        else
        {
            lock_.SetActive(false);
        }
    }

    public void SetActive(bool bActive)
    {
        button.gameObject.SetActive(bActive);
    }

    public void SetText(string txt)
    {
        if (text == null)
        {
            return;
        }

        text.SetText(txt);
    }

    public void SetTextColor(Color color)
    {
        if (text == null)
        {
            return;
        }

        text.SetTextColor(color);
    }

    public bool IsNotice()
    {
        if (notice == null)
        {
            return false;
        }

        return notice.activeSelf;
    }
}
