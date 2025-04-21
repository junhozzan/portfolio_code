using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections.ObjectModel;
using System;

public class Ability
{
    private float[] values { get; } = new float[(int)eAbility.CNT];

    private static readonly ReadOnlyDictionary<eAbility, StatType> convertToStats = Util.GetEnumArray<eAbility>()
        .Where(x => x > eAbility.NONE && x < eAbility.CNT)
        .ToDictionary(x => x, x => x.ToString().ToENUM(StatType.NONE))
        .ReadOnly();

    public static Ability Of()
    {
        return new Ability();
    }

    private Ability()
    {
        
    }

    public void DoReset()
    {
        for (int i = 0, len = values.Length; i < len; ++i)
        {
            values[i] = 0f;
        }
    }

    public float Get(eAbility ability)
    {
        if (ability < eAbility.NONE || ability >= eAbility.CNT)
        {
            return 0f;
        }

        return values[Util.EnumToInt(ability)];
    }

    public void Copy(Ability copy)
    {
        for (int i = 0, len = values.Length; i < len; ++i)
        {
            values[i] = copy.values[i];
        }
    }

    public void CalcAbilty(Stat stat)
    {
        // 능력치 계산
        for (eAbility e = eAbility.NONE; e < eAbility.CNT; ++e)
        {
            var value = 0.0f;
            switch (e)
            {
                case eAbility.POWER:
                    var power = stat.Get(StatType.POWER);
                    power += power * stat.Get(StatType.POWER_INC);
                    value = Mathf.Max(Mathf.Floor(power), 0f);
                    break;

                case eAbility.POWER_HOLY:
                    var holyPower = stat.Get(StatType.POWER_HOLY);
                    holyPower += holyPower * stat.Get(StatType.POWER_HOLY_INC);
                    value = Mathf.Max(Mathf.Floor(holyPower), 0f);
                    break;

                case eAbility.POWER_DARK:
                    var darkPower = stat.Get(StatType.POWER_DARK);
                    darkPower += darkPower * stat.Get(StatType.POWER_DARK_INC);
                    value = Mathf.Max(Mathf.Floor(darkPower), 0f);
                    break;

                case eAbility.DEFENCE:
                    var defence = stat.Get(StatType.DEFENCE);
                    defence += defence * stat.Get(StatType.DEFENCE_INC);
                    value = Mathf.Max(defence, 0f);
                    break;

                case eAbility.MAXHP:
                    var hp = Mathf.Max(stat.Get(StatType.MAXHP));
                    hp += hp * stat.Get(StatType.MAXHP_INC);
                    value = Mathf.Max(hp, 0f);
                    break;

                case eAbility.MAXMP:
                    var mp = Mathf.Max(stat.Get(StatType.MAXMP));
                    mp += mp * stat.Get(StatType.MAXMP_INC);
                    value = Mathf.Max(mp, 0f);
                    break;

                case eAbility.MOVESPEED:
                    value = Mathf.Max(stat.Get(StatType.MOVESPEED) * (1f + stat.Get(StatType.MOVESPEED_INC)), 0f);
                    break;

                case eAbility.DAMAGED_DEC:
                    value = Mathf.Min(stat.Get(StatType.DAMAGED_DEC), 0.9f);
                    break;

                default:
                    if (convertToStats.TryGetValue(e, out var estat))
                    {
                        value = stat.Get(estat);
                    }
                    break;
            }

            values[Util.EnumToInt(e)] = value;
        }
    }
}

public class Stat
{
    public static readonly StatType[] types = Enum.GetValues(typeof(StatType))
        .Cast<StatType>()
        .Where(x => x > StatType.NONE && x < StatType.CNT)
        .ToArray();

    private static readonly ReadOnlyCollection<StatType> nonZeroUnderStats = new List<StatType>()
    {
        StatType.POWER,
        StatType.POWER_DARK,
        StatType.POWER_HOLY,
        StatType.DEFENCE,
        StatType.MAXHP,
        StatType.MAXMP,
        StatType.MOVESPEED,
    }.ReadOnly();


    private readonly Dictionary<StatType, float> stats = null;

