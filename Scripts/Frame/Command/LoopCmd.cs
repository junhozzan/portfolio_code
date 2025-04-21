using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class LoopCmd
{
    private readonly MonoBehaviour mono = null;
    private readonly Functions functinos = null;
    private readonly Cmd cmdPress = null;
    private readonly float interval = 0f;
    private readonly float waitTime = 1f;

    private Action onRelease = null;

    public LoopCmd(MonoBehaviour mono, GameObject goButton, Functions functinos, float interval, float waitTime = 1f)
    {
        this.mono = mono;
        this.functinos = functinos;
        this.interval = interval;
        this.waitTime = waitTime;

        this.cmdPress = Cmd.Add(goButton, eCmdTrigger.Press, Press);
        Cmd.Add(goButton, eCmdTrigger.Release, Release);
    }

    private void Press()
    {
        // 이전 루프 중지 처리
        Release();

        if (functinos == null)
        {
            return;
        }

        mono.StartCoroutine(CoLoopCall());
    }

    private void Release()
    {
        onRelease?.Invoke();
        onRelease = null;
    }

    private IEnumerator CoLoopCall()
    {
        var isLoop = true;
        onRelease = () => { isLoop = false; };


        // 반복 대기
        var waitAt = Time.unscaledTime + waitTime;
        while (isLoop && waitAt > Time.unscaledTime)
        {
            yield return null;
        }

        var pointerEvent = cmdPress.GetPointerEvent();
        var loopCount = 0;
        do
        {
            // 버튼 드래그시 반복 중지
            if (pointerEvent == null || pointerEvent.dragging)
            {
                break;
            }

            if (functinos.IsBreak(loopCount))
            {
                break;
            }

            ++loopCount;

            functinos.OnLoop(loopCount);

            var delayAt = Time.unscaledTime + interval;
            while (isLoop && delayAt > Time.unscaledTime)
            {
                yield return null;
            }
        }
        while (isLoop);

        onRelease = null;
        functinos.OnComplete(loopCount);
    }

    public void Use(bool bUse)
    {
        if (cmdPress == null)
        {
            return;
        }
     
        cmdPress.Use(bUse);
    }

    public abstract class Functions
    {
        public abstract void OnLoop(int loopCount);

        public abstract void OnComplete(int loopCount);

        public abstract bool IsBreak(int loopCount);
    }

#if USE_DEBUG
    private const bool _DEBUG = true;
#else
    private const bool _DEBUG = false;
#endif
}
