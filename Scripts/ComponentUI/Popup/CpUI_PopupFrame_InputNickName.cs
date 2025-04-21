using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

namespace UIPopup
{
    public class CpUI_PopupFrame_InputNickName : CpUI_PopupFrame_Base
    {
        [SerializeField] InputField inputField = null;
        [SerializeField] UIText countText = null;
        [SerializeField] UIText contentText = null;
        [SerializeField] GameObject okButton = null;
        [SerializeField] GameObject cancelButton = null;

        private Cmd cmdOK = null;

        const int MAX_NICKNAME = 10;

        public override void Init(CpUI_Popup parent, Action<CpUI_PopupFrame_Base> onCloseAt)
        {
            base.Init(parent, onCloseAt);

            cmdOK = Cmd.Add(okButton, eCmdTrigger.OnClick, Cmd_OK);
            Cmd.Add(cancelButton, eCmdTrigger.OnClick, Cmd_Close);

            inputField.onValueChanged.AddListener(ValueChanged);
        }

        public override void On()
        {
            base.On();

            inputField.text = string.Empty;

            cmdOK.Use(false);
            countText.SetText(string.Format(FORMAT_COUNT, 0, MAX_NICKNAME));

            RefreshContent();
        }

        private void RefreshContent()
        {
            var content = $"{"key_ex_nickname".L()}\n\n<color=#{GameData.COLOR.WARNING.hex}>{"key_ex_nick_warning".L()}</color>";
            contentText.SetText(content);
        }

        private void ValueChanged(string s)
        {
            var len = s.Length;

            cmdOK.Use(len >= 2 && len <= MAX_NICKNAME);
            countText.SetText(string.Format(FORMAT_COUNT, len, MAX_NICKNAME));
        }

        private void Cmd_OK()
        {
            var str = inputField.text;

            if (
                //Regex.IsMatch(str, @"^[0-9a-zA-Z가-힣]*$", RegexOptions.Singleline) && 
                !SlangTable.IsSlang(str)
                && !str.Contains("운영")
                && !str.Contains("GM")
                && !str.Contains("gm"))
            {
                VirtualServer.Send(Packet.CREATE_NICKNAME, 
                    (arg) => 
                    {

                        if (!VirtualServer.TryGet(arg, out CREATE_NICKNAME tArg))
                        {
                            return;
                        }

                        GameEvent.Instance.AddEvent(GameEventType.CREATE_NICKNAME, tArg.tinfo);
                    }, 
                    str);
                
                CloseAt();
            }
            else
            {
                Main.Instance.ShowFloatingMessage("key_noinput".L());
            }

            ClickSound();
        }
    }
}