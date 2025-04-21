using UnityEngine;
using UnityEngine.UI;

public class UIRadioButton : MonoBehaviour
{
    [SerializeField] UIRadio parent = null;
    [SerializeField] Button button = null;
    [SerializeField] GameObject on = null;
    [SerializeField] GameObject off = null;
    [SerializeField] GameObject notice = null;

    private void Awake()
    {
        Cmd.Add(button.gameObject, eCmdTrigger.OnClick, Cmd_Click);
        
        if (parent != null)
        {
            parent.Enroll(this);
        }
    }

    private void Cmd_Click()
    {
        parent.OnClickButton(this);
    }

    public void On(bool isOn)
    {
        on?.SetActive(isOn);
        off?.SetActive(!isOn);
    }
}
