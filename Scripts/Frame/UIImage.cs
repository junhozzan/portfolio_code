using UnityEngine;
using UnityEngine.UI;

public class UIImage : MonoBehaviour
{
    [SerializeField] Image image = null;
    [SerializeField] UIGradient gradient = null;

    private Image grayscaleImg = null;

    public void SetSprite(Atlas atlas, string spriteName)
    {
        var sprite = GetAtlasSprite(atlas, spriteName);
        if (sprite == null)
        {
            sprite = GetNoneSprite();
        }

        if (image != null)
        {
            image.gameObject.SetActive(true);
            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
        }

        if (grayscaleImg != null)
        {
            grayscaleImg.sprite = sprite;
            grayscaleImg.type = Image.Type.Simple;
            grayscaleImg.preserveAspect = true;
        }
    }

    private Sprite GetAtlasSprite(Atlas atlas, string spriteName)
    {
        return AtlasManager.Instance.GetSprite(atlas, spriteName);
    }

    private Sprite GetNoneSprite()
    {
        return AtlasManager.Instance.GetSprite(Atlas.UI_BASE, "none");
    }

    public void SetGrayscale(bool bUse)
    {
        if (bUse && grayscaleImg == null)
        {
            // image 오브젝트를 부모로 설정하여 생성
            grayscaleImg = UIManager.CreateEmpty(image.transform).AddComponent<Image>();
            grayscaleImg.transform.SetAsFirstSibling();
            grayscaleImg.raycastTarget = image.raycastTarget;
            grayscaleImg.preserveAspect = image.preserveAspect;
            grayscaleImg.sprite = image.sprite;
            grayscaleImg.type = image.type;
            //grayscaleImg.material = a_matGrayscale;

        }

        if (image != null)
        {
            image.enabled = !bUse;
        }

        if (grayscaleImg != null)
        {
            grayscaleImg.enabled = bUse;
        }
    }

    public void SetColor(Color col)
    {
        if (image != null)
        {
            image.color = col;
        }
    }

    public void SetAlpha(float a)
    {
        if (image != null)
        {
            var col = image.color;
            col.a = a;
            image.color = col;
        }
    }

    public void SetGradient(Color top, Color bottom)
    {
        if (gradient != null)
        {
            gradient.Set(top, bottom);
        }
    }

    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }

    public void SetFillAmount(float fill)
    {
        image.fillAmount = fill;
    }

    public void SetNativeSize()
    {
        image.SetNativeSize();
    }

    public RectTransform GetImageRect()
    {
        return image.rectTransform;
    }

    public float GetAlpha()
    {
        return image.color.a;
    }
}
