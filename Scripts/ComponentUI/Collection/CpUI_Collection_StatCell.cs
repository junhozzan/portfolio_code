using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UICollection
{
    public class CpUI_Collection_StatCell : MyOSABasicItem
    {
        [SerializeField] UIText statText = null;
        [SerializeField] UIText valueText = null;

        public override void DoReset()
        {

        }

        public override void Refresh()
        {

        }

        public override void UpdateViews(MyOSABasic.IOsaItem tOsaItem)
        {
            var osaItem = tOsaItem as CpUI_Collection_Stat.StatOsaItem;
            if (osaItem == null)
            {
                return;
            }

            statText.SetText(StatItem.TypeToLocailzeKey(osaItem.stat));
            valueText.SetText(StatItem.ValueToString(osaItem.stat, osaItem.value));
        }

        public void SetTexts(string stat, string value)
        {
            statText.SetText(stat);
            valueText.SetText(value);
        }
    }
}