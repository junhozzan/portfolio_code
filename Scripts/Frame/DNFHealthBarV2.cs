using DNF_HEALTH_BAR;
using UnityEngine;

public class DNFHealthBarV2 : MonoBehaviour
{
    [SerializeField] DNFHealthBarItem item = null;
    [SerializeField] UIText countText = null;
    
    private ObjectPool<DNFHealthBarItem> pool = null;
    private float prevRate = 1f;
    private float currRate = 1f;
    private float shadowRate = 1f;

    private int maxCount = 0;
    private int startIndex = 0;
    private int endIndex = 0;
    private float shadowSpeed = 0f;

    public const int BAR_DEFAULT_COUNT = 2;

    private void Awake()
    {
        pool = ObjectPool<DNFHealthBarItem>.Of(item, gameObject);
    }

    private void DoReset()
    {
        foreach (var item in pool.GetPool())
        {
            if (!item.gameObject.activeSelf)
            {
                continue;
            }

            item.DoReset();
        }

        pool.Clear();
    }

    public void Init(float maxHp, float hp, float countValue)
    {
        DoReset();

        currRate = prevRate = shadowRate = hp / maxHp;
        maxCount = Mathf.Max(1, (int)(maxHp / countValue));
        startIndex = endIndex = maxCount - 1;
        shadowSpeed = 0f;
    }

    private void Update()
    {
        var dt = Time.deltaTime;
        UpdateShadowRate(dt);
        UpdateShadow(dt);
    }

    private void UpdateShadowRate(float dt)
    {
        //if (shadowUpdateDelay > 0f && IsShadowState())
        //{
        //    shadowUpdateDelay -= dt;
        //    return;
        //}

        //var count = 0;
        //var stopRate = 0f;
        //foreach (var item in pool.GetPool())
        //{
        //    if (!item.gameObject.activeSelf)
        //    {
        //        continue;
        //    }

        //    count += 1;
        //    stopRate = Mathf.Max(item.GetStopRate(), stopRate);
        //}

        //var speed = shadowRate > stopRate ? Mathf.Clamp((count - BAR_DEFAULT_COUNT) * 2f, 1f, MAX_SHAODW_SPEED) : 0f;
        //controlBarSpeed = Mathf.Max(controlBarSpeed, speed);
        //shadowRate = Mathf.Max(currRate, shadowRate - (dt * applyBarSpeed * controlBarSpeed));

        //ResetShadowDelay();

        shadowSpeed = (shadowRate - currRate) * 3f * dt;
        shadowRate = Mathf.Max(currRate, shadowRate - shadowSpeed);
    }

    private void UpdateShadow(float dt)
    {
        foreach (var item in pool.GetPool())
        {
            if (!item.gameObject.activeSelf)
            {
                continue;
            }

            var min = item.idx / (float)maxCount;
            var max = (item.idx + 1) / (float)maxCount;
            var fill = (shadowRate - min) / (max - min);

            item.UpdateShadow(dt, fill);
        }
    }

    public void SetHP(float maxHp, float hp)
    {
        prevRate = currRate;
        currRate = Mathf.Min(Mathf.Clamp01(hp / maxHp), currRate);
        Refresh();
    }

    private void Refresh()
    {
        CreateItems();
        RefreshFill();
        RefreshCountText();
    }

    private void CreateItems()
    {
        var index = RateToIndex(currRate);

        startIndex = endIndex;
        endIndex = Mathf.Min(endIndex, Mathf.Max(index - BAR_DEFAULT_COUNT, -1));

        for (int i = startIndex; i > endIndex; --i)
        {
            var item = pool.Pop();
            item.Init(i, IndexToFill(i, prevRate), GetColor(i));
        }
    }

    private void RefreshFill()
    {
        foreach (var item in pool.GetPool())
        {
            if (!item.gameObject.activeSelf)
            {
                continue;
            }

            item.SetFill(IndexToFill(item.idx, currRate), prevRate);
        }
    }

    private void RefreshCountText()
    {
        if (countText == null)
        {
            return;
        }

        var s = string.Empty;
        var count = Mathf.CeilToInt(Mathf.Lerp(0, maxCount, currRate));
        if (maxCount > 1 && count > 0)
        {
            s = $"x{Mathf.CeilToInt(Mathf.Lerp(0, maxCount, currRate))}";
        }

        countText.SetText(s);
    }

    private int RateToIndex(float rate)
    {
        return Mathf.FloorToInt(Mathf.Lerp(0f, maxCount - 1, rate));
    }

    private float IndexToFill(int idx, float rate)
    {
        var min = idx / (float)maxCount;
        var max = (idx + 1) / (float)maxCount;
        return Mathf.Clamp01((rate - min) / (max - min));
    }

    private static Color GetColor(int idx)
    {
        var colors = GameData.COLOR.HP_BAR_COLORS;
        return colors[idx % colors.Count];
    }
}
