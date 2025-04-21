using System;

public static class EasingFunctions
{
    public static float Linear(float t) => t;

    public static float InQuad(float t) => t * t;
    public static float OutQuad(float t) => 1f - InQuad(1f - t);
    public static float InOutQuad(float t)
    {
        if (t < 0.5f) return InQuad(t * 2) / 2;
        return 1f - InQuad((1 - t) * 2) / 2;
    }

    public static float InCubic(float t) => t * t * t;
    public static float OutCubic(float t) => 1f - InCubic(1f - t);
    public static float InOutCubic(float t)
    {
        if (t < 0.5f) return InCubic(t * 2) / 2;
        return 1f - InCubic((1 - t) * 2) / 2;
    }

    public static float InQuart(float t) => t * t * t * t;
    public static float OutQuart(float t) => 1f - InQuart(1f - t);
    public static float InOutQuart(float t)
    {
        if (t < 0.5f) return InQuart(t * 2) / 2;
        return 1f - InQuart((1 - t) * 2) / 2;
    }

    public static float InQuint(float t) => t * t * t * t * t;
    public static float OutQuint(float t) => 1f - InQuint(1f - t);
    public static float InOutQuint(float t)
    {
        if (t < 0.5f) return InQuint(t * 2) / 2;
        return 1f - InQuint((1 - t) * 2) / 2;
    }

    public static float InSine(float t) => (float)-Math.Cos(t * Math.PI / 2);
    public static float OutSine(float t) => (float)Math.Sin(t * Math.PI / 2);
    public static float InOutSine(float t) => (float)(Math.Cos(t * Math.PI) - 1) / -2;

    public static float InExpo(float t) => (float)Math.Pow(2, 10 * (t - 1));
    public static float OutExpo(float t) => 1f - InExpo(1 - t);
    public static float InOutExpo(float t)
    {
        if (t < 0.5f) return InExpo(t * 2) / 2;
        return 1f - InExpo((1 - t) * 2) / 2;
    }

    public static float InCirc(float t) => -((float)Math.Sqrt(1 - t * t) - 1);
    public static float OutCirc(float t) => 1 - InCirc(1 - t);
    public static float InOutCirc(float t)
    {
        if (t < 0.5f) return InCirc(t * 2) / 2;
        return 1f - InCirc((1f - t) * 2) / 2;
    }

    public static float InElastic(float t) => 1f - OutElastic(1 - t);
    public static float OutElastic(float t)
    {
        float p = 0.3f;
        return (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t - p / 4) * (2 * Math.PI) / p) + 1;
    }
    public static float InOutElastic(float t)
    {
        if (t < 0.5f) return InElastic(t * 2) / 2;
        return 1f - InElastic((1f - t) * 2) / 2;
    }

    public static float InBack(float t)
    {
        float s = 1.70158f;
        return t * t * ((s + 1) * t - s);
    }
    public static float OutBack(float t) => 1f - InBack(1f - t);
    public static float InOutBack(float t)
    {
        if (t < 0.5f) return InBack(t * 2) / 2;
        return 1f - InBack((1f - t) * 2) / 2;
    }

    public static float InBounce(float t) => 1f - OutBounce(1f - t);
    public static float OutBounce(float t)
    {
        float div = 2.75f;
        float mult = 7.5625f;

        if (t < 1 / div)
        {
            return mult * t * t;
        }
        else if (t < 2 / div)
        {
            t -= 1.5f / div;
            return mult * t * t + 0.75f;
        }
        else if (t < 2.5f / div)
        {
            t -= 2.25f / div;
            return mult * t * t + 0.9375f;
        }
        else
        {
            t -= 2.625f / div;
            return mult * t * t + 0.984375f;
        }
    }
    public static float InOutBounce(float t)
    {
        if (t < 0.5f) return InBounce(t * 2) / 2;
        return 1f - InBounce((1 - t) * 2) / 2;
    }
}
