using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.ForbiddenByte.OSA;

public class CpUI_LogList : UIMonoBehaviour
{
    private static CpUI_LogList instance = null;
    public static CpUI_LogList Instance
    {
        get
        {
            if (instance == null)
            {
                instance = UIManager.Instance.Find<CpUI_LogList>("pf_ui_loglist");
            }

            return instance;
        }
    }

    [SerializeField] MyOSABasic osaScroll = null;
    private MyOSABasic.OsaPool<LogOsaItem> osaPool = new MyOSABasic.OsaPool<LogOsaItem>();
    private List<MyOSABasic.IOsaItem> sortOsaItems = new List<MyOSABasic.IOsaItem>();

    private List<string> logs = new List<string>();

    public override void Init()
    {
        base.Init();
        SetCanvas(UIManager.eCanvans.LAST, true);

        if (osaScroll != null)
        {
            osaScroll.Init(null);
        }
    }

    private void AddLog(string log)
    {
        Show(true);

        if (logs.Count > 50)
        {
            logs.Clear();
        }

        logs.Add(log);

        osaPool.DoReset();
        sortOsaItems.Clear();

        for (int i = 0; i < logs.Count; ++i)
        {
            var item = osaPool.Pop(i);
            item.log = logs[i];

            sortOsaItems.Add(item);
        }

        osaScroll.SetItems(sortOsaItems);
        osaScroll.Scroll(logs.Count - 1);
    }

    public static void ShowLog(string log)
    {
        //CpUI_LogList.Instance.AddLog(log);
    }

    public class LogOsaItem : MyOSABasic.IOsaItem
    {
        public string log = string.Empty;

        public void DoReset()
        {
            log = string.Empty;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(log);
        }

        public int SortCompare(MyOSABasic.IOsaItem other)
        {
            return 0;
        }
    }
}
