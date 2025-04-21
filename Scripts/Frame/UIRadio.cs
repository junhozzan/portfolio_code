using System.Collections.Generic;
using UnityEngine;
using System;

public class UIRadio : UIBase
{
    private readonly List<UIRadioButton> buttons = new List<UIRadioButton>();
    private Action<GameObject> evnt = null;

    // 현재 활성화 버튼
    public GameObject current { get; private set; } = null;

    public void Init(Action<GameObject> evnt)
    {
        this.evnt = evnt;
    }

    public void Enroll(UIRadioButton button)
    {
        if (buttons.Contains(button))
        {
            return;
        }

        buttons.Add(button);

        RefreshButtons();
    }

    public void OnClickButton(UIRadioButton button)
    {
        Enroll(button);
        OnPress(button.gameObject);
    }

    public void Choice(GameObject go)
    {
        OnPress(go);
    }

    private void OnPress(GameObject go)
    {
        if (current != null && current == go)
        {
            return;
        }

        current = go;
        evnt?.Invoke(current);

        RefreshButtons();
    }

    private void RefreshButtons()
    {
        foreach (var button in buttons)
        {
            button.On(current == button.gameObject);
        }
    }
}