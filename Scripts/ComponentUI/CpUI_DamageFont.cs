using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CpUI_DamageFont : CpObject
{
    [Header("_DAMAGE FONT_")]
    [SerializeField] UIText maintext = null;

    private static List<CpUI_DamageFont> upFonts = new List<CpUI_DamageFont>();

    public override eObjectLocation _objectLocation => eObjectLocation.UI_DamageFont;
    private long hitUnitUID = 0; // 상승 이동 체크 변수 1

    const float UP_POSITION = 0.22f;

    protected override void Init()
    {
        base.Init();

        if (!upFonts.Contains(this))
        {
            upFonts.Add(this);
        }
    }

    protected override void InitSortinGroup()
    {
        // empty
    }

    public override void DoReset()
    {
        base.DoReset();

        if (trPosition != null)
        {
            trPosition.localPosition = Vector2.zero;
        }

        if (maintext != null)
        {
            maintext.SetTextColor(Color.white);
        }
    }

    public void UpPosition()
    {
        var upPos = Vector3.zero;
        if (maintext.gameObject.activeSelf)
        {
            upPos.y += UP_POSITION;
        }

        for (int i = 0, cnt = upFonts.Count; i < cnt; ++i)
        {
            var fontObj = upFonts[i];
            if (fontObj == null
                || !fontObj._isActive
                || fontObj == this
                || fontObj.hitUnitUID == 0
                || fontObj.hitUnitUID != this.hitUnitUID
                )
            {
                continue;
            }

            fontObj.AddPosition(upPos);
        }
    }

    public void SetText(string str)
    {
        if (maintext == null)
        {
            return;
        }

        maintext.SetText(str);
    }

    public void SetTextColor((Color, Color) colors)
    {
        if (maintext == null)
        {
            return;
        }

        maintext.SetGradient(colors.Item1, colors.Item2);
    }

    public void SetCritical(bool critical)
    {
        if (maintext == null)
        {
            return;
        }

        if (!critical)
        {
            return;
        }

        maintext.SetTextColor(Color.red);
    }

    public static CpUI_DamageFont ShowFont(long targetUID, Vector3 pos, string text)
    {
        var damageFont = ObjectManager.Instance.Pop<CpUI_DamageFont>(GameData.PREFAB.DAMAGE_FONT);
        damageFont.hitUnitUID = targetUID;
        damageFont.SetPosition(pos);
        damageFont.SetAsLastSibling();
        damageFont.SetText(text);

        return damageFont;
    }
}
