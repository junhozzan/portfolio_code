using System.Collections.ObjectModel;
using UnityEngine;

public class CpUI_MonsterHpSlider : MonoBehaviour
{
    //[SerializeField] DNFHealthBar healthBar = null;
    [SerializeField] DNFHealthBarV2 healthBar = null;
    [SerializeField] RectTransform rectTransform = null;
    [SerializeField] UIText infoText = null;

    private ObjectPool<UIText> infoTextPool = null;
    private long unitUID = -1;

    private ReadOnlyCollection<(StatType, Color)> viewRisistStats = null;

    private void Awake()
    {
        viewRisistStats = new (StatType, Color)[]
        {
            (StatType.DAMAGED_DEC, Color.white),
            (StatType.RESIST_NONE, Color.white),
            (StatType.RESIST_HOLY, Color.white),
            (StatType.RESIST_DARK, Color.white),
            (StatType.CRITICAL_RESIST, Color.white),
            (StatType.CRITICAL_DAMAGE_RESIST, Color.white),
        }.ReadOnly();

        infoTextPool = ObjectPool<UIText>.Of(infoText, infoText.transform.parent);
    }

    public void DoReset()
    {
        unitUID = -1;
    }

    private void SetInfos(long uid)
    {
        var unit = UnitManager.Instance.GetUnitByUID(uid);
        if(unit == null)
        {
            return;
        }

        infoTextPool.Clear();
        SetNameText(unit);
        SetResistStats(unit);
    }

    private void SetNameText(Unit unit)
    {
        var str = string.Empty;
#if UNITY_EDITOR
        str = $"{unit.core.profile.tunit.resUnit.GetName()}({unit.core.profile.tunit.uid})";
#else
        str = unit.core.profile.tunit.resUnit.GetName();
#endif
        infoTextPool.Pop().SetText(str);
    }

    private void SetResistStats(Unit unit)
    {
        foreach (var t in viewRisistStats)
        {
            var v = unit.core.stat.GetStat().Get(t.Item1);
            if (v == 0)
            {
                continue;
            }

            var text = infoTextPool.Pop();
            text.SetText(StatItem.TypeToLocailzeKey(t.Item1));
            text.SetTextColor(t.Item2);
        }
    }

    private void SetBarSize(float barSize)
    {
        var resize = rectTransform.rect.size;
        resize.x = barSize;
        rectTransform.sizeDelta = resize;
    }

    public void SetShadowFill(bool isFisthit, long uid, float maxHp, float prevHp, float hp, float countValue, float hpBarSize)
    {
        if (isFisthit && unitUID != uid)
        {
            SetBarSize(hpBarSize);
            SetInfos(uid);
            unitUID = uid;
            healthBar.Init(maxHp, prevHp, countValue);

        }

        if (unitUID == uid)
        {
            healthBar.SetHP(maxHp, hp);
        }
    }

    public void SetHeal(long uid, float maxHp, float hp, float countValue)
    {
        if (unitUID != uid)
        {
            return;
        }

        healthBar.Init(maxHp, hp, countValue);
        healthBar.SetHP(maxHp, hp);
    }
}
