using System.Collections.Generic;

namespace UnitComponent
{
    public class UnitItemStatComponent : UnitBaseComponent
    {
        private readonly HashSet<int> setItemIDs = new HashSet<int>();
        private readonly List<StatItem.Param> statItemParams = new List<StatItem.Param>(128);

        private UnitItemComponent itemCom = null;

        public UnitItemStatComponent(Unit owner) : base(owner)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            itemCom = GetComponent<UnitItemComponent>();
        }

        public List<StatItem.Param> GetStatItemParams()
        {
            statItemParams.Clear();
            setItemIDs.Clear();

            var equipedItemIDs = itemCom.GetEquipedItemIDs();
            foreach (var item in itemCom.GetAllItems())
            {
                var resItem = item.resItem;
                if (resItem == null)
                {
                    continue;
                }

                var itemLevel = item.GetLevel();
                var itemAmount = item.GetAmount();
                var itemOptions = item.GetOptions();
                var riseParam = StatItem.Param.RiseParam.Of(itemLevel, itemAmount);
                if (equipedItemIDs.Contains(resItem.id))
                {
                    if (resItem.itemSetID != -1)
                    {
                        setItemIDs.Add(resItem.itemSetID);
                    }

                    // 장착 아이템 스탯
                    statItemParams.AddRange(StatItem.GetParams(owner, owner, resItem.equip.targetAbilities, riseParam, itemOptions));
                }

                // 보유 아이템 스탯
                statItemParams.AddRange(StatItem.GetParams(owner, owner, resItem.hold.targetAbilities, riseParam, itemOptions));
            }

            // 세트 아이템 스탯 
            foreach (var setID in setItemIDs)
            {
                var resItemSet = ResourceManager.Instance.item.GetItemSet(setID);
                if (resItemSet == null)
                {
                    continue;
                }

                var count = 0;
                foreach (var itemID in resItemSet.itemIDs)
                {
                    if (!equipedItemIDs.Contains(itemID))
                    {
                        continue;
                    }

                    ++count;
                }

                foreach (var option in resItemSet.options)
                {
                    if (option.count > count)
                    {
                        continue;
                    }

                    statItemParams.AddRange(StatItem.GetParams(owner, owner, option.targetAbilities, StatItem.Param.RiseParam.zero, null));
                }
            }

            setItemIDs.Clear();
            return statItemParams;
        }
    }
}