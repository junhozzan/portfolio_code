using System;
using UnityEngine;

namespace UIPopup
{
    public class CpUI_PopupFrame_PlatformSelect : CpUI_PopupFrame_Base
    {
        [SerializeField] GameObject googleButton = null;
        [SerializeField] GameObject guestButton = null;
        [SerializeField] GameObject typeLogin = null;
        [SerializeField] GameObject typePlaying = null;

        private bool canClose = true;
        private Action<PlatformType> onSelect = null;

        public override void Init(CpUI_Popup parent, Action<CpUI_PopupFrame_Base> onCloseAt)
        {
            base.Init(parent, onCloseAt);

            Cmd.Add(googleButton, eCmdTrigger.OnClick, Cmd_LoginGoogle);
            Cmd.Add(guestButton, eCmdTrigger.OnClick, Cmd_LoginGuest);
        }

        public override void DoReset()
        {
            base.DoReset();
            googleButton.SetActive(false);
            guestButton.SetActive(false);
            typeLogin.SetActive(false);
            typePlaying.SetActive(false);
        }

        public CpUI_PopupFrame_PlatformSelect OnByLogin()
        {
            //googleButton.SetActive(true);
            guestButton.SetActive(true);
            typeLogin.SetActive(true);
            canClose = false;

            return this;
        }

        public CpUI_PopupFrame_PlatformSelect OnByPlaying()
        {
            //googleButton.SetActive(true);
            typePlaying.SetActive(true);
            canClose = true;

            return this;
        }

        public CpUI_PopupFrame_PlatformSelect SetOnSelect(Action<PlatformType> onSelect)
        {
            this.onSelect = onSelect;
            return this;
        }

        public override bool CanClose()
        {
            return canClose;
        }

        private void Cmd_LoginGoogle()
        {
            onSelect?.Invoke(PlatformType.GOOGLE);
            canClose = true;
            CloseAt();
            ClickSound();
        }

        private void Cmd_LoginGuest()
        {
            onSelect?.Invoke(PlatformType.GUEST);
            canClose = true;
            CloseAt();
            ClickSound();
        }
    }
}
