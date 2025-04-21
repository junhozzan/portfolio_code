using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Unity.Collections.LowLevel.Unsafe;
using ModeComponent;


public class Util
{
    public static int EnumToInt<T>(T e) where T : struct, Enum
    {
        return UnsafeUtility.EnumToInt(e);
    }

    public static T IntToEnum<T>(int value) where T : struct
    {
        return BitConvert.IntToEnum32<T>(value);
    }

    public static long Clamp(long value, long min, long max)
    {
        return Math.Min(max, Math.Max(min, value));
    }

    /// <summary>
    /// (좌표평면) 두점 사이각 구하기
    /// </summary>
    /// <param name="aPos"> 기준 </param>
    /// <param name="bPos"> 목표 </param>
    /// <returns></returns>
    public static float AngleAToB(Vector2 aPos, Vector2 bPos, bool isReverse = false)
    {
        return isReverse ? 360f - ToAngle(bPos - aPos) : ToAngle(bPos - aPos);
    }

    /// <summary>
    /// 부채꼴 범위
    /// </summary>
    public static bool IsInFan(Vector2 myPos, Vector2 targetPos, Vector2 seeDir, float angle, float range)
    {
        Vector3 interV = targetPos - myPos;

        // 거리 비교
        if (interV.sqrMagnitude <= range * range)
        {
            // '타겟-나 벡터'와 '내 정면 벡터'를 내적
            float dot = Vector3.Dot(interV.normalized, seeDir);
            // 두 벡터 모두 단위 벡터이므로 내적 결과에 cos의 역을 취해서 theta를 구함
            float theta = Mathf.Acos(dot);
            // angleRange와 비교하기 위해 degree로 변환
            float degree = Mathf.Rad2Deg * theta;

            // 시야각 판별
            if (degree <= angle * 0.5f)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsInCirce(Vector2 center, Vector2 target, float range)
    {
        return (target - center).sqrMagnitude <= range * range;
    }

    /// <summary>
    /// (좌표평면) 각도로 회전값 구하기
    /// </summary>
    public static Quaternion Rotate2D(float fAngle) { return Quaternion.Euler(new Vector3(0f, 0f, fAngle)); }

    /// <summary>
    /// (좌표평면) 방향으로 회전값 구하기
    /// </summary>
    public static Quaternion Rotate2D(Vector3 vec) { return Quaternion.Euler(new Vector3(0f, 0f, ToAngle(vec))); }

    /// <summary>
    /// (좌표평면) 방향으로 각도 구하기 -180 ~ 180
    /// </summary>
    public static float ToAngle(Vector2 vec)
    {
        /// return Mathf.Atan2(vec.y, vec.x) * (180f / Mathf.PI);

        return Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
        //return Math.Atan2(vec.y, vec.x) * (180f / Math.PI);
    }

    /// <summary>
    /// (좌표평면) 방향으로 각도 구하기 0 ~ 360
    /// </summary>
    public static float ToAngle360(Vector2 vec)
    {
        float angle = ToAngle(vec);

        if (angle < 0)
        {
            angle = 360 + angle;
        }

        return angle;
    }

    /// <summary>
    /// (좌표평면) 각도로 단위벡터(Length == 1) 구하기
    /// </summary>
    public static Vector2 ToUnitVector(float deg)
    {
        /// float value = fAngle * ;
        var rad = deg * Mathf.Deg2Rad; //(Mathf.PI / 180f)
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    /// <summary>
    /// (좌표평면) 방향에 각도 더하기
    /// </summary>
    public static Vector2 AddAngle(Vector3 vec, float addAngle)
    {
        return ToUnitVector(ToAngle(vec) + addAngle);
    }

    /// <summary>
    /// 랜덤으로 리스트 정렬
    /// sawpClamp : 랜덤으로 정렬할 인덱스 범위값
    /// </summary>
    public static List<T> Shuffle<T>(List<T> list)
    {
        for (int i = 0, cnt = list.Count; i < cnt; ++i)
        {
            var ranIdx = UnityEngine.Random.Range(0, cnt);
            if (ranIdx != i)
            {
                var temp = list[i];
                list[i] = list[ranIdx];
                list[ranIdx] = temp;
            }
        }

        return list;
    }

    public static bool IsChance(float chance)
    {
        if (chance <= 0f)
        {
            return false;
        }

        return UnityEngine.Random.Range(0, 10000) < chance * 10000;
    }

    /// <summary>
    /// 시간에 따른 예상 위치 반환
    /// </summary>
    public static Vector3 PredictPosition(Vector3 delta, float time)
    {
        // 프레임당 움직임 값 * 초당 프레임 수 * 움직일 시간
        return delta * (int)(1f / Time.deltaTime) * time;
    }

    /// <summary>
    /// 해당 소수점까지만 값 표시
    /// </summary>
    /// <param name="value"> 실제 값 </param>
    /// <param name="space"> 소수점 자리수 0.AA..</param>
    /// <returns></returns>
    public static float Decimal(float value, float space)
    {
        return Mathf.Floor(value / space) * space;
    }

    public static void TextSizeFitter(Text text, bool isVertial = false, bool isHorizontal = false)
    {
        TextSizeFitter(text, Vector2.zero, isVertial, isHorizontal);
    }

    public static void TextSizeFitter(Text text, Vector2 offset, bool isVertial = false, bool isHorizontal = false)
    {
        if (text == null)
        {
            return;
        }

        var size = text.rectTransform.sizeDelta;

        if (isHorizontal)
        {
            size.x = text.preferredWidth;
        }

        if (isVertial)
        {
            size.y = text.preferredHeight;
        }

        text.rectTransform.sizeDelta = size + offset;
    }

    private static string[] aSimbols { get; } = { "", "a", "b", "c", "d", "e", "f", "g" };
    private static System.Text.StringBuilder m_symbolSB = new System.Text.StringBuilder();
    const string DOT = ".";
    public static string SymbolString(float value)
    {
        m_symbolSB.Clear();

        string str = Math.Ceiling(value).ToString();
        int length = str.Length;
        if (length > 3)
        {
            int simbolIdx = length / 3;
            if (simbolIdx < aSimbols.Length)
            {
                switch (length % 3)
                {
                    case 0: m_symbolSB.Append(str[0]).Append(DOT).Append(str[1]).Append(str[2]).Append(aSimbols[simbolIdx]); break;
                    case 1: m_symbolSB.Append(str[0]).Append(str[1]).Append(DOT).Append(str[2]).Append(aSimbols[simbolIdx]); break;
                    case 2: m_symbolSB.Append(str[0]).Append(str[1]).Append(str[2]).Append(aSimbols[simbolIdx]); break;
                }
            }

            return m_symbolSB.ToString();
        }
        else
        {
            return str;
        }
    }

    public static decimal Calc(decimal _base, bool _add, long _value)
    {
        decimal value = _value;
        if (_add)
        {
            return Math.Min(_base + value, long.MaxValue);
        }
        else
        {
            return value;
        }
    }

    public static long Calc(long lBase, bool add, long value)
    {
        if (add)
        {
            return Math.Min(lBase + value, long.MaxValue);
        }
        else
        {
            return value;
        }
    }

    /// <summary>
    /// 원의 형태로 범위 표시
    /// </summary>
    public static void DebugSphere(Vector3 center, float r, Color color)
    {
        if (r <= 0f)
            return;

        var length = 2 * Mathf.PI * r;
        var count = Mathf.FloorToInt(length / (r * 0.2f));
        float deg = 360f / count;

        for (int i = 0; i < count; ++i)
        {
            var sRad = (deg * i) * Mathf.Deg2Rad;
            var eRad = (deg * (i + 1)) * Mathf.Deg2Rad;

            var s = center + (new Vector3(Mathf.Sin(sRad), Mathf.Cos(sRad)) * r);
            var e = center + (new Vector3(Mathf.Sin(eRad), Mathf.Cos(eRad)) * r);

            Debug.DrawLine(s, e, color);
        }
    }

    const string TIME_FORMAT = "{0:00}:{1:00}:{2:00}";
    public static string SecToTimer(double sec)
    {
        var ts = TimeSpan.FromSeconds(sec);
        return string.Format(TIME_FORMAT, (int)ts.TotalHours, ts.Minutes, ts.Seconds);
    }

    const string TIME_FORMAT_2 = "{0:00}:{1:00}";
    public static string SecToTimer2(double sec)
    {
        var ts = TimeSpan.FromSeconds(sec);
        return string.Format(TIME_FORMAT_2, ts.Minutes, ts.Seconds);
    }

    public static string ToHexString(string hexa, string str)
    {
        return string.Format("<color={0}>{1}</color>", hexa, str);
    }

    const string COMMA_FORMAT = "#,##0";
    public static string ToComma(long l)
    {
        return l.ToString(COMMA_FORMAT);
    }

    /// <summary>
    /// 점과 직선 사이의 거리
    /// </summary>
    public static float DotAndLineDist(Vector2 dot, Vector2 lineDir, Vector2 linePos)
    {
        return Vector2.Distance(dot, DotAndLineCross(dot, lineDir, linePos));
    }

    /// <summary>
    /// 직선과 점의 수직인 교차점
    /// </summary>
    public static Vector2 DotAndLineCross(Vector2 dot, Vector2 lineDir, Vector2 linePos)
    {
        if (lineDir == Vector2.zero)
        {
            return linePos;
        }
        else if (lineDir.x == 0f)
        {
            return new Vector2(linePos.x, dot.y);
        }
        else if (lineDir.y == 0f)
        {
            return new Vector2(dot.x, linePos.y);
        }

        // 원점 평행 이동
        dot -= linePos;

        var inclination = lineDir.y / lineDir.x;
        var n = (dot.x / inclination) + dot.y;
        var x = n / (inclination + (1f / inclination));
        var y = inclination * x;

        return new Vector2(x, y) + linePos;
    }

    public static Rect GetScreenRect(Camera cam)
    {
        return new Rect()
        {
            size = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height)) - cam.ScreenToWorldPoint(new Vector3(0, 0)),
            center = cam.transform.position
        };
    }

    public static object GetNewInstance(string packagename, params object[] args)
    {
        return GetNewInstance(Type.GetType(packagename), args);
    }

    public static object GetNewInstance(Type type, params object[] args)
    {
        if (type == null)
        {
            return null;
        }

        object instance;
        if (args != null && args.Length > 0)
        {
            instance = Activator.CreateInstance(type, args);
        }
        else
        {
            instance = Activator.CreateInstance(type);
        }

        return instance;
    }

    public static T[] GetEnumArray<T>(bool reverse = false) where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        var cast = values.Cast<T>();
        return Enumerable.ToArray(reverse ? cast.Reverse() : cast);
    }

