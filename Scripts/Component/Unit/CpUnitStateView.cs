using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpUnitStateView : MonoBehaviour
{
    [TextArea(1, 26)]
    public string TextBox = string.Empty;

    public void SetString(string str)
    {
        TextBox = str;
    }
}
