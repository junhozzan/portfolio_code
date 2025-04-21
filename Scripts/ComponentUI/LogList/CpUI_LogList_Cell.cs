using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CpUI_LogList_Cell : MyOSABasicItem
{
    [SerializeField] Text text = null;

    public override void DoReset()
    {

    }

    public override void Refresh()
    {

    }

    public override void UpdateViews(MyOSABasic.IOsaItem tOsaItem)
    {
        if (!(tOsaItem is CpUI_LogList.LogOsaItem osaItem))
        {
            return;
        }

        if (text == null)
        {
            return;
        }

        text.text = osaItem.log;
    }
}
