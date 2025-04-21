using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UIPopup
{
    public class CpUI_PopupButton : MonoBehaviour
    {
        [SerializeField] GameObject button = null;
        [SerializeField] UIImage icon = null;
        [SerializeField] UIText text = null;
        [SerializeField] UIImage bg = null;
        [SerializeField] UIImage bgFrame = null;

        // 활성화 이벤트
        private readonly List<Action> onActives = new List<Action>();
        // 비활성화 이벤트
        private readonly List<Action> onDisables = new List<Action>();
        // 닫기 이벤트
        private Action onClose = null;
        // 누르기 방어
        private Func<bool> fnBan = null;

        private Cmd cmd = null;

        private Action onUpdate = null;

        public void Init(Action onClose)
        {
            this.onClose = onClose;

            cmd = Cmd.Add(button, eCmdTrigger.OnClick, Cmd_Active);
            cmd.SetOnDisable(Cmd_Disable);
            cmd.Use(true);

            DoReset();
        }

        public void DoReset()
        {
            onUpdate = null;

            onActives.Clear();
            onDisables.Clear();

            if (text != null)
            {
                text.transform.localPosition = Vector3.zero;
                text.SetText(string.Empty);
                text.SetTextColor(GameData.COLOR.POPUP_BUTTON_FONT);
            }

            if (icon != null)
            {
                icon.gameObject.SetActive(false);
            }

            gameObject.SetActive(false);
        }

        private void Update()
        {
            onUpdate?.Invoke();
        }

        public void SetOnUpdate(Action onUpdate)
        {
            this.onUpdate = onUpdate;

            onUpdate.Invoke();
        }

        /// <summary>
        /// 버튼 기본 셋팅
        /// </summary>
        public CpUI_PopupButton SetText(string strText, Color? color = null)
        {
            if (text != null)
            {
                text.SetText(strText);

                if (color != null)
                {
                    text.SetTextColor(color.Value);
                }
            }

            return this;
        }

        public CpUI_PopupButton SetTextKey(string key, Color? color = null)
        {
            if (text != null)
            {
                text.SetTextKey(key);

                if (color != null)
                {
                    text.SetTextColor(color.Value);
                }
            }

            return this;
        }

        public CpUI_PopupButton SetIcon(Atlas atlas, string name)
        {
            if (icon != null)
            {
                if (atlas == Atlas.NONE || string.IsNullOrEmpty(name))
                {
                    icon.gameObject.SetActive(false);
                }
                else
                {
                    icon.gameObject.SetActive(true);
                    icon.SetSprite(atlas, name);
                }
            }

            return this;
        }

        public CpUI_PopupButton SetBackground(Color bgColor, Color frameColor)
        {
            if (bg != null)
            {
                bg.SetColor(bgColor);
            }

            if (bgFrame != null)
            {
                bgFrame.SetColor(frameColor);
            }

            return this;
        }

        /// <summary>
        /// 버튼 이벤트 셋팅
        /// </summary>
        public CpUI_PopupButton SetOnActive(Action on)
        {
            onActives.Clear();

            if (on != null)
            {
                onActives.Add(on);
            }

            return this;
        }

        public CpUI_PopupButton SetOnDisable(Action on)
        {
            onDisables.Clear();
            onDisables.Add(on);
            return this;
        }

        public CpUI_PopupButton SetBlocking(Func<bool> ban)
        {
            fnBan = ban;
            return this;
        }

        public CpUI_PopupButton SetUse(bool use)
        {
            if (cmd != null)
            {
                cmd.Use(use);
            }

            return this;
        }

        private void Cmd_Active()
        {
            if (fnBan != null && fnBan.Invoke())
            {
                return;
            }

            UIBase.ClickSound();

            foreach (var on in onActives)
            {
                on.Invoke();
            }

            if (onClose != null)
            {
                onClose.Invoke();
            }
        }

        private void Cmd_Disable()
        {
            if (fnBan != null && fnBan.Invoke())
            {
                return;
            }

            foreach (var on in onDisables)
            {
                on.Invoke();
            }

            if (onClose != null)
            {
                onClose.Invoke();
            }
        }
    }
}