    public static Stat Of()
    {
        return new Stat();
    }

    private Stat()
    {
        stats = new Dictionary<StatType, float>();
        foreach (var stat in types)
        {
            stats.Add(stat, 0f);
        }
    }

    public void DoReset()
    {
        ClearStats();
    }

    private void ClearStats()
    {
        foreach (var stat in types)
        {
            stats[stat] = 0f;
        }
    }

    public Stat AddStat(StatItem statItem, StatItem.Param.RiseParam riseParam, float rate)
    {
        if (statItem == null)
        {
            return this;
        }

        if (!stats.ContainsKey(statItem.stat))
        {
            stats.Add(statItem.stat, 0f);
        }

        stats[statItem.stat] += statItem.GetValue(riseParam, rate);
        return this;
    }

    public float Get(StatType stat)
    {
        if (!stats.TryGetValue(stat, out var value))
        {
            return 0f;
        }

        // 0 미만으로 떨어졌을때 보정
        if (nonZeroUnderStats.Contains(stat) && value < 0f)
        {
            return 0f;
        }

        return value;
    }

    public void Copy(Stat origin)
    {
        ClearStats();

        foreach (var kv in origin.stats)
        {
            stats[kv.Key] = kv.Value;
        }
    }
}

public enum StatType
{
    NONE,
    POWER, // 공격력
    POWER_INC, // 공격력 증가
    POWER_HOLY, // 신성력
    POWER_HOLY_INC, // 신성력 증가
    POWER_DARK, // 암흑력
    POWER_DARK_INC, // 암흑력 증가
    DEFENCE, // 방어력
    DEFENCE_INC, // 방어력 증가
    MAXHP, // 최대HP
    MAXHP_INC, // 체력 증가

    MAXMP, // 최대MP
    MAXMP_INC, //마력 증가
    RECOVERY_MP, //마력 회복(초당)

    CRITICAL_CHANCE, // 치명타 확률
    CRITICAL_RESIST, // 치명타 저항

    CRITICAL_DAMAGE, // 치명타 피해
    CRITICAL_DAMAGE_RESIST, // 치명타 피해 저항

    DAMAGE_INC, // 피해 증가
    DAMAGED_DEC, // 피해 감소

    RESIST_NONE, // 물리 피해 저항
    RESIST_HOLY, // 신성 피해 저항
    RESIST_DARK, // 암흑 피해 저항

    MOVESPEED, // 이동 속도
    MOVESPEED_INC, // 이동 속도 증가 %
    DAMAGE_FIXED, // 고정 대미지

    ATTACK_SPEED_INC, // 공격 속도 증가 (표시용)
    MERGE_CHARGE_CELL, // 머지 셀 개수
    MERGE_CHARGE_SPEED_INC, // 셀 생성 속도
    MERGE_CREATE_SPEED_INC, // 1단계 생성 속도
    MERGE_NEXT_SPEED_INC, // 합성 속도

    CNT
}

public enum eAbility
{
    NONE,
    POWER, // 공격력
    POWER_HOLY, // 신성력
    POWER_DARK, // 암흑력
    DEFENCE, // 방어율
    MAXHP, // 최대HP
    MAXMP, // 최대MP
    RECOVERY_MP, // 초당MP회복

    CRITICAL_CHANCE, // 치명타 확률
    CRITICAL_RESIST, // 치명타 저항

    CRITICAL_DAMAGE, // 치명타 피해
    CRITICAL_DAMAGE_RESIST, // 치명타 피해 저항

    DAMAGE_INC, // 피해 증가
    DAMAGED_DEC, // 피해 감소

    RESIST_NONE, // 물리 피해 저항
    RESIST_HOLY, // 신성 피해 저항
    RESIST_DARK, // 암흑 피해 저항

    MOVESPEED, // 이동 속도
    DAMAGE_FIXED, // 고정 대미지


    ATTACK_SPEED_INC, // 공격 속도 증가 (표시용)
    MERGE_CHARGE_CELL,
    MERGE_CHARGE_SPEED_INC,
    MERGE_CREATE_SPEED_INC,
    MERGE_NEXT_SPEED_INC,

    CNT
}