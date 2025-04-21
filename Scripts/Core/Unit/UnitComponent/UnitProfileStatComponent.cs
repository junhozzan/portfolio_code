using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitComponent
{
    public class UnitProfileStatComponent : UnitBaseComponent
    {
        private readonly List<StatItem.Param> statItemParams = new List<StatItem.Param>();

        public UnitProfileStatComponent(Unit owner) : base(owner)
        {

        }

        public List<StatItem.Param> GetStatItemParams()
        {
            statItemParams.Clear();
            statItemParams.AddRange(StatItem.GetStatItemListToParams(owner.core.profile.tunit.resUnit.statItems, StatItem.Param.RiseParam.Of(owner.core.profile.tunit.GetLevel(), 0), null));

            return statItemParams;
        }
    }
}