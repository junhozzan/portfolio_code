using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIText : UIBase
{
    public static readonly List<UIText> a_list = new List<UIText>();

    [SerializeField] Text m_text = null;
    [SerializeField] CustomOutline m_outline = null;
    [SerializeField] UIGradient m_gradient = null;
    [SerializeField] UITextFitter fitter = null;
    [SerializeField] string m_textKey = string.Empty;

    private Cmd cmd = null;

    public RectTransform _textRect
    {
        get
        {
            return m_text.rectTransform;
        }
    }

    public string _strText
    {
        get
        {
            if (m_text == null)
            {
                return string.Empty;
            }

            return m_text.text;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        a_list.Add(this);
    }

    protected override void OnDestroy()
    {
        if (a_list.Contains(this))
        {
            a_list.Remove(this);
        }
    }

    public void UseCmd(Action<int, GameObject> on)
    {
        if (cmd != null)
        {
            return;
        }

        cmd = Cmd.Add(m_text.gameObject, eCmdTrigger.OnClick, on);
    }

    public void SetCmdKey(int key)
    {
        if (cmd == null)
        {
            return;
        }

        cmd.SetKey(key);
    }

    public void SetText(string str)
    {
        if (m_text == null)
        {
            return;
        }

        m_text.text = str;

        if (fitter != null)
        {
            fitter.SetFitter(m_text);
        }
    }

    public void SetFontSize(int size)
    {
        if (m_text == null)
        {
            return;
        }

        m_text.fontSize = size;
    }

    public void SetTextColor(Color color)
    {
        if (m_text == null)
        {
            return;
        }
     
        m_text.color = color;
    }

    public void SetOutlineColor(Color color)
    {
        if (m_outline == null)
        {
            return;
        }

        m_outline.SetColor(color);
    }

    public void SetGradient(Color top, Color bottom)
    {
        if (m_gradient == null)
        {
            return;
        }

        m_gradient.Set(top, bottom);
    }

    public void SetScale(float s)
    {
        transform.localScale = Vector3.one * s;
    }

    public void SetTextKey(string key)
    {
        m_textKey = key;
        SetLocalize();
    }

    public void SetLocalize()
    {
        if (string.IsNullOrEmpty(m_textKey))
        {
            return;
        }

        SetText(Localize.L(m_textKey));
    }

    public Vector2 GetRectSize()
    {
        if (m_text == null)
        {
            return Vector2.zero;
        }

        return m_text.rectTransform.rect.size;
    }

    private void OnEnable()
    {
        SetLocalize();
    }
}

public struct TextParam
{
    public Color color;
    public string text;

    public static TextParam empty = Of(Color.white, string.Empty);

    public static TextParam Of(Color color, string text)
    {
        return new TextParam()
        {
            color = color,
            text = text
        };
    }

}
