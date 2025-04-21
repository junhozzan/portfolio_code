using System;
using System.Text;
using UnityEngine;

public static class SINumber
{
    public enum NumberType
    {
        KOR,
        USA,
    }

    private static string[] k_suffixes = { "", "만", "억", "조", "경", "해" }; // 10000단위
    private static string[] u_suffixes = { "", "K", "M", "B", "T", "P", "E" }; // 1000단위

    private static StringBuilder builder = new StringBuilder();

    public static string ToString(this long value, NumberType type, int showCount)
    {
        builder.Clear();

        var suffixes = type == NumberType.KOR ? k_suffixes : u_suffixes;
        var slice = type == NumberType.KOR ? 4 : 3; // 10000단위 : 1000단위
        var numCount = (int)Mathf.Log10(value) / slice;
        var loopCount = Mathf.Min(showCount, suffixes.Length);

        var temp = value;
        for (int i = 0; i < loopCount; ++i)
        {
            var index = numCount - i;
            var digit = (long)Mathf.Pow(10, index * slice);

            var quotient = temp / digit;
            if (quotient == 0)
            {
                continue;
            }

            builder.Append($"{quotient}{suffixes[index]}");

            temp -= quotient * digit;
            if (temp <= 0L)
            {
                break;
            }
        }

        return builder.ToString();
    }
}
