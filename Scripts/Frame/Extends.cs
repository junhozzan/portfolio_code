using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Globalization;
using System.Collections.ObjectModel;

public static class Extends
{
    // enum 요소의 수를 알 수 없을때 사용
    public static int EnumLength(this Type type)
    {
        return Enum.GetValues(type).Length;
    }

    public static byte ToBYTE(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        str = RemoveQuotationMark(str);
        if (!byte.TryParse(str, out byte by))
        {
#if UNITY_EDITOR
            Debug.LogWarning("parse fail : " + str);
#endif
        }

        return by;
    }

    public static short ToSHORT(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        str = RemoveQuotationMark(str);
        if (!short.TryParse(str, out short s))
        {
#if UNITY_EDITOR
            Debug.LogWarning("parse fail : " + str);
#endif
        }

        return s;
    }

    public static ushort ToUSHORT(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        str = RemoveQuotationMark(str);
        if (!ushort.TryParse(str, out ushort us))
        {
#if UNITY_EDITOR
            Debug.LogWarning("parse fail : " + str);
#endif
        }
        return us;
    }

    public static int ToINT(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        str = RemoveQuotationMark(str);
        if (!int.TryParse(str, out int n))
        {
#if UNITY_EDITOR
            Debug.LogWarning("parse fail : " + str);
#endif
        }
        return n;
    }

    public static uint ToUINT(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        str = RemoveQuotationMark(str);
        if (!uint.TryParse(str, out uint un))
        {
#if UNITY_EDITOR
            Debug.LogWarning("parse fail : " + str);
#endif
        }
        return un;
    }

    public static ulong ToULONG(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        str = RemoveQuotationMark(str);
        if (!ulong.TryParse(str, out ulong ul))
        {
#if UNITY_EDITOR
            Debug.LogWarning("parse fail : " + str);
#endif
        }
        return ul;
    }

    public static long ToLONG(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        str = RemoveQuotationMark(str);
        if (!long.TryParse(str, out long l))
        {
#if UNITY_EDITOR
            Debug.LogWarning("parse fail : " + str);
#endif
        }
        return l;
    }

    public static float ToFLOAT(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0f;

        str = RemoveQuotationMark(str);
        if (!float.TryParse(str, out float f))
        {
#if UNITY_EDITOR
            Debug.LogWarning("parse fail : " + str);
#endif
        }
        return f;
    }

    public static decimal ToDECIMAL(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return 0m;
        }

        str = RemoveQuotationMark(str);
        if (!decimal.TryParse(str, out decimal d))
        {
#if UNITY_EDITOR
            Debug.LogWarning("parse fail : " + str);
#endif
        }
        return d;
    }

    public static bool ToBOOL(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;

        str = RemoveQuotationMark(str);
        if (!bool.TryParse(str, out bool b))
        {
#if UNITY_EDITOR
            Debug.LogWarning("parse fail : " + str);
#endif
        }
        return b;
    }

    public static DateTime ToDATETIME(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return DateTime.MinValue;

        str = RemoveQuotationMark(str);
        if (!DateTime.TryParse(str, out DateTime dt))
        {
#if UNITY_EDITOR
            Debug.LogWarning("parse fail : " + str);
#endif
        }
        return dt;
    }

    public static T ToENUM<T>(this string str, T _default = default)
    {
        if (!typeof(T).IsEnum)
        {
            Debug.LogWarning("T must be an enumerated type");
            return default;
        }

        try
        {
            str = RemoveQuotationMark(str);
            return (T)System.Enum.Parse(typeof(T), str);
        }
        catch
        {
#if UNITY_EDITOR
            Debug.LogWarning("parse fail : " + str);
#endif
        }
        return _default;
    }

    public static T ToENUM<T>(this int n) where T : struct
    {
        if (!typeof(T).IsEnum)
        {
#if UNITY_EDITOR
            Debug.LogWarning("T must be an enumerated type");
#endif
            return default;
        }

        return (T)(object)n;
    }

    public static T[] ToEnumArray<T>(this string[] aStr) where T : struct
    {
        T[] arr = new T[aStr.Length];

        for (int i = 0; i < aStr.Length; ++i)
        {
            arr[i] = aStr[i].ToINT().ToENUM<T>();
        }

        return arr;
    }

    public static int BitToIdx(this long l)
    {
        return (int)Mathf.Log(l, 2);
    }

    public static string RemoveQuotationMark(string str)
    {
        return str.Replace("\"", "");
    }

    public static ReadOnlyDictionary<K, V> ReadOnly<K, V>(this IDictionary<K, V> dic)
    {
        return new ReadOnlyDictionary<K, V>(dic);
    }

    public static ReadOnlyCollection<T> ReadOnly<T>(this IEnumerable<T> e)
    {
        return new ReadOnlyCollection<T>(e.ToList());
    }

    public static long ToEpochSecond(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
    }

    public static int STimeToSecond(this string str)
    {
        var array = str.Split(':');
        if (array.Length < 3)
        {
#if UNITY_EDITOR
            Debug.Log(S.Red($"stime length 3 under : {str}"));
#endif
            return 0;
        }

        return array[0].ToINT() * 3600 + array[1].ToINT() * 60 + array[2].ToINT();
    }
}