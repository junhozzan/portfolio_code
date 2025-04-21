using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIcon
{
    string GetName();
    Atlas GetSpriteAtlas();
    string GetSpriteName();
    Color GetBackgroundColor();
    string GetAmountText();

    bool IsShowToolTip();
    string GetToolTipString();
}
