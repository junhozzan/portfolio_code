using UnityEngine;
using DG.Tweening;

public class UIBase : MonoBehaviour
{
    [SerializeField] protected RectTransform panel = null;
    [SerializeField] protected CanvasGroup canvasGroup = null;

    private EventDispatcher<GameEventType>.Handler handler = null;

    protected RectTransform _rectTransform;
    protected RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = transform as RectTransform;
            }

            return _rectTransform;
        }
    }

    protected virtual void Awake()
    {
        handler = CreateHandler();
    }

    protected virtual void OnDestroy()
    {
        GameEvent.Instance.RemoveHandler(handler);
    }

    protected virtual EventDispatcher<GameEventType>.Handler CreateHandler()
    {
        return null;
    }

    private Tween TwPanel(Vector2 start, Vector2 end)
    {
        return DOTween.To(
            null,
            t =>
            {
                if (panel != null)
                {
                    panel.anchoredPosition = Vector2.Lerp(start, end, t);
                }

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = t;
                }
            },
            1f,
            0.3f)
            .SetUpdate(true)
            .From(0f);
    }

    public Tween TwAppearUpPanel()
    {
        var start = new Vector2(0f, -3f);
        var end = Vector2.zero;

        return TwPanel(start, end);
    }

    public Tween TwAppearDownPanel()
    {
        var start = new Vector2(0f, 3f);
        var end = Vector2.zero;

        return TwPanel(start, end);
    }

    public Tween TwAppearSidePanel()
    {
        var start = new Vector2(30f, 0f);
        var end = Vector2.zero;

        return TwPanel(start, end);
    }

    public static void ClickSound()
    {
        SoundManager.Instance.PlaySfx(GameData.SOUND.SFX_CLICK);
    }

    public static void LoopClickSound()
    {

    }

    public static string GetCostText(long value)
    {
        return value.ToString("#,##0");
    }

    public const string FORMAT_COUNT = "{0}/{1}";

#if USE_DEBUG
    protected const bool _DEBUG = true;
#else
    protected const bool _DEBUG = false;
#endif
}
