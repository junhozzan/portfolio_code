using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIAmountSlider : UIBase
{
    [SerializeField] Slider slider = null;
    [SerializeField] RectTransform range = null;
    [SerializeField] RectTransform point = null;
    [SerializeField] Text pointAmountText = null;

    private List<Section> sections = new List<Section>();
    private Section? section = null;
    private long maxAmount = 0;

    private Action<long> externalValueChange = null;

    protected override void Awake()
    {
        base.Awake();
        slider.onValueChanged.AddListener(OnValueChange);
    }

    public void DoReset()
    {
        section = null;
        sections.Clear();
        maxAmount = 0;
        slider.enabled = false;
        point.gameObject.SetActive(false);
    }

    public void SetExternalValueChange(Action<long> valueChange)
    {
        externalValueChange = valueChange;
    }

    public void SetMax(long max)
    {
        sections.Clear();

        maxAmount = max;
        if (maxAmount == 0)
        {
            return;
        }

        slider.enabled = true;
        point.gameObject.SetActive(true);

        var w = range.rect.width * 0.5f;
        var minPos = new Vector2(-w, 0f);
        var maxPos = new Vector2(w, 0f);

        var n = maxAmount - 1;
        var halfRate = 0.5f / n;
        for (long i = 0; i <= n; ++i)
        {
            var rate = n > 0 ? (i / (float)n) : 0f;
            sections.Add(new Section() { range = rate + halfRate, point = Vector2.Lerp(minPos, maxPos, rate), amount = i + 1 });
        }

        SetAmount(0f);
    }

    private void OnValueChange(float f)
    {
        SetAmount(f);
    }

    private void SetAmount(float f)
    {
        Section? catched = null;
        for (int i = 0, cnt = sections.Count; i < cnt; ++i)
        {
            var section = sections[i];
            if (f <= section.range)
            {
                catched = section;
                break;
            }
        }

        section = catched;
        if (!section.HasValue)
        {
            return;
        }

        point.localPosition = section.Value.point;
        externalValueChange?.Invoke(section.Value.amount);

        if (pointAmountText != null)
        {
            pointAmountText.text = ($"{section.Value.amount}/{maxAmount}");
        }
    }

    public void AddAmount(int add)
    {
        var amount = Math.Min(Math.Max(GetAmount() + add, 1), GetMaxAmount());
        var cached = 0f;
        for (int i = 0; i < sections.Count; ++i)
        {
            var section = sections[i];
            if (amount <= section.amount)
            {
                cached = section.range;
                break;
            }
        }

        SetAmount(cached);
    }

    public long GetAmount()
    {
        if (!section.HasValue)
        {
            return 1;
        }

        return section.Value.amount;
    }

    public long GetMaxAmount()
    {
        return maxAmount;
    }

    public struct Section
    {
        public float range;
        public Vector2 point;
        public long amount;
    }
}
