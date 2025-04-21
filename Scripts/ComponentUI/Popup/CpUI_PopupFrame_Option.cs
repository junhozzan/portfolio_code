using UnityEngine;
using UnityEngine.UI;
using System;

namespace UIPopup
{
    public class CpUI_PopupFrame_Option : CpUI_PopupFrame_Base
    {
        [SerializeField] Slider bgmSlider = null;
        [SerializeField] Slider sfxSlider = null;
        [SerializeField] UIText bgmValueText = null;
        [SerializeField] UIText sfxValueText = null;

        [SerializeField] UIText languageText = null;
        [SerializeField] GameObject languageButton = null;

        [SerializeField] GameObject connectButton = null;
        [SerializeField] GameObject conntectGoogleMark = null;

        [SerializeField] UIRadio fpsRadio = null;
        [SerializeField] GameObject fps60Button = null;
        [SerializeField] GameObject fps30Button = null;

        [SerializeField] GameObject exitButton = null;

        public override void Init(CpUI_Popup parent, Action<CpUI_PopupFrame_Base> onCloseAt)
        {
            base.Init(parent, onCloseAt);

            fpsRadio.Init(OnRadioTab);

            bgmSlider.onValueChanged.AddListener(SliderUpdateBGM);
            sfxSlider.onValueChanged.AddListener(SliderUpdateSFX);

            Cmd.Add(languageButton, eCmdTrigger.OnClick, Cmd_OpenLanguageSelect);
            Cmd.Add(exitButton, eCmdTrigger.OnClick, Cmd_OpenExitPopup);
            Cmd.Add(connectButton, eCmdTrigger.OnClick, Cmd_OpenPlatformSelect);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.CONNECT_PLATFORM, Handle_CONNECT_PLATFORM)
                ;
        }

        private void Handle_CONNECT_PLATFORM(object[] args)
        {
            RefreshConnect();
        }

        public override void On()
        {
            base.On();

            RefreshFpsRadioTab();
            RefreshLanguageText();
            RefreshSoundSlider();
            RefreshConnect();
        }

        private void RefreshSoundSlider()
        {
            bgmSlider.value = SoundManager.Instance._volBgm;
            SliderUpdateBGM(SoundManager.Instance._volBgm);

            sfxSlider.value = SoundManager.Instance._volSfx;
            SliderUpdateSFX(SoundManager.Instance._volSfx);
        }

        private void RefreshConnect()
        {
            connectButton.SetActive(false);
            conntectGoogleMark.SetActive(false);

            if (PlatformManager.Instance.IsConnected(out var platform))
            {
                switch (platform.type)
                {
                    case PlatformType.GOOGLE: conntectGoogleMark.SetActive(true); break;
                }
            }
            else
            {
                connectButton.SetActive(true);
            }
        }

        private void RefreshLanguageText()
        {
            languageText.SetText($"key_{Localize.language.ToString().ToLower()}".L());
        }

        private void RefreshFpsRadioTab()
        {
            GameObject tab;
            if (Option.fps >= 60)
            {
                tab = fps60Button;
            }
            else
            {
                tab = fps30Button;
            }

            fpsRadio.Choice(tab);
        }

        private void SliderUpdateBGM(float t)
        {
            SoundManager.Instance._volBgm = t;
            if (bgmValueText != null)
            {
                bgmValueText.SetText((t * 100).ToString("0"));
            }
        }

        private void SliderUpdateSFX(float t)
        {
            SoundManager.Instance._volSfx = t;
            if (sfxValueText != null)
            {
                sfxValueText.SetText((t * 100).ToString("0"));
            }
        }

        private void OnRadioTab(GameObject go)
        {
            if (go == fps60Button)
            {
                Option.fps = 60;
            }
            else if (go == fps30Button)
            {
                Option.fps = 30;
            }
        }

        private void Cmd_OpenLanguageSelect()
        {
            ClickSound();
            PopupExtend.Instance.ShowLanguage(RefreshLanguageText);
        }

        private void Cmd_OpenExitPopup()
        {
            ClickSound();
            PopupExtend.Instance.ShowQuit(() => Option.Save());
        }

        private void Cmd_OpenPlatformSelect()
        {
            ClickSound();
            PopupExtend.Instance.ShowPlatformSelectByPlaying(PlatformManager.Instance.ConnectPlatform);
        }

        protected override void CloseAt()
        {
            Option.Save();
            base.CloseAt();
        }
    }
}