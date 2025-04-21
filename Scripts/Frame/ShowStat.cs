using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class ShowStat
{
    private static readonly List<(StatType, TextParam)> statTextParams = new List<(StatType, TextParam)>(0);
    private static readonly List<TextParam> tempTextParams = new List<TextParam>(0);
    private static readonly List<ResourceTargetAbility> tempItemBuffTargetAbilities = new List<ResourceTargetAbility>();
    private static readonly Dictionary<int, long> tempItemBuffs = new Dictionary<int, long>();
    private static readonly List<(string, Color)> result = new List<(string, Color)>();

    private static readonly ReadOnlyCollection<StatType> longTypeStats = new List<StatType>()
    {
        StatType.MAXHP,
        StatType.MAXMP,
        StatType.POWER,
        StatType.POWER_DARK,
        StatType.POWER_HOLY,

    }.ReadOnly();

    const string COLUMN = "<size=14>◈</size>\u00A0";

    public static List<(string, Color)> GetShowStats(Unit unit)
    {
        result.Clear();
        statTextParams.Clear();

        if (!UnitRule.IsValid(unit))
        {
            return result;
        }

        unit.core.refresh.RefreshImmediate();
        var stat = unit.core.stat.GetStat();
        foreach (var type in Stat.types)
        {
            var value = stat.Get(type);
            if (value == 0f)
            {
                continue;
            }

            var sValue = string.Empty;
            if (StatItem.mapIncreaseBaseStats.TryGetValue(type, out var baseStatType))
            {
                var baseValue = stat.Get(baseStatType);
                if (baseValue == 0f)
                {
                    continue;
                }

                var v = longTypeStats.Contains(baseStatType) ? (long)(baseValue * value) : baseValue * value;
                sValue = $"{StatItem.ValueToString(type, value, false)} (+{v})";
            }
            else
            {
                sValue = StatItem.ValueToString(type, value, false);
            }

            statTextParams.Add((type, TextParam.Of(Color.white, $"<color=#{GameData.COLOR.STAT_TITLE.hex}>{StatItem.TypeToLocailzeKey(type)}:</color> {sValue}")));
        }

        if (statTextParams.Count == 0)
        {
            return result;
        }

        result.Add(($"[{"key_current_stat".L()}]", Color.white));
        foreach (var param in statTextParams)
        {
            result.Add((param.Item2.text, param.Item2.color));
        }

        statTextParams.Clear();
        
        return result;
    }

    public static List<(string, Color)> GetShowSpecialAbilities(Unit unit)
    {
        result.Clear();
        tempItemBuffs.Clear();
        tempTextParams.Clear();

        var items = unit.core.item.GetAllItems();
        var equipedItemIDs = unit.core.item.GetEquipedItemIDs();

        foreach (var item in items)
        {
            var resItem = item.resItem;
            if (resItem == null)
            {
                continue;
            }

            tempItemBuffTargetAbilities.Clear();

            if (equipedItemIDs.Contains(item.id))
            {
                tempItemBuffTargetAbilities.AddRange(resItem.equip.targetAbilities);
            }

            tempItemBuffTargetAbilities.AddRange(resItem.hold.targetAbilities);

            foreach (var ability in tempItemBuffTargetAbilities)
            {
                if (ability.buffIDs.Count == 0)
                {
                    continue;
                }

                if (!UnitRule.IsTargetable(unit, unit, ability.applyTargets))
                {
                    continue;
                }

                foreach (var buffID in ability.buffIDs)
                {
                    var itemLevel = item.GetLevel();
                    if (!tempItemBuffs.TryGetValue(buffID, out var level))
                    {
                        tempItemBuffs.Add(buffID, itemLevel);
                    }
                    else
                    {
                        if (itemLevel > level)
                        {
                            tempItemBuffs[buffID] = itemLevel;
                        }
                    }
                }
            }
        }

        foreach (var buffs in tempItemBuffs)
        {
            var resBuff = ResourceManager.Instance.buff.GetBuff(buffs.Key);
            if (resBuff == null)
            {
                continue;
            }

            var name = resBuff.GetName(buffs.Value);
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            tempTextParams.Add(TextParam.Of(GameData.COLOR.ITEM_SPECIAL_TEXT, $"{COLUMN}{name}"));
        }

        tempItemBuffTargetAbilities.Clear();
        tempItemBuffs.Clear();

        if (tempTextParams.Count == 0)
        {
            return result;
        }

        result.Add(($"[{"key_special_ability".L()}]", GameData.COLOR.ITEM_SPECIAL_TEXT));
        foreach (var param in tempTextParams)
        {
            result.Add((param.text, param.color));
        }

        tempTextParams.Clear();

        return result;
    }
}
