using UnityEngine;

public class CpUI_CostButton : MonoBehaviour
{
    [SerializeField] UIImage icon = null;
    [SerializeField] UIText valueText = null;
    [SerializeField] GameObject button = null;

    public void DoReset()
    {
        icon.gameObject.SetActive(false);
    }

    public void SetIcon(ResourceItem.Appearance appearance)
    {
        if (!appearance.use)
        {
            return;
        }

        icon.gameObject.SetActive(true);
        icon.SetSprite(appearance.atlas, appearance.sprite);
    }

    public void SetValueText(string text, Color color)
    {
        valueText.SetText(text);
        valueText.SetTextColor(color);
        //valueText.SetFitterHorizontal();
    }

    public GameObject GetButton()
    {
        return button;
    }
}