    /// <summary>
    /// 선과 선의 충돌 체크
    /// </summary>
    public static bool IsCollisionLineAndLine(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
    {
        Vector2 vA = a2 - a1;
        Vector2 vB = b2 - b1;

        if (vA == Vector2.zero && vB == Vector2.zero)
        {
            if (a1 == b1)
            {
                // 점과점
                return true;
            }
        }
        else
        {
            var parent = vA.x * vB.y - vA.y * vB.x;
            if (parent != 0f)
            {
                var _t = vB.x * (a1.y - b1.y) - vB.y * (a1.x - b1.x);
                var _s = vA.x * (a1.y - b1.y) - vA.y * (a1.x - b1.x);
                var t = _t / parent;
                var s = _s / parent;

                // 부동 소수점 오차로 인한 계산 최대, 최소값을 설정
                if (t >= -0.0001f && t <= 1.0001f
                    && s >= -0.0001f && s <= 1.0001f)
                {
                    // 선분 내 교차
                    /*
                    var cross = new Vector2()
                    {
                        x = a1.x + t * (a2.x - a1.x),
                        y = a1.y + t * (a2.y - a1.y)
                    };
                    */
                    return true;
                }
            }
            else
            {
                // 평행
                if (IsInLine(a1, a2, b1)
                    || IsInLine(a1, a2, b2)
                    || IsInLine(b1, b2, a1)
                    || IsInLine(b1, b2, a2))
                {
                    return true;
                }
            }
        }

        // 만나지 않는다.
        return false;
    }

    public static bool TryGetCrossLine(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2, out Vector3 result)
    {
        var vA = a2 - a1;
        var vB = b2 - b1;

        var parent = vA.x * vB.y - vA.y * vB.x;
        if (parent != 0f)
        {
            var _t = vB.x * (a1.y - b1.y) - vB.y * (a1.x - b1.x);
            var _s = vA.x * (a1.y - b1.y) - vA.y * (a1.x - b1.x);
            var t = _t / parent;
            var s = _s / parent;

            // 부동 소수점 오차로 인한 계산 최대, 최소값을 설정
            if (t >= -0.0001f && t <= 1.0001f
                && s >= -0.0001f && s <= 1.0001f)
            {
                result = new Vector3()
                {
                    x = a1.x + t * (a2.x - a1.x),
                    y = a1.y + t * (a2.y - a1.y)
                };

                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    private static bool IsInLine(Vector2 a, Vector2 b, Vector2 pos)
    {
        // x축 평행으로 정렬 후 충돌 계산
        // 부동 소수점 오차로 인한 정확한 cross 값을 구할 수 없음으로 거리 최솟값을 설정
        var angle = ToAngle(b - a);
        var movedB = AngleMove(angle, a, b);
        var movedPos = AngleMove(angle, a, pos);
        var cross = DotAndLineCross(movedPos, (movedB - a).normalized, a);
        return (movedPos - cross).sqrMagnitude <= 0.000000001f && a.x <= movedPos.x && movedPos.x <= movedB.x;
    }


    /// <summary>
    /// 원과 사각형의 충돌
    /// </summary>
    public static bool IsCollisionBoxAndCircle(Rect rect, float rectAngle, Vector2 circlePos, float radius)
    {
        var vCirclePos = AngleMove(rectAngle, rect.center, circlePos);
        var dx = Math.Abs(vCirclePos.x - rect.center.x) - rect.width * 0.5f;
        var dy = Math.Abs(vCirclePos.y - rect.center.y) - rect.height * 0.5f;

        if (dx > radius || dy > radius)
        {
            return false;
        }
        else if (dx <= 0 || dy <= 0)
        {
            return true;
        }

        return Math.Pow(dx, 2) + Math.Pow(dy, 2) <= Math.Pow(radius, 2);
    }

    /// <summary>
    /// 직선과 사각형의 충돌
    /// </summary>
    public static bool IsCollisionBoxAndLine(Rect rect, float rectAngle, Vector2 s, Vector2 e)
    {
        var vS = AngleMove(rectAngle, rect.center, s);
        var vE = AngleMove(rectAngle, rect.center, e);

        if (IsContainsRect(rect, vS) || IsContainsRect(rect, vE))
        {
            return true;
        }

        var contacts = GetContacts(vS, vE, rect);
        foreach (var contact in contacts)
        {
            if (IsContainsRect(rect, contact) && IsInLine(vS, vE, contact))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsContainsRect(Rect rect, Vector2 pos)
    {
        return pos.x >= rect.xMin && pos.x <= rect.xMax && pos.y >= rect.yMin && pos.y <= rect.yMax;
    }

    private static List<Vector2> a_contact = new List<Vector2>(4);
    public static List<Vector2> GetContacts(Vector2 s, Vector2 e, Rect rect)
    {
        a_contact.Clear();

        var vec = e - s;
        if (vec.x != 0)
        {
            a_contact.Add(new Vector2(rect.xMin, -vec.y * (e.x - rect.xMin) / vec.x + e.y));
            a_contact.Add(new Vector2(rect.xMax, -vec.y * (e.x - rect.xMax) / vec.x + e.y));
        }

        if (vec.y != 0)
        {
            a_contact.Add(new Vector2(-vec.x * (e.y - rect.yMin) / vec.y + e.x, rect.yMin));
            a_contact.Add(new Vector2(-vec.x * (e.y - rect.yMax) / vec.y + e.x, rect.yMax));
        }

        return a_contact;
    }

    public static Vector2 AngleMove(float angle, Vector2 origin, Vector2 pos)
    {
        var vec = pos - origin;
        var deg = Mathf.Rad2Deg * Mathf.Atan2(vec.y, vec.x) - angle;
        var rad = Mathf.Deg2Rad * deg;

        return origin + (new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * vec.magnitude);
    }

    public static void IncreaseCapacity<T>(List<T> list, int count)
    {
        if (list.Capacity >= count)
        {
            return;
        }

        list.Capacity = count;
    }

    public static T[] InitArray<T>(T[] array, int size, T _default)
    {
        if (array == null || array.Length != size)
        {
            array = new T[size];
        }

        for (int i = 0; i < size; ++i)
        {
            array[i] = _default;
        }

        return array;
    }

    public static Vector2 GetNearestPointInPerimeter(Rect box, Vector2 pos)
    {
        if (box.Contains(pos))
        {
            return pos;
        }

        var x1 = Mathf.Clamp(pos.x, box.xMin, box.xMax);
        var y1 = Mathf.Clamp(pos.y, box.yMin, box.yMax);

        var dl = Mathf.Abs(x1 - box.xMin);
        var dr = Mathf.Abs(x1 - box.xMax);
        var db = Mathf.Abs(y1 - box.yMin);
        var dt = Mathf.Abs(y1 - box.yMax);
        var m = Mathf.Min(Mathf.Min(dl, dr), Mathf.Min(db, dt));

        if (m == db)
        {
            return new Vector2(x1, box.yMin);
        }

        if (m == dt)
        {
            return new Vector2(x1, box.yMax);
        }

        if (m == dl)
        {
            return new Vector2(box.xMin, y1);
        }

        return new Vector2(box.xMax, y1);
    }

    public static int GetDayOffset(DateTime a, DateTime b)
    {
        var aMid = new DateTime(a.Year, a.Month, a.Day);
        var bMid = new DateTime(b.Year, b.Month, b.Day);

        return (int)(aMid - bMid).TotalDays;
    }

    public static Color HexToColor(string hex)
    {
        if (hex == null || hex.Length < 6)
        {
            return Color.white;
        }

        return new Color32()
        {
            r = (byte)Convert.ToUInt32(hex.Substring(0, 2), 16),
            g = (byte)Convert.ToUInt32(hex.Substring(2, 2), 16),
            b = (byte)Convert.ToUInt32(hex.Substring(4, 2), 16),
            a = 255
        };
    }

    public static int ConvertRateToLocate(float rateX, float rateY)
    {
        var x = (int)(rateX * 100f);
        var y = (int)(rateY * 100f);

        return y * 1000 + x;
    }

    public static Vector2 ConvertLocateToPos(Vector2 size, int locate)
    {
        float rateX = locate % 1000 / 100f;
        float rateY = locate / 1000 / 100f;

        return new Vector2(size.x * rateX, size.y * rateY);
    }
}
