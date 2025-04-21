using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UIPopup
{
    public class CpUI_PopupFrame_Language : CpUI_PopupFrame_Base
    {
        [SerializeField] GameObject koButton = null;
        [SerializeField] GameObject enButton = null;
        [SerializeField] GameObject jpButton = null;
        [SerializeField] GameObject hiButton = null;

        private Action onCloseEvent = null;

        public override void Init(CpUI_Popup parent, Action<CpUI_PopupFrame_Base> onCloseAt)
        {
            base.Init(parent, onCloseAt);

            Cmd.Add(koButton, eCmdTrigger.OnClick, Cmd_Translate, (int)eLanguage.KO);
            Cmd.Add(enButton, eCmdTrigger.OnClick, Cmd_Translate, (int)eLanguage.EN);
            Cmd.Add(jpButton, eCmdTrigger.OnClick, Cmd_Translate, (int)eLanguage.JP);
            Cmd.Add(hiButton, eCmdTrigger.OnClick, Cmd_Translate, (int)eLanguage.HI);
        }

        public CpUI_PopupFrame_Language On(Action onCloseEvent)
        {
            this.onCloseEvent = onCloseEvent;
            return this;
        }

        private void Cmd_Translate(int e)
        {
            Localize.Refresh((eLanguage)e);

            CloseAt();
            ClickSound();
        }

        protected override void CloseAt()
        {
            base.CloseAt();
            onCloseEvent?.Invoke();
        }
    }
}