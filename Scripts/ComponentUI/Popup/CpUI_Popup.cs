using System.Collections.Generic;
using UnityEngine;
using System;

namespace UIPopup
{
    /*
     단일로 쓰는 UI들은 팝업에서 천천히 빼도록 하자.
     */
    public class CpUI_Popup : UIMonoBehaviour
    {
        private static CpUI_Popup instance = null;
        public static CpUI_Popup Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_Popup>("pf_ui_popup");
                }

                return instance;
            }
        }

        [SerializeField] CpUI_PopupFrame_BasicText basicTextFrame = null;
        [SerializeField] CpUI_PopupFrame_GetItems getItemsFrame = null;
        [SerializeField] CpUI_PopupFrame_InputNickName inputNickNameFrame = null;
        [SerializeField] CpUI_PopupFrame_ItemUtil itemUtilFrame = null;
        [SerializeField] CpUI_PopupFrame_DismantleItem dismantleItemFrame = null;
        [SerializeField] CpUI_PopupFrame_Dictionary dictionaryFrame = null;
        [SerializeField] CpUI_PopupFrame_Option optionFrame = null;
        [SerializeField] CpUI_PopupFrame_Language languageFrame = null;
        [SerializeField] CpUI_PopupFrame_ItemInfo itemInfoFrame = null;
        [SerializeField] CpUI_PopupFrame_PlatformSelect platformFrame = null;

        private readonly Dictionary<Type, ObjectPool<CpUI_PopupFrame_Base>> pools = new Dictionary<Type, ObjectPool<CpUI_PopupFrame_Base>>();
        private readonly List<CpUI_PopupFrame_Base> openedPopups = new List<CpUI_PopupFrame_Base>();

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.POPUP2, true);
            UsingBlind(isTouchClose: false, false);


            // 원본 프레임 숨기기
            var originFrames = transform.GetComponentsInChildren<CpUI_PopupFrame_Base>();
            foreach (var frams in originFrames)
            {
                frams.gameObject.SetActive(false);
            }
        }

        public T Show<T>() where T : CpUI_PopupFrame_Base
        {
            var type = typeof(T);
            if (!pools.TryGetValue(type, out var pool))
            {
                CpUI_PopupFrame_Base originFrame = null;
                if (type == basicTextFrame.GetType()) { originFrame = basicTextFrame; }
                else if (type == getItemsFrame.GetType()) { originFrame = getItemsFrame; }
                else if (type == inputNickNameFrame.GetType()) { originFrame = inputNickNameFrame; }
                else if (type == itemUtilFrame.GetType()) { originFrame = itemUtilFrame; }
                else if (type == dismantleItemFrame.GetType()) { originFrame = dismantleItemFrame; }
                else if (type == dictionaryFrame.GetType()) { originFrame = dictionaryFrame; }
                else if (type == optionFrame.GetType()) { originFrame = optionFrame; }
                else if (type == languageFrame.GetType()) { originFrame = languageFrame; }
                else if (type == itemInfoFrame.GetType()) { originFrame = itemInfoFrame; }
                else if (type == platformFrame.GetType()) { originFrame = platformFrame; }
                
                if (originFrame == null)
                {
                    if (_DEBUG)
                    {
                        Debug.Log($"## popup type prefab is null {type}");
                    }

                    return null;
                }

                pools.Add(type, pool = ObjectPool<CpUI_PopupFrame_Base>.Of(originFrame, gameObject, onCreateInit: OnCreateInit));
            }

            UIManager.Instance.Show(this);

            var frame = pool.Pop();
            frame.DoReset();
            frame.On();

            openedPopups.Add(frame);
            return frame as T;
        }

        private void OnCreateInit(CpUI_PopupFrame_Base frame)
        {
            frame.Init(this, CloseAt);
        }

        private bool CloseFrame(CpUI_PopupFrame_Base frame)
        {
            if (openedPopups.Count > 0)
            {
                if (frame == null)
                {
                    // 특정 팝업이 없는 경우 가장 최근에 열린 팝업을 선택
                    frame = openedPopups[openedPopups.Count - 1];
                }

                if (!frame.CanClose())
                {
                    return false;
                }

                frame.gameObject.SetActive(false);
                openedPopups.Remove(frame);
            }

            // 열린 팝업이 없다면 종료.
            return openedPopups.Count == 0;
        }

        private void CloseAt(CpUI_PopupFrame_Base frame)
        {
            if (!CloseFrame(frame))
            {
                return;
            }

            Off();
        }

        public override bool CanClose()
        {
            return CloseFrame(null);
        }
    }
}