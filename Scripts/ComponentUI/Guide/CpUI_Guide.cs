using System;
using UnityEngine;

namespace UIGuide
{
    public class CpUI_Guide : UIMonoBehaviour
    {
        private static CpUI_Guide m_instance = null;
        public static CpUI_Guide Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = UIManager.Instance.Find<CpUI_Guide>("pf_ui_guide");
                }
                return m_instance;
            }
        }

        [SerializeField] CpUI_GuidePages pagesCreateItem = null;
        [SerializeField] CpUI_GuidePages pagesMergeItem = null;
        [SerializeField] GameObject nextButton = null;

        private CpUI_GuidePages currPages = null;
        private int pageIndex = 0;
        private Action onComplete = null;

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.LAST, true);

            Cmd.Add(nextButton, eCmdTrigger.OnClick, Cmd_Next);
        }

        private void DoReset()
        {
            pagesCreateItem.gameObject.SetActive(false);
            pagesMergeItem.gameObject.SetActive(false);
        }

        public void On(GuideType guideType, Action<GuideType> exOnComplete)
        {
            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            DoReset();

            pageIndex = 0;
            currPages = GetPages(guideType);
            if (currPages == null)
            {
                return;
            }

            currPages.gameObject.SetActive(true);
            currPages.SetPage(pageIndex);
            onComplete = () => { exOnComplete?.Invoke(guideType); };
        }

        public void TapPage()
        {
            int currPage = pageIndex;
            int nextPage = pageIndex + 1;

            if (currPages.IsFinal(currPage))
            {
                onComplete?.Invoke();
                UIManager.Instance.CloseAt(this);
            }
            else
            {
                pageIndex = nextPage;
                currPages.SetPage(nextPage);
            }
        }

        public override bool CanClose()
        {
            if (currPages == null)
            {
                return true;
            }

            return false;
        }

        public CpUI_GuidePages GetPages(GuideType guideType)
        {
            switch (guideType)
            {
                case GuideType.CREATE_ITEM:
                    return pagesCreateItem;

                case GuideType.MERGE_ITEM:
                    return pagesMergeItem;
            }

            return null;
        }

        private void Cmd_Next()
        {
            TapPage();
        }

        public static bool IsPlay()
        {
            return Instance.IsActive();
        }
    }
}