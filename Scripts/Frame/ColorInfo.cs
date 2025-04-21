using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorInfo
{
    public readonly string hex;
    public readonly Color color;

    public static ColorInfo Of(string hex)
    {
        return new ColorInfo(hex);
    }

    private ColorInfo(string hex)
    {
        this.hex = hex;
        this.color = Util.HexToColor(hex);
    }

    public static implicit operator Color(ColorInfo v)
    {
        return v.color;
    }
}
