using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UICollection
{
    public class CpUI_Collection_Stat : UIBase
    {
        [SerializeField] MyOSABasic osaScroll = null;

        private readonly MyOSABasic.OsaPool<StatOsaItem> osaPool = new MyOSABasic.OsaPool<StatOsaItem>();
        private readonly List<MyOSABasic.IOsaItem> sortOsaItems = new List<MyOSABasic.IOsaItem>();

        private readonly Stat appliedStat = Stat.Of();

        public void Init()
        {
            osaScroll.Init(null);
        }

        public void On()
        {
            Refresh();
        }

        public void Refresh()
        {
            appliedStat.DoReset();
            osaPool.DoReset();
            sortOsaItems.Clear();

            var statParams = MyPlayer.Instance.core.collection.GetStatItemParams(MyUnit.Instance);
            foreach (var param in statParams)
            {
                appliedStat.AddStat(param.statItem, param.riseParam, param.rate);
            }

            foreach (var type in Stat.types)
            {
                var value = appliedStat.Get(type);
                if (value == 0f)
                {
                    continue;
                }

                var item = osaPool.Pop();
                item.Set(type, value);

                sortOsaItems.Add(item);
            }

            osaScroll.SetItems(sortOsaItems);
        }

        public class StatOsaItem : MyOSABasic.IOsaItem
        {
            public StatType stat { get; private set; } = StatType.NONE;
            public float value { get; private set; } = 0f;

            public void DoReset()
            {
                stat = StatType.NONE;
                value = 0f;            
            }

            public void Set(StatType stat, float value)
            {
                this.stat = stat;
                this.value = value;
            }

            public bool IsEmpty()
            {
                return stat == StatType.NONE;
            }

            public int SortCompare(MyOSABasic.IOsaItem other)
            {
                return 0;
            }
        }
    }
}