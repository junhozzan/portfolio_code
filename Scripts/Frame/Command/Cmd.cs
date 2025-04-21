using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using CCommand;

public class Cmd
{
    private static Dictionary<long, Cmd> a_dicCmds = new Dictionary<long, Cmd>(256);
    public static Cmd a_current { get; private set; } = null; // 현재 커맨드 객체

    // 클릭 타입
    private eCmdTrigger trigger = eCmdTrigger.OnClick;
    // 이미지
    private MaskableGraphic graphic = null;
    // 커맨드 키
    private int key = 0;
    // 버튼 오브젝트
    private GameObject goButton = null;                   
    // 비활성 이벤트 호출 버튼
    private GameObject goDisableButton = null;
    // 호출 이벤트
    private Action<int, GameObject> onEvent = null;
    // 활성 비활성 적용
    private Action<bool> onUse = null;
    // 활성 비활성 상태
    private Func<bool> isUse = null;

    // 비활성시 콜백
    private Action onDisable = null;
    // 포인터이벤트
    private Func<PointerEventData> pointerEvent = null;

    public bool _isUse
    {
        get
        {
            if (isUse == null)
            {
                return false;
            }

            return isUse();
        }
    }

    private void EventAssign()
    {
        // 이벤트 할당 완료
        if (graphic != null)
        {
            return;
        }

        if (goButton.TryGetComponent(out Text text))
        {
            graphic = text;
        }
        else if (goButton.TryGetComponent(out Image image))
        {
            graphic = image;
        }
        else
        {
            image = goButton.AddComponent<Image>();
            image.color = Color.clear;
            graphic = image;
        }

        if (!goButton.TryGetComponent(out Button button))
        {
            button = goButton.AddComponent<Button>();
            button.transition = Selectable.Transition.None;

            //var colors = button.colors;
            //colors.disabledColor = colors.normalColor;
            //colors.pressedColor = colors.normalColor;
            //button.colors = colors; // 컬러 변경값 설정
        }

        if (trigger == eCmdTrigger.OnClick)
        {
            button.onClick.AddListener(() => { On(); });
            onUse = (bUse) =>
            {
                if (button != null)
                {
                    button.interactable = bUse;
                }

                if (goDisableButton != null)
                {
                    goDisableButton.SetActive(!bUse);
                }
            };
            isUse = () => { return button.interactable; };
        }
        else
        {
            IPointerCmd pointerCmd = null;

            switch (trigger)
            {
                case eCmdTrigger.Press: pointerCmd = goButton.AddComponent<PointerDownCmd>(); break;
                case eCmdTrigger.Release: pointerCmd = goButton.AddComponent<PointerUpCmd>(); break;
            }

            if (pointerCmd != null)
            {
                pointerCmd._onEvent = On;
                onUse = (bUse) =>
                {
                    if (button != null)
                    {
                        button.interactable = bUse;
                    }

                    pointerCmd._bUse = bUse;
                };
                isUse = () => { return pointerCmd._bUse; };
                pointerEvent = () => { return pointerCmd._pointerEvent; };
            }
        }
    }

    private void On()
    {
        a_current = this;

        if (onEvent != null)
        {
            onEvent.Invoke(key, goButton);
        }
    }

    public void SetKey(int iKey)
    {
        key = iKey;
    }

    private int GetKey()
    {
        return key;
    }

    public void Use(bool bUse)
    {
        if (onUse == null)
        {
            return;
        }

        onUse(bUse);
    }

    public void SetRaycast(bool bUse)
    {
        if (graphic == null)
        {
            return;
        }

        graphic.raycastTarget = bUse;
    }

    public Cmd SetOnDisable(Action onDisable)
    {
        if (onDisable == null)
        {
            return this;
        }

        if (!goButton.TryGetComponent(out RectTransform rtButton))
        {
            return this;
        }

        if (goDisableButton == null)
        {
            goDisableButton = new GameObject();

            var rtDisable = goDisableButton.AddComponent<RectTransform>();

            rtDisable.SetParent(rtButton);
            rtDisable.localScale = Vector3.one;
            rtDisable.anchorMin = Vector2.zero;
            rtDisable.anchorMax = Vector2.one;
            rtDisable.offsetMin = Vector2.zero;
            rtDisable.offsetMax = Vector2.zero;

            var img = goDisableButton.AddComponent<Image>();
            img.color = Color.clear;
            img.raycastTarget = true;

            var disableButton = goDisableButton.AddComponent<Button>();
            disableButton.onClick.AddListener(() => { if (this.onDisable != null) { this.onDisable(); } });

            goDisableButton.SetActive(false);
        }

        this.onDisable = onDisable;
        return this;
    }

    public Cmd SetID(long id)
    {
        if (!a_dicCmds.ContainsKey(id))
        {
            a_dicCmds.Add(id, this);
        }
        else
        {
            if (_DEBUG)
            {
                Debug.LogFormat("## exist cmd id : {0}", id);
            }
        }

        return this;
    }

    public PointerEventData GetPointerEvent()
    {
        if (pointerEvent == null)
        {
            return null;
        }

        return pointerEvent.Invoke();
    }

    public static Cmd Add(GameObject goBtn, eCmdTrigger trigger, Action<int> action, int iKey = -1)
    {
        return Add(goBtn, trigger, (key, go) => action.Invoke(key), iKey);
    }

    public static Cmd Add(GameObject goBtn, eCmdTrigger trigger, Action<int, GameObject> action, int iKey = -1)
    {
        if (goBtn == null)
        {
            if (_DEBUG)
            {
                Debug.Log("## cmd button object is null");
            }

            return null;
        }

        var cmd = new Cmd
        {
            goButton = goBtn,
            trigger = trigger,
            key = iKey,
            onEvent = action
        };

        cmd.EventAssign();
        cmd.SetRaycast(true); // default
        cmd.Use(true); // default

        return cmd;
    }

    public static Cmd Add(GameObject goBtn, eCmdTrigger trigger, Action action)
    {
        if (goBtn == null)
        {
            if (_DEBUG)
            {
                Debug.Log(S.Red("## cmd button game object is null"));
            }

            return null;
        }

        var cmd = new Cmd
        {
            goButton = goBtn,
            trigger = trigger,
            onEvent = (i, go) => 
            {
                if (action != null)
                {
                    action();
                }
            }
        };

        cmd.EventAssign();
        cmd.SetRaycast(true); // default
        cmd.Use(true); // default

        return cmd;
    }

    public void CallEvent()
    {
        if (onEvent == null)
        {
            return;
        }

        onEvent(key, goButton);
    }

#if USE_DEBUG
    private const bool _DEBUG = true;
#else
    private const bool _DEBUG = false;
#endif
}


public interface IPointerCmd
{
    Action _onEvent { get; set; }
    bool _bUse { get; set; }
    PointerEventData _pointerEvent { get; set; }
}

public enum eCmdTrigger
{
    OnClick,
    Press,
    Release
}
