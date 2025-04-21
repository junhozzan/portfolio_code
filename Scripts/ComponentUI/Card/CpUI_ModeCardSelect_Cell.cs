using UnityEngine;

namespace UIModeCard
{
    public class CpUI_ModeCardSelect_Cell : UIBase
    {
        [SerializeField] UIImage image = null;
        [SerializeField] UIText contentText = null;
        [SerializeField] UIText currValueText = null;
        [SerializeField] GameObject goSelect = null;

        public void Set(ResourceModeCard resCard)
        {
            var mode = ModeManager.Instance.mode;
            if (mode == null)
            {
                return;
            }

            var currLevel = mode.core.card.GetCardLevelByID(resCard.id);
            var nextLevel = currLevel + 1;
            if (image != null)
            {
                var spriteName = resCard.GetSprite(nextLevel);
                if (string.IsNullOrEmpty(spriteName))
                {
                    image.gameObject.SetActive(false);
                }
                else
                {
                    image.gameObject.SetActive(true);
                    image.SetSprite(Atlas.UI_ICON_AXE, spriteName);
                }
            }

            if (contentText != null)
            {
                contentText.SetText(resCard.GetName(nextLevel));
            }

            if (currValueText != null)
            {
                var s = string.Empty;
                if (resCard.isShowCount && resCard.isShowMaxCount)
                {
                    s = $"Lv.{currLevel}/{resCard.maxCount}";
                }
                else if (resCard.isShowCount)
                {
                    s = $"Lv.{currLevel}";
                }

                currValueText.SetText(s);
            }
        }

        public void Select(bool b)
        {
            if (goSelect != null)
            {
                goSelect.SetActive(b);
            }
        }
    }
}