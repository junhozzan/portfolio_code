using UnityEngine;

namespace UIBaseTop
{
    public class CpUI_BaseTop : UIMonoBehaviour
    {
        private static CpUI_BaseTop instance = null;
        public static CpUI_BaseTop Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_BaseTop>("pf_ui_base_top");
                }

                return instance;
            }
        }

        [SerializeField] UIWealth uiWealthOrigin = null;
        [SerializeField] GameObject optionButton = null;

        private ObjectPool<UIWealth> uiWealthPool = null;

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.JOIN_FIELD, Refresh)
                .Add(GameEventType.UPDATE_ITEM_AMOUNT, Refresh)
                .Add(GameEventType.UPDATE_LAB_VIRTUAL, Refresh)
                .Add(GameEventType.MODE_SUCCESS, Refresh)
                ;
        }

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.BASE, true);
            UsingUpdate();

            uiWealthPool = ObjectPool<UIWealth>.Of(uiWealthOrigin, uiWealthOrigin.transform.parent);

            Cmd.Add(optionButton, eCmdTrigger.OnClick, Cmd_ShowOptionPopup);
        }

        public void On()
        {
            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            RefreshInternal();
        }

        protected override void RefreshInternal()
        {
            RefreshWealth();
        }

        private void RefreshWealth()
        {
            uiWealthPool.Clear();
            uiWealthPool.Pop().SetWealthID(GameData.ITEM_DATA.GOLD).Refresh();
        }

        public override bool IsFixed()
        {
            return true;
        }

        private void Cmd_ShowOptionPopup()
        {
            PopupExtend.Instance.ShowOption();
        }
    }
}