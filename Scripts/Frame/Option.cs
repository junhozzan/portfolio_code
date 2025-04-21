using UnityEngine;

public class Option
{
    public static int fps
    {
        get 
        { 
            return Application.targetFrameRate; 
        }
        set
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = value;
        }
    }

    public static string checkedVersion { get; set; } = "1.01";
    public static string checkedBundleDate { get; set; } = "0";
    public static bool isAutoSelect { get; set; } = false;
    public static bool isAutoPlay { get; set; } = false;

    public static void DefaultSetting()
    {
        // 최초 진입시 60프레임 설정
        fps = 60;
        checkedVersion = "1.01";
        checkedBundleDate = "0";
        isAutoSelect = false;
        isAutoPlay = false;
    }

    public static void Load()
    {
        Localize.Refresh(Util.IntToEnum<eLanguage>(PlayerPrefs.GetInt("Language", (int)Localize.language)));
        SoundManager.Instance._volBgm = PlayerPrefs.GetFloat("BGM", SoundManager.Instance._volBgm);
        SoundManager.Instance._volSfx = PlayerPrefs.GetFloat("SFX", SoundManager.Instance._volSfx);
        Option.fps = PlayerPrefs.GetInt("FPS", Option.fps);
        Option.checkedVersion = PlayerPrefs.GetString("Ver", checkedVersion);
        Option.checkedBundleDate = PlayerPrefs.GetString("BundleDate", checkedBundleDate);
        Option.isAutoSelect = PlayerPrefs.GetInt("IsAutoSelect", 0) == 1;
        Option.isAutoPlay = PlayerPrefs.GetInt("IsAutoPlay ", 0) == 1;
    }

    public static void Save()
    {
        PlayerPrefs.SetInt("Language", ((int)Localize.language));
        PlayerPrefs.SetFloat("BGM", SoundManager.Instance._volBgm);
        PlayerPrefs.SetFloat("SFX", SoundManager.Instance._volSfx);
        PlayerPrefs.SetInt("FPS", fps);
        PlayerPrefs.SetString("Ver", checkedVersion);
        PlayerPrefs.SetString("BundleDate", checkedBundleDate);
        PlayerPrefs.SetInt("IsAutoSelect", isAutoSelect ? 1 : 0);
        PlayerPrefs.SetInt("IsAutoSeIsAutoPlaylect", isAutoPlay ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
