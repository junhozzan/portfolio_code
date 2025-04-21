using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace UIModeCard
{
    public class CpUI_ModeCardSelect : UIMonoBehaviour
    {
        private static CpUI_ModeCardSelect instance = null;
        public static CpUI_ModeCardSelect Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_ModeCardSelect>("pf_ui_mode_card_select");
                }

                return instance;
            }
        }

        [SerializeField] CpUI_ModeCardSelectProfiler profiler = null;
        [SerializeField] Toggle autoSelectToggle = null;
        [SerializeField] UIText autoSelectText = null;
        [SerializeField] GameObject cardCellParent = null;
        [SerializeField] UIText cardCountText = null;

        private Mode mode = null;
        private CpUI_ModeCardSelect_Cell[] cardCells = null;
        private List<ResourceModeCard> selectableModeCards = new List<ResourceModeCard>();

        private float timer = 0f;
        private int autoSelectIdx = -1;

        const float AUTO_SELECT_TIME = 3f;

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.UPDATE_MODE, Handle_UPDATE_MODE)
                .Add(GameEventType.UPDATE_CARD, Handle_UPDATE_CARD)
                ;
        }

        private void Handle_UPDATE_MODE(object[] args)
        {
            Refresh();
        }

        private void Handle_UPDATE_CARD(object[] args)
        {
            Refresh();
        }

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.POPUP2, true);
            UsingUpdate();

            InitCells();
            profiler.Init();
            autoSelectToggle.onValueChanged.AddListener(OnAutoSelectValueChange);
        }

        private void InitCells()
        {
            cardCells = cardCellParent.GetComponentsInChildren<CpUI_ModeCardSelect_Cell>(true);
            for (int i = 0, cnt = cardCells.Length; i < cnt; ++i)
            {
                Cmd.Add(cardCells[i].gameObject, eCmdTrigger.OnClick, Cmd_SelectItem, i);
            }
        }

        public void On(Mode mode, List<ResourceModeCard> selectableModeCards)
        {
            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            this.mode = mode;
            this.selectableModeCards.Clear();
            this.selectableModeCards.AddRange(selectableModeCards);
            this.autoSelectToggle.isOn = Option.isAutoSelect;
            this.timer = 0f;

            for (int i = 0, cnt = cardCells.Length; i < cnt; ++i)
            {
                var slot = cardCells[i];
                slot.gameObject.SetActive(i < selectableModeCards.Count);
                if (!slot.gameObject.activeSelf)
                {
                    continue;
                }

                slot.Set(selectableModeCards[i]);
            }

            Refresh();
        }

        protected override void RefreshInternal()
        {
            profiler.Refresh();
            RefresCountCountText();
            RefreshAutoSelect(autoSelectToggle.isOn);
        }

        private void RefresCountCountText()
        {
            cardCountText.SetText($"{"key_card_count".L()}: {ModeManager.Instance.mode.core.card.ReceivableCardCount()}");
        }

        public override void UpdateDt(float unDt, DateTime now)
        {
            base.UpdateDt(unDt, now);
            UpdateAutoSelect(unDt);
        }

        private void UpdateAutoSelect(float unDt)
        {
            if (!autoSelectToggle.isOn)
            {
                return;
            }

            timer += unDt;

            if (autoSelectText != null)
            {
                autoSelectText.SetText($"{"key_itemautoselect".L()} ({(AUTO_SELECT_TIME - timer).ToString("0")})");
            }

            if (timer > AUTO_SELECT_TIME)
            {
                OnSelectCard(autoSelectIdx);
                Off();
            }
        }

        private void OnSelectCard(int idx)
        {
            if (mode == null)
            {
                return;
            }

            if (idx < 0 && idx >= selectableModeCards.Count)
            {
                return;
            }

            VirtualServer.Send(Packet.SELECT_MODE_CARD,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out SELECT_MODE_CARD tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_CARD, tArg.tcards);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                },
                selectableModeCards[idx].id);
        }

        private void RefreshAutoSelect(bool b)
        {
            if (!b)
            {
                timer = 0f;
                autoSelectIdx = -1;

                if (autoSelectText != null)
                {
                    autoSelectText.SetText($"{"key_itemautoselect".L()}");
                }
            }
            else
            {
                autoSelectIdx = UnityEngine.Random.Range(0, selectableModeCards.Count);
            }

            if (cardCells != null)
            {
                for (int i = 0, cnt = cardCells.Length; i < cnt; ++i)
                {
                    cardCells[i].Select(autoSelectIdx == i);
                }
            }

            Option.isAutoSelect = autoSelectIdx != -1;
            Option.Save();
        }

        private void OnAutoSelectValueChange(bool b)
        {
            RefreshAutoSelect(b);
        }

        private void Cmd_SelectItem(int idx)
        {
            ClickSound();
            OnSelectCard(idx);
            Off();
        }

        public override bool CanClose()
        {
            return false;
        }
    }
}