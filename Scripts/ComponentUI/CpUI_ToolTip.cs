using UnityEngine;

public class CpUI_ToolTip : UIMonoBehaviour
{
    private static CpUI_ToolTip instance = null;
    public static CpUI_ToolTip Instance
    {
        get
        {
            if (instance == null)
            {
                instance = UIManager.Instance.Find<CpUI_ToolTip>("pf_ui_tooltip");
            }

            return instance;
        }
    }

    [SerializeField] UIText text = null;

    public override void Init()
    {
        base.Init();
        SetCanvas(UIManager.eCanvans.LAST, true);
        UsingBlind(true, true, 100);
    }

    public void On(string s, Vector2 pos)
    {
        if (!UIManager.Instance.Show(this))
        {
            return;
        }
        
        text.transform.position = pos;
        text.SetText(s);
    }
}
