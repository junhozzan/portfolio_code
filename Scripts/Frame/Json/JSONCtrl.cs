using System;
using SimpleJSON;

public class JSONCtrl
{
    public static void SetNode(JSONNode vData, string strKey, JSONNode push)
    {
        if (vData == null || string.IsNullOrEmpty(strKey))
        {
            return;
        }

        if (push != null)
        {
            vData.Add(strKey, push);
        }
        else
        {
            vData.Remove(strKey);
        }
    }

    public static void SetNode(JSONNode vData, JSONNode push)
    {
        if (vData == null
            || push == null)
        {
            return;
        }

        vData.Add(push);
    }

    public static void SetString(JSONNode vData, string strKey, string strValue)
    {
        if (strKey != null && vData != null)
        {
            if (strValue != null)
                vData[strKey] = strValue; // dictionary와 비슷하게 데이터 생성
            else
                vData.Remove(strKey);
        }
    }

    public static void CombineNode(JSONNode vData, params JSONNode[] aNodes)
    {
        for (int i = 0; i < aNodes.Length; ++i)
        {
            if (aNodes[i].IsArray)
            {
                JSONCtrl.SetNode(vData, aNodes[i]);
            }
            else
            {
                foreach (var v in aNodes[i])
                {
                    JSONCtrl.SetString(vData, v.Key, v.Value);
                }
            }
        }
    }

    public static void RemoveChild(JSONNode vNode)
    {
        for (int i = vNode.Count - 1; i >= 0; --i)
            vNode.Remove(i);
    }

    public static JSONNode GetNode(JSONNode vData, string strKey)
    {
        if (vData == null)
        {
            return null;
        }

        return vData[strKey];
    }

    public static bool TryGetNode(JSONNode vData, string strKey, out JSONNode node)
    {
        node = null;
        if (vData == null)
        {
            return false;
        }

        node = vData[strKey];
        return node != null;
    }

    public static JSONNode GetSafeNode(JSONNode vData, string strKey)
    {
        var result = GetNode(vData, strKey);
        return result != null ? result : new JSONObject();
    }

    public static string GetString(JSONNode vData, string strKey, string strDefault = "")
    {
        if (vData == null)
            return strDefault;

        JSONNode vNode = vData[strKey];
        if (vNode == null)
            return strDefault;

        return vNode.Value;
    }

    public static float GetFloat(JSONNode vData, string strKey, float fDefault = 0.0f)
    {
        if (float.TryParse(GetString(vData, strKey), out float fResult))
            return fResult;
        return fDefault;
    }

    public static decimal GetDecimal(JSONNode vData, string strKey, decimal _default = 0.0m)
    {
        if(decimal.TryParse(GetString(vData ,strKey), out decimal result))
            return result;

        return _default;
    }

    public static long GetLong(JSONNode vData, string strKey, long lDefault = 0)
    {
        if (long.TryParse(GetString(vData, strKey), out long lResult))
            return lResult;

        return lDefault;
    }

    public static ulong GetUlong(JSONNode vData, string strKey, ulong ulDefault = 0)
    {
        if (ulong.TryParse(GetString(vData, strKey), out ulong ulResult))
            return ulResult;

        return ulDefault;
    }

    public static int GetInt(JSONNode vData, string strKey, int iDefault = 0)
    {
        if (int.TryParse(GetString(vData, strKey), out int iResult))
            return iResult;

        return iDefault;
    }

    public static uint GetUint(JSONNode vData, string strKey, uint uiDefault = 0)
    {
        if (uint.TryParse(GetString(vData, strKey), out uint uiResult))
            return uiResult;

        return uiDefault;
    }
    public static byte GetByte(JSONNode vData, string strKey, byte btDefault = 0)
    {
        if (byte.TryParse(GetString(vData, strKey), out byte btResult))
            return btResult;

        return btDefault;
    }

    public static bool GetBool(JSONNode vData, string strKey, bool bDefault = false)
    {
        if (bool.TryParse(GetString(vData, strKey), out bool bResult))
            return bResult;

        return bDefault;
    }

    public static DateTime GetDateTime(JSONNode vData, string strKey, DateTime defaultDate)
    {
        if (DateTime.TryParse(GetString(vData, strKey), out DateTime dtResult))
            return dtResult;

        return defaultDate;
    }

    /// <summary>
    /// SQL DateTime2 형식을 Unity DateTime형으로 변환
    /// </summary>
    public static DateTime GetDateTime2(JSONNode vData, string strKey, DateTime defaultDate)
    {
        string str = GetString(vData, strKey);

        if (string.IsNullOrEmpty(str))
            return defaultDate;

        string format = System.Text.RegularExpressions.Regex.Replace(str, "[A-Za-z]", " "); // 영문을 스페이스로 변환

        if (string.IsNullOrEmpty(format))
            return defaultDate;

        if (DateTime.TryParse(format, out DateTime dtResult))
            return dtResult;

        return defaultDate;
    }
}
