using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UnitComponent
{
    public class UnitSkillBonusComponent : UnitBaseComponent
    {
        private static IReadOnlyList<ResourceSkillBonus.SkillBonusType> cachedBonusTypes = Util.GetEnumArray<ResourceSkillBonus.SkillBonusType>()
            .Where(x => x != ResourceSkillBonus.SkillBonusType.NONE)
            .ReadOnly();

        private readonly Dictionary<ResourceSkillBonus.SkillBonusType, List<(ResourceSkillBonus, long)>> cachedSkillBonuses = 
            new Dictionary<ResourceSkillBonus.SkillBonusType, List<(ResourceSkillBonus, long)>>();

        private readonly List<(ResourceSkillBonus, long)> tempSkillBonuses = new List<(ResourceSkillBonus, long)>();

        public UnitSkillBonusComponent(Unit owner) : base(owner)
        {

        }

        public void Refresh()
        {
            cachedSkillBonuses.Clear();
            foreach (var type in cachedBonusTypes)
            {
                var bonuses = CreateSkillBonuses(type);
                if (!cachedSkillBonuses.TryGetValue(type, out var v))
                {
                    cachedSkillBonuses.Add(type, v = new List<(ResourceSkillBonus, long)>());
                }

                v.Clear();
                v.AddRange(bonuses);
            }

            tempSkillBonuses.Clear();
        }

        private List<(ResourceSkillBonus, long)> CreateSkillBonuses(ResourceSkillBonus.SkillBonusType type)
        {
            tempSkillBonuses.Clear();

            var equipedItemIDs = owner.core.item.GetEquipedItemIDs();
            foreach (var item in owner.core.item.GetAllItems())
            {
                var resItem = item.resItem;
                if (resItem == null)
                {
                    continue;
                }

                var itemLevel = item.GetLevel();
                if (equipedItemIDs.Contains(resItem.id) && resItem.equip.targetAbilities.Count > 0)
                {
                    InputSkillBonus(type, owner, resItem.equip.targetAbilities, itemLevel);
                }

                if (resItem.hold.targetAbilities.Count > 0)
                {
                    InputSkillBonus(type, owner, resItem.hold.targetAbilities, itemLevel);
                }
            }

            foreach (var buff in owner.core.buff.buffScript.GetBuffsByScriptType(BuffScriptType.SKILL_BONUS))
            {
                var buffLevel = buff.tbuff.GetLevel();
                foreach (var script in buff.GetBuffScripts(BuffScriptType.SKILL_BONUS))
                {
                    InputSkillBonus(type, owner, script.GetTargetAbilities(), buffLevel);
                }
            }

            return tempSkillBonuses;
        }

        private void InputSkillBonus(ResourceSkillBonus.SkillBonusType type, Unit from, ICollection<ResourceTargetAbility> targetAbilities, long level)
        {
            if (targetAbilities == null)
            {
                return;
            }

            foreach (var ability in targetAbilities)
            {
                if (!UnitRule.IsTargetable(from, owner, ability.applyTargets))
                {
                    continue;
                }

                foreach (var bonus in ability.skillBonuses)
                {
                    if (bonus.bonusType != type)
                    {
                        continue;
                    }

                    tempSkillBonuses.Add((bonus, level));
                }
            }
        }

        public List<(ResourceSkillBonus, long)> Get(ResourceSkillBonus.SkillBonusType type)
        {
            return cachedSkillBonuses.TryGetValue(type, out var v) ? v : null;
        }
    }
}