using UnityEngine;

namespace UIUserInfo
{
    public class CpUI_UserInfoView : UIMonoBehaviour, IMenuItem
    {
        private static CpUI_UserInfoView m_instance = null;
        public static CpUI_UserInfoView Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = UIManager.Instance.Find<CpUI_UserInfoView>("pf_ui_userinfo_view");
                }

                return m_instance;
            }
        }

        [SerializeField] UIText textOrigin = null;

        private ObjectPool<UIText> textPool = null;

        public override void Init()
        {
            base.Init();
            UsingUpdate();
            SetCanvas(UIManager.eCanvans.POPUP1, true);

            textPool = ObjectPool<UIText>.Of(textOrigin, textOrigin.transform.parent);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.MY_UNIT_STAT_UPDATED, Handle_MY_UNIT_STAT_UPDATED)
                ;
        }

        private void Handle_MY_UNIT_STAT_UPDATED(object[] args)
        {
            Refresh();
        }

        private void On()
        {
            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            Refresh();
        }

        public void Refresh()
        {
            textPool.Clear();

            ShowStatNormal();
            ShowStatSpecial();
        }

        private void ShowStatNormal()
        {
            var infos = ShowStat.GetShowStats(MyUnit.Instance);
            if (infos.Count > 0)
            {
                foreach (var info in infos)
                {
                    SetText(info.Item1, info.Item2);
                }
            }
        }

        private void ShowStatSpecial()
        {
            var infos = ShowStat.GetShowSpecialAbilities(MyUnit.Instance);
            if (infos.Count > 0)
            {
                SetText(string.Empty, Color.white);
                foreach (var info in infos)
                {
                    SetText(info.Item1, info.Item2);
                }
            }
        }

        private void SetText(string str, Color color)
        {
            var text = textPool.Pop();
            text.gameObject.SetActive(true);
            text.transform.SetAsLastSibling();
            text.SetText(str);
            text.SetTextColor(color);
        }

        void IMenuItem.On(int value)
        {
            On();
        }

        bool IMenuItem.Notice()
        {
            return false;
        }
    }
}