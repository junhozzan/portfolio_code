//using System.Runtime.CompilerServices;
using UnityEngine;

public struct ColorHex
{
    // ---------------------------------------------------------------------
    // shift.
    // ---------------------------------------------------------------------
    private const int OFFSET_L_3 = 8 * 3; // 24
    private const int OFFSET_L_2 = 8 * 2; // 16
    private const int OFFSET_L_1 = 8 * 1; // 8
    private const int OFFSET_L_0 = 8 * 0; // 0
    #region UINT32
    // format: 0xRRGGBBAA
    public static Color32 FromRgba(uint rgba)
    {
        return new Color32(
            (byte)((rgba >> OFFSET_L_3) & 0xFF),
            (byte)((rgba >> OFFSET_L_2) & 0xFF),
            (byte)((rgba >> OFFSET_L_1) & 0xFF),
            (byte)((rgba >> OFFSET_L_0) & 0xFF));
    }

    // format: 0xRRGGBB
    public static Color32 FromRgb(uint rgb)
    {
        return new Color32(
            (byte)((rgb >> OFFSET_L_2) & 0xFF),
            (byte)((rgb >> OFFSET_L_1) & 0xFF),
            (byte)((rgb >> OFFSET_L_0) & 0xFF),
            (byte)(255));
    }

    // format: 0xAARRGGBB
    public static Color32 FromArgb(uint argb)
    {
        return new Color32(
            (byte)((argb >> OFFSET_L_2) & 0xFF),
            (byte)((argb >> OFFSET_L_1) & 0xFF),
            (byte)((argb >> OFFSET_L_0) & 0xFF),
            (byte)((argb >> OFFSET_L_3) & 0xFF));
    }
    #endregion UINT32

    #region INT32
    // format: 0xRRGGBBAA
    public static Color32 FromRgba(int rgba)
    {
        return FromRgba((uint)rgba);
    }

    // format: 0xRRGGBB
    public static Color32 FromRgb(int rgb)
    {
        return FromRgb((uint)rgb);
    }

    // format: 0xAARRGGBB
    public static Color32 FromArgb(int argb)
    {
        return FromArgb((uint)argb);
    }

    // format: 0xAARRGGBB
    public static int ToArgbInt(Color32 color)
    {
        return (color.a << OFFSET_L_3) |
               (color.r << OFFSET_L_2) |
               (color.g << OFFSET_L_1) |
               (color.b << OFFSET_L_0);
    }
    #endregion INT32
    #region STRING
    public static Color32 FromRgbaString(string rgbaString)
    {
        if (uint.TryParse(rgbaString, System.Globalization.NumberStyles.HexNumber, null, out uint rgba))
        {
            return FromRgba(rgba);
        }
#if UNITY_EDITOR
        throw new System.FormatException($"Invalid hex string #{rgbaString}.");
#else
            return (Color32)Color.magenta; // Error
#endif
    }

    public static Color32 FromArgbString(string argbString)
    {
        if (uint.TryParse(argbString, System.Globalization.NumberStyles.HexNumber, null, out uint argb))
        {
            return FromArgb(argb);
        }
#if UNITY_EDITOR
        throw new System.FormatException($"Invalid hex string #{argbString}.");
#else
            return (Color32)Color.magenta; // Error
#endif
    }
    #endregion STRING
}
