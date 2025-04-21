using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CpUI_Loading : UIBase
{
    [SerializeField] UIText versionText = null;
    [SerializeField] CanvasGroup touchToStart = null;
    [SerializeField] Animator unitAnimator = null;
    [SerializeField] UISlider loadingSlider = null;
    [SerializeField] UIText logo = null;
    [SerializeField] GameObject loading = null;

    private Tween twTouchToStart = null;

    public void Init()
    {
        Cmd.Add(touchToStart.gameObject, eCmdTrigger.OnClick, Cmd_TouchToStart);
    }

    public void On()
    {
        logo.gameObject.SetActive(false);
        loading.SetActive(false);
        gameObject.SetActive(true);
        touchToStart.gameObject.SetActive(false);
        loadingSlider.gameObject.SetActive(true);
        
        versionText.SetText($"version.{Application.version}");
        unitAnimator.Play(Animator.StringToHash("IDLE"), 0);
    }

    public void ShowLogo()
    {
        StartCoroutine(CoShowLogo());
    }

    private IEnumerator CoShowLogo()
    {
        while (!ResourceManager.Instance.str.IsLoadComplete)
        {
            yield return null;
        }

        logo.gameObject.SetActive(true);
    }

    public void LoadStart(string str)
    {
        LoadingProgress(0f);
    }

    public void LoadEnd()
    {
        loadingSlider.gameObject.SetActive(false);
        loading.SetActive(true);
    }

    public void OnTouchToStart()
    {
        loading.SetActive(false);
        touchToStart.gameObject.SetActive(true);
        unitAnimator.Play(Animator.StringToHash("MOVE"), 0);
        //SoundManager.Instance.PlayBgm(GameData.SOUND.BGM_HORSE);

        twTouchToStart = DOTween.To(
            null,
            t =>
            {
                touchToStart.alpha = t;
            },
            1f,
            1f)
            .From(0f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void Off()
    {
        gameObject.SetActive(false);
    }

    public void LoadingProgress(float rate)
    {
        loadingSlider.SetFill(rate);
        loadingSlider.SetText($"{(rate * 100).ToString("0")}%");
    }

    public bool IsTouchToStart()
    {
        return touchToStart.gameObject.activeSelf;
    }

    private void Cmd_TouchToStart()
    {
        ClickSound();

        touchToStart.gameObject.SetActive(false);

        if (twTouchToStart != null)
        {
            twTouchToStart.Kill();
            twTouchToStart = null;
        }
    }
}
