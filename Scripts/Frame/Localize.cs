using System.Globalization;
using UnityEngine;

public static class Localize
{
    public static eLanguage language { get; private set; } = eLanguage.EN; // default

    public static void DefaultSetting()
    {
        var sysLanguage = Application.systemLanguage;

        switch (sysLanguage)
        {
            case SystemLanguage.Korean:
                language = eLanguage.KO;
                break;

            case SystemLanguage.Japanese:
                language = eLanguage.JP;
                break;

            //case SystemLanguage.Hindi:
            //    language = eLanguage.HI;
            //    break;

            default:
                language = eLanguage.EN;
                break;
        }
    }

    public static void Refresh(eLanguage _language)
    {
        language = (_language == eLanguage.NONE) ? eLanguage.KO : _language;

        foreach (var text in UIText.a_list)
        {
            if (text == null)
            {
                continue;
            }

            if (!text.gameObject.activeSelf)
            {
                continue;
            }

            text.SetLocalize();
        }
    }

    public static string L(this string strKey)
    {
        return ResourceManager.Instance.str.Get(strKey, language);
    }

    public static string L(this string strKey, object arg0)
    {
        return string.Format(L(strKey), arg0);
    }

    public static string L(this string strKey, object arg0, object arg1)
    {
        return string.Format(L(strKey), arg0, arg1);
    }

    public static string L(this string strKey, object arg0, object arg1, object arg2)
    {
        return string.Format(L(strKey), arg0, arg1, arg2);
    }
}

public enum eLanguage
{
    NONE = -1,
    KO,
    JP, // 일본어
    HI, // 힌디어
    EN, // 영어

    CNT
}