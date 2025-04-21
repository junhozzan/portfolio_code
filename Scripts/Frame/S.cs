using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S 
{
    public static string Red(string str)
    {
#if UNITY_EDITOR
        return $"<color=#FF2E00>{str}</color>";
#else
        return str;
#endif
    }

    public static string Blue(string str)
    {
#if UNITY_EDITOR
        return $"<color=#009AFF>{str}</color>";
#else
        return str;
#endif
    }
}
