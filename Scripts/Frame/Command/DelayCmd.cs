using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class DelayCmd
{
    private Cmd cmdPress = null;
    private Cmd cmdRelease = null;

    private Action<int> onRelease = null;
    private Action<int> onComplete = null;
    private Tween tween = null;
    private float fDelay = 0.5f;

    public bool UseDelay
    {
        get
        {
            return cmdPress._isUse;
        }
        set
        {
            cmdPress.Use(value);
        }
    }

    public DelayCmd(GameObject button, Action<int> onRelease, Action<int> onComplete, float fDelay = 0.5f)
    {
        cmdPress = Cmd.Add(button, eCmdTrigger.Press, Press);
        cmdRelease = Cmd.Add(button, eCmdTrigger.Release, Release);

        this.onRelease = onRelease;
        this.onComplete = onComplete;

        this.fDelay = fDelay;
    }

    public void Use(bool bUse)
    {
        if (cmdPress != null)
        {
            cmdPress.Use(bUse);
        }

        if (cmdRelease != null)
        {
            cmdRelease.Use(bUse);
        }
    }

    public void SetKey(int key)
    {
        cmdPress.SetKey(key);
        cmdRelease.SetKey(key);
    }

    private void Press(int key)
    {
        // 이벤트 제어
        var bEventCtrl = false;
        var pointerEvent = cmdPress.GetPointerEvent();

        var sqrDragThreshold = Mathf.Pow(EventSystem.current.pixelDragThreshold, 2);
        var startPos = pointerEvent.position;

        tween = DOTween.To(
            null,
            x =>
            {
                if (bEventCtrl)
                {
                    return;
                }

                if (pointerEvent != null)
                {
                    //if (pointerEvent.dragging)
                    if ((startPos - pointerEvent.position).sqrMagnitude >= sqrDragThreshold)
                    {
                        // 일정 포지션 이상 드래그 하면 이벤트 중지
                        bEventCtrl = true;
                    }
                }

                // 딜레이 콜
                if (!bEventCtrl && x >= fDelay)
                {
                    //bEventCtrl = true;

                    if (onComplete != null)
                    {
                        bEventCtrl = true;
                        onComplete(key);
                    }

                }

                if (bEventCtrl)
                {
                    tween = null;
                }
            },
            fDelay,
            fDelay).
            From(0f);
    }

    private void Release(int key)
    {
        if (cmdPress._isUse)
        {
            if (tween != null)
            {
                tween.Kill();

                if (onRelease != null)
                {
                    onRelease(key);
                }
            }
        }
        else
        {
            if (onRelease != null)
            {
                onRelease(key);
            }
        }

        tween = null;
    }

}
