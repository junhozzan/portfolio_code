using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIToggle : MonoBehaviour
{
    [SerializeField] Toggle toggle = null;
    [SerializeField] GameObject[] on = null;
    [SerializeField] GameObject[] off = null;

    public void Init()
    {
        AddValueChanged(OnValueChanged);
    }

    public void SetOn(bool on)
    {
        // 내부적으로 같은 값이면 Set 호출이 안되어서 강제로 호출
        if (toggle.isOn == on)
        {
            toggle.onValueChanged.Invoke(on);
        }

        toggle.isOn = on;
    }

    public void AddValueChanged(UnityAction<bool> on)
    {
        toggle.onValueChanged.AddListener(on);
    }

    private void OnValueChanged(bool b)
    {
        foreach (var _ in on)
        {
            _.SetActive(b);
        }

        foreach (var _ in off)
        {
            _.SetActive(!b);
        }
    }
}
