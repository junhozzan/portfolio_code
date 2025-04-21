using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitComponent
{
    public class UnitBuffItemBuffComponent : UnitBaseComponent
    {
        private readonly List<ResourceTargetAbility> targetAbilities = new List<ResourceTargetAbility>();
        private readonly Dictionary<int, (ResourceItem, long)> addBuffs = new Dictionary<int, (ResourceItem, long)>();
        private UnitItemComponent itemCom = null;
        private UnitBuffComponent buffCom = null;

        private static readonly Type resItemType = typeof(ResourceItem); 

        public UnitBuffItemBuffComponent(Unit owner) : base(owner)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            itemCom = GetComponent<UnitItemComponent>();
            buffCom = GetComponent<UnitBuffComponent>();
        }

        private void Clear()
        {
            addBuffs.Clear();
        }

        public void Refresh()
        {
            Clear();

            var equipedItemIDs = itemCom.GetEquipedItemIDs();
            foreach (var item in itemCom.GetAllItems())
            {
                var resItem = item.resItem;
                if (resItem == null || resItem.itemType == ItemType.WEALTH)
                {
                    continue;
                }

                targetAbilities.Clear();

                if (resItem.equip.targetAbilities.Count > 0 && equipedItemIDs.Contains(item.id))
                {
                    targetAbilities.AddRange(resItem.equip.targetAbilities);
                }

                if (resItem.hold.targetAbilities.Count > 0)
                {
                    targetAbilities.AddRange(resItem.hold.targetAbilities);
                }

                foreach (var ability in targetAbilities)
                {
                    if (ability.buffIDs.Count == 0)
                    {
                        continue;
                    }

                    if (!UnitRule.IsTargetable(owner, owner, ability.applyTargets))
                    {
                        continue;
                    }

                    foreach (var buffID in ability.buffIDs)
                    {
                        addBuffs[buffID] = (resItem, item.GetLevel());
                    }
                }

                targetAbilities.Clear();
            }

            var addBuffIDs = addBuffs.Keys.ToList();
            foreach (var buff in buffCom.GetBuffs())
            {
                if (buff.tbuff.fromResType != resItemType || addBuffIDs.Contains(buff.tbuff.id))
                {
                    continue;
                }

                buffCom.remove.AddRemoveBuffID(buff.tbuff.id);
            }

            foreach (var addBuff in addBuffs)
            {
                var res = addBuff.Value.Item1;
                var level = addBuff.Value.Item2;
                var buff = buffCom.GetBuffByID(addBuff.Key);
                if (buff != null && buff.tbuff.GetLevel() >= level)
                {
                    continue;
                }

                buffCom.add.AddBuff(owner, addBuff.Key, res, level);
            }

            Clear();
        }
    }
}