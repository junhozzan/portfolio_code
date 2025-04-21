using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace UIPopup
{
    public class CpUI_PopupFrame_Base : UIBase
    {
        [SerializeField] Text titleText = null;
        [SerializeField] CpUI_PopupButton button = null;
        [SerializeField] RectTransform safeArea = null;
        [SerializeField] UIWealth uiWealthOrigin = null;

        private ObjectPool<UIWealth> uiWealthPool = null;

        private List<CpUI_PopupButton> buttons = new List<CpUI_PopupButton>();
        private Action<CpUI_PopupFrame_Base> onCloseAt = null;

        protected CpUI_Popup parent = null;
        protected Vector2 originPanelSize = Vector2.zero;

        public virtual void Init(CpUI_Popup parent, Action<CpUI_PopupFrame_Base> onCloseAt)
        {
            this.parent = parent;
            this.onCloseAt = onCloseAt;

            if (button != null)
            {
                // 버튼 원본 프리펩 비활성화
                button.gameObject.SetActive(false);
            }

            if (panel != null)
            {
                originPanelSize = panel.rect.size;
            }

            SetSafeArea();
            CreateBlind();
            InitUIWealth();
        }

        private void SetSafeArea()
        {
            if (safeArea == null)
            {
                return;
            }

            var anchor = new Vector2(0.5f, 0.5f);
            safeArea.anchorMin = anchor;
            safeArea.anchorMax = anchor;
            safeArea.sizeDelta = DefineUI.safeAreaSize;
            safeArea.localPosition = DefineUI.safeAreaCenter;
        }

        private void CreateBlind()
        {
            // 블라인드 생성
            var blind = UIManager.CreateBlind(transform, true);
            blind.transform.SetAsFirstSibling();
            blind.transform.localPosition = Vector2.zero;
        }

        private void InitUIWealth()
        {
            if (uiWealthOrigin == null)
            {
                return;
            }

            uiWealthPool = ObjectPool<UIWealth>.Of(uiWealthOrigin, uiWealthOrigin.transform.parent);
        }

        public virtual void DoReset()
        {
            for (int i = 0, cnt = buttons.Count; i < cnt; ++i)
            {
                buttons[i].DoReset();
            }

            if (panel != null)
            {
                panel.sizeDelta = originPanelSize;
            }

            if (uiWealthPool != null)
            {
                uiWealthPool.Clear();
            }
        }

        public virtual void On()
        {
            transform.SetAsLastSibling();
            transform.localPosition = Vector2.zero;
        }

        public void SetTitle(string strTitle)
        {
            if (titleText == null)
            {
                return;
            }

            titleText.text = strTitle;
        }

        public void SetPanelSize(float xSize = -1f, float ySize = -1f)
        {
            if (panel == null)
            {
                return;
            }

            panel.sizeDelta = new Vector2()
            {
                x = xSize == -1f ? originPanelSize.x : xSize,
                y = ySize == -1f ? originPanelSize.y : ySize
            };
        }

        public void SetWealthParams(params int[] wealthIDs)
        {
            SetWealth(wealthIDs);
        }

        public void SetWealth(IEnumerable<int> wealthIDs)
        {
            if (uiWealthPool == null)
            {
                return;
            }

            uiWealthPool.Clear();
            foreach (var id in wealthIDs)
            {
                uiWealthPool.Pop().SetWealthID(id).Refresh();
            }
        }

        public CpUI_PopupButton SetButton()
        {
            var btn = buttons.Find(x => !x.gameObject.activeSelf);
            if (btn == null)
            {
                if (button == null)
                {
                    if (_DEBUG)
                    {
                        Debug.LogFormat("## popup frame button is null {0}", transform.name);
                    }
                    return null;
                }

                // 버튼 생성
                btn = Instantiate(button, button.transform.parent);
                // 기본 닫기 이벤트 등록
                btn.Init(CloseAt);
                buttons.Add(btn);
            }

            // 사용 버튼 활성화
            btn.gameObject.SetActive(true);
            btn.SetBackground(GameData.COLOR.DEFAULT_BUTTON, GameData.COLOR.DEFAULT_BUTTON_FRAME);

            return btn;
        }

        protected virtual void CloseAt()
        {
            onCloseAt?.Invoke(this);
        }

        protected void Cmd_Close()
        {
            ClickSound();
            CloseAt();
        }

        protected bool IsActive()
        {
            return gameObject.activeSelf;
        }

        protected static void SyncScrollFitter(ScrollRect scrollView, UIText text, float offset)
        {
            if (scrollView == null || text == null)
            {
                return;
            }

            //text.SetFitterVertical(offset);

            scrollView.enabled = false;
            if (scrollView.content == null)
            {
                return;
            }

            var scrollRectSize = (scrollView.transform as RectTransform).rect.size;
            var textRectSize = text.GetRectSize();
            var content = scrollView.content;
            var reSize = content.sizeDelta;

            if (textRectSize.y > scrollRectSize.y)
            {
                scrollView.enabled = true;
                reSize.y = textRectSize.y;
            }
            else
            {
                reSize.y = scrollRectSize.y;
            }

            content.anchoredPosition = Vector3.zero;
            content.sizeDelta = reSize;
        }

        public virtual bool CanClose()
        {
            return true;
        }
    }
}