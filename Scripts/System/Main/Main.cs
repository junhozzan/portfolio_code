using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;
using UIMessageLabel;
using System.Collections.Generic;

public class Main : MonoSingleton<Main>
{
    [SerializeField] CpUI_Loading uiLoading = null;
    [SerializeField] CpUI_MessageLabel uiMessageLabel = null;
    [SerializeField] CpUI_Blind uiBlind = null;
    [SerializeField] CpUI_LoadingProgress uiLoadingProgress = null;

    public readonly TimeOfDay timeOfDay = new TimeOfDay();
    public readonly GameTime time = new GameTime();
    public readonly VersionCheck versionCheck = new VersionCheck();
    public readonly InputNickName inputNickName = new InputNickName();

#if UNITY_EDITOR
    [SerializeField] bool runInBackground = false;
#else
    private bool runInBackground = false;
#endif
    
    private bool isLoadCompleted = false;

    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        DrawController.Create();
#endif
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.multiTouchEnabled = false;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Application.runInBackground = runInBackground;

        if (uiLoading != null)
        {
            uiLoading.gameObject.SetActive(false);
            uiLoading.Init();
        }

        if (uiMessageLabel != null)
        {
            uiMessageLabel.gameObject.SetActive(false);
            uiMessageLabel.Init();
        }

        if (uiBlind != null)
        {
            uiBlind.gameObject.SetActive(false);
        }

        if (uiLoadingProgress != null)
        {
            uiLoadingProgress.gameObject.SetActive(false);
        }
    }

    protected IEnumerator Start()
    {
        uiBlind.Fade(CpUI_Blind.eFade.Out, false, 1f);

        DOTween.SetTweensCapacity(1000, 50);
        Localize.DefaultSetting();
        Option.DefaultSetting();
        Option.Load();

        var liveAd = false;
        var bundleBuildDate = 0L;
        var so = Resources.Load<BuildData>(BuildData.FILE_NAME);
        if (so != null)
        {
            liveAd = so.live_ad;
#if UNITY_EDITOR
            liveAd = false;
#endif

            bundleBuildDate = so.bundle_build_date;
        }

        DefineUI.Initialize();
        AdManager.Instance.Initialize(liveAd);
        BundleManager.Initialize(bundleBuildDate);
        UIManager.Instance.Initialize();
        SoundManager.Instance.Initialize();
        User.Instance.Initialize();
        MyPlayer.Instance.Initialize();
        PopupExtend.Instance.Initialize();
        PlatformManager.Instance.Initialize();

        uiLoading.On();
        uiLoading.LoadStart("");
        uiLoading.ShowLogo();

        var loads = new Func<IEnumerator>[]
        {
            () => { return ResourceManager.Instance.CoLoad(); },
            () => { return JsonTableManager.Instance.CoLoad(); },
            () => { return BundleManager.DownloadAndCache(); },
            () => { return SoundManager.Instance.CoLoad(ResourceManager.Instance.sound.GetSounds()); },
            () => 
            {
                AdManager.Instance.Load();
                LocalSave.Load();
                PlatformManager.Instance.ReadSavedPlatform();
                return null;
            },
            () => { return versionCheck.LoadProductVersion(); },
        };

        for (int i = 0, cnt = loads.Length; i < cnt; ++i)
        {
            yield return loads[i].Invoke();
            uiLoading.LoadingProgress((i + 1) / (float)cnt);
        }
        uiLoading.LoadEnd();

        yield return PlatformManager.Instance.Login();

        uiLoading.OnTouchToStart();
        while (uiLoading.IsTouchToStart())
        {
            yield return null;
        }

        yield return versionCheck.ShowVersionCheck();
        yield return inputNickName.ShowInputNickName();

        uiLoading.Off();
        isLoadCompleted = true;

        uiBlind.Fade(CpUI_Blind.eFade.Out, false, 1f);
        MySceneManager.Instance.EnterMode(GameData.MODE_DATA.MODE_1_ID);

        GameObject.Destroy(uiLoading.gameObject);
        uiLoading = null;
    }

    private void Update()
    {
        if (!isLoadCompleted)
        {
            return;
        }

        var dt = Time.deltaTime;

        timeOfDay.UpdateDt(dt);
        User.Instance.info.AddPlayTime(dt);

        GameEvent.Instance.UpdateDt(dt);
        MySceneManager.Instance.UpdateDt(dt);
        AdManager.Instance.UpdateDt(dt);
        PlatformManager.Instance.UpdateDt(dt);
    }

    public void ShowFloatingMessage(string str)
    {
        if (uiMessageLabel == null)
        {
            return;
        }

        uiMessageLabel.On(str);
    }

    public void ShowLoadingProgress(bool isShow)
    {
        if (uiLoadingProgress == null)
        {
            return;
        }

        uiLoadingProgress.gameObject.SetActive(isShow);
    }
}