using UnityEngine;
using UnityEngine.UI;

namespace UIPopup
{
    public class CpUI_PopupFrame_BasicText : CpUI_PopupFrame_Base
    {
        [SerializeField] UIText contentText = null;
        [SerializeField] ScrollRect scrollView = null;

        public void SetContent(string str)
        {
            if (contentText != null)
            {
                contentText.SetText(str);
            }

            SyncScrollFitter(scrollView, contentText, 10f);

            scrollView.enabled = scrollView.content.rect.size.y > scrollView.viewport.rect.size.y;
        }
    }
}