using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DefineUI
{
    // 가로 세로 게임 상관없이 Expand로
    private const CanvasScaler.ScreenMatchMode MATCH_MODE = CanvasScaler.ScreenMatchMode.Expand;

    // 설정된 UI 해상도
    private readonly static Vector2 targetScreen = new Vector2(720, 1280);

    // 기기의 스크린 비율
    public static Vector2 deviceRate { get; private set; } = new Vector2(1f, 1f);

    // 기기의 화면 비율이 적용된 UI 해상도
    public static Vector2 canvasSize { get; private set; } = Vector2.zero;

    public static Vector2 safeAreaSize { get; private set; } = targetScreen;
    public static Vector2 safeAreaCenter { get; private set; } = Vector2.zero;

    // 기기의 화면 비율에 대한 카메라 사이즈 비율
    private static float camSizeRate = 1f;

    public static void Initialize()
    {
        var scaleFactor = 1f;
        switch(MATCH_MODE)
        {
            case CanvasScaler.ScreenMatchMode.MatchWidthOrHeight:
                var logW = Mathf.Log(Screen.width / targetScreen.x, 2);
                var logH = Mathf.Log(Screen.height / targetScreen.y, 2);
                var matchValue = 0.5f;
                var logWeightedAverage = Mathf.Lerp(logW, logH, matchValue);
                scaleFactor = Mathf.Pow(matchValue, logWeightedAverage);
                break;

            case CanvasScaler.ScreenMatchMode.Expand:
                scaleFactor = Mathf.Min(Screen.width / targetScreen.x, Screen.height / targetScreen.y);
                break;

            case CanvasScaler.ScreenMatchMode.Shrink:
                scaleFactor = Mathf.Max(Screen.width / targetScreen.x, Screen.height / targetScreen.y);
                break;
        }

        canvasSize = new Vector2(Screen.width, Screen.height) / scaleFactor;


        var targetAspect = 1f;
        var screenAspect = 1f;
        if (Screen.width >= Screen.height)
        {
            targetAspect = targetScreen.x / targetScreen.y;
            screenAspect = Screen.width / (float)Screen.height;

        }
        else
        {
            targetAspect = targetScreen.y / targetScreen.x;
            screenAspect = Screen.height / (float)Screen.width;
        }

        var scale = screenAspect / targetAspect;
        if (scale < 1f)
        {
            camSizeRate = Mathf.Max(1f, 1f / screenAspect);
        }

        deviceRate = new Vector2(canvasSize.x / Screen.width, canvasSize.y / Screen.height);

        var safeArea = Screen.safeArea;
        safeAreaSize = safeArea.size * deviceRate;
        safeAreaCenter = DeviceToCanvasPos(safeArea.center);

#if UNITY_EDITOR
        Debug.Log($"Device Rate:{deviceRate}");
        Debug.Log($"Camera Rate:{camSizeRate}");

        Debug.Log($"Screen:({Screen.width},{Screen.height})\n"
                + $"Canvas:{canvasSize}\n"
                + $"SafeAreaSzie:{safeAreaSize}\n"
                + $"SafeAreaCenter:{safeAreaCenter}\n");
#endif
    }

    // 고정된 화면비로 화면 설정
    public static void CameraResolution(Camera cam)
    {
        Rect rect = cam.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / (targetScreen.x / targetScreen.y); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;

        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }

        cam.rect = rect;
    }

    public static Vector2 RateToDevice(float fRateX, float fRateY)
    {
        return new Vector2(Screen.width * fRateX, Screen.height * fRateY);
    }

    public static Vector2 RateToCanvas(float fRateX, float fRateY, bool bCenterPivot = true)
    {
        Vector2 pos = new Vector2(canvasSize.x * fRateX, canvasSize.y * fRateY);

        if (bCenterPivot)
        {
            pos.x -= canvasSize.x * 0.5f;
            pos.y -= canvasSize.y * 0.5f;
        }

        return pos;
    }

    public static Vector2 CanvasToDevice(Vector2 canvasPos)
    {
        return canvasPos / deviceRate;
    }

    public static Vector3 WorldToDevice(Camera cam, Vector3 wouldPos)
    {
        return cam.WorldToScreenPoint(wouldPos);
    }

    // 카메라에 비친 월드 포지션을 유아이 포지션으로 변환 
    public static Vector2 WorldToCanvas(Camera cam, Vector3 wouldPos, Vector2 pivot)
    {
        return DeviceToCanvasPos(cam.WorldToScreenPoint(wouldPos), pivot);
    }

    // 스크린 포지션을 부모의 피봇에 대응
    private static Vector2 a_defaultPivot = new Vector2(0.5f, 0.5f);
    public static Vector2 DeviceToCanvasPos(Vector2 scrnPos)
    {
        return DeviceToCanvasPos(scrnPos, a_defaultPivot);
    }

    public static Vector2 DeviceToCanvasPos(Vector2 scrnPos, Vector2 pivot)
    {
        Vector2 uiPos = scrnPos * deviceRate;

        uiPos.x -= (canvasSize.x * pivot.x);
        uiPos.y -= (canvasSize.y * pivot.y);

        return uiPos;
    }

    public static void ResizeCamera2D(Camera cam, float baseSize)
    {
        if (cam == null)
        {
            return;
        }
        // 기기 화면 해상도에 대응하여 카메라의 줌을 변경한다
        cam.orthographicSize = baseSize * camSizeRate;
    }
}