using System.Collections.Generic;

namespace UnitComponent
{
    public class AssistUnitItemComponent : UnitItemComponent
    {
        private readonly new AssistUnit owner = null;

        public AssistUnitItemComponent(Unit owner) : base(owner)
        {
            this.owner = owner as AssistUnit;
        }

        public override ICollection<TItem> GetItems()
        {
            return owner.core.profile.summoner.core.item.GetItems();
        }

        public override List<int> GetEquipedItemIDs()
        {
            return owner.core.profile.summoner.core.item.GetEquipedItemIDs();
        }
    }
}