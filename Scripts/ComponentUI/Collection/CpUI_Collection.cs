using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UICollection
{
    public class CpUI_Collection : UIMonoBehaviour
    {
        private static CpUI_Collection instance = null;
        public static CpUI_Collection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_Collection>("pf_ui_collection");
                }

                return instance;
            }
        }

        [SerializeField] CpUI_Collection_ContentTree contentTree = null;
        [SerializeField] CpUI_Collection_Stat stat = null;
        [SerializeField] GameObject exitButton = null;

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.CONTENTS, true);
            UsingBlind(false);
            UsingUpdate();

            contentTree.Init();
            stat.Init();

            Cmd.Add(exitButton, eCmdTrigger.OnClick, Cmd_Close);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.UPDATE_ITEM_ALL, Refresh)
                .Add(GameEventType.UPDATE_COLLECTION, Refresh)
                ;
        }

        public void On()
        {
            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            contentTree.On();
            stat.On();
        }

        protected override void RefreshInternal()
        {
            contentTree.Refresh();
            stat.Refresh();
        }
    }
}