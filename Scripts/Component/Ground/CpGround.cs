using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CpGround : CpObject
{
    [SerializeField] GameObject map = null;
    [SerializeField] CpGroundLine originLine = null;

    private ObjectPool<CpGroundLine> linePool = null;

    private float loadingTimeAt = 0f;
    //private Sprite applySprite = null;
    private List<Sprite> applySprites = new List<Sprite>();

    const float OPEN_TEIM = 1f;
    const float DROP_TIME = 1f;
    const float MOVE_DISTANCE = 400f;

    protected override void Init()
    {
        base.Init();

        linePool = ObjectPool<CpGroundLine>.Of(originLine, map, onCreateInit: InitGroundLine);
    }

    private void InitGroundLine(CpGroundLine o)
    {
        o.Init();
    }

    public override void DoReset()
    {
        base.DoReset();

        trPosition.localPosition = Vector3.zero;
        linePool.Clear();
    }

    public void SetSprite( string spriteName)
    {
        applySprites.Clear();
        applySprites.Add(AtlasManager.Instance.GetSprite(Atlas.GROUND, spriteName));
    }

    public void SetSprites(ICollection<string> names)
    {
        applySprites.Clear();
        foreach (var name in names)
        {
            applySprites.Add(AtlasManager.Instance.GetSprite(Atlas.GROUND, name));
        }
    }

    public void Open()
    {
        TweenOpen();
    }

    private void TweenOpen()
    {
        var startPos = trPosition.localPosition + Vector3.down * Env.Distance(MOVE_DISTANCE);

        DOTween.To(
            null,
            t =>
            {
                trPosition.localPosition = Vector3.Lerp(startPos, Vector3.zero, EasingFunctions.InCubic(t));
            },
            1f,
            OPEN_TEIM)
            .From(0f)
            .SetEase(Ease.Linear)
            .SetId(transform)
            .OnComplete(EndOpen);


        loadingTimeAt = Main.Instance.time.realtimeSinceStartup + (OPEN_TEIM * 0.8f);
    }

    private void EndOpen()
    {
        DOTween.Kill(transform);
    }

    public void Drop()
    {
        TweenDrop();
    }

    private void TweenDrop()
    {
        DropBlocks();

        var startPos = trPosition.localPosition;
        var endPos = trPosition.localPosition + new Vector3(0f, Env.Distance(MOVE_DISTANCE), -2f);

        DOTween.To(
            null,
            t =>
            {
                trPosition.localPosition = Vector3.Lerp(startPos, endPos, EasingFunctions.InCubic(t));

                foreach (var obj in linePool.GetPool())
                {
                    if (!obj.gameObject.activeSelf)
                    {
                        continue;
                    }

                    obj.DoDrop(t);
                }
            },
            1f,
            DROP_TIME)
            .From(0f)
            .SetEase(Ease.Linear)
            .OnComplete(EndDrop);
    }

    private void EndDrop()
    {
        gameObject.SetActive(false);
        DOTween.Kill(transform);
    }

    private void DropBlocks()
    {
        foreach(var obj in linePool.GetPool())
        {
            if (!obj.gameObject.activeSelf)
            {
                continue;
            }

            obj.Drop();
        }
    }

    public void SetToZero(Vector3 offset)
    {
        foreach (var line in linePool.GetPool())
        {
            if (!line.gameObject.activeSelf)
            {
                continue;
            }

            line.ApplyOffsetPosition(offset);
        }
    }

    public bool IsLoading()
    {
        return loadingTimeAt > Main.Instance.time.realtimeSinceStartup;
    }

    public CpGroundLine PopLine(int index, bool isRight)
    {
        var pop = linePool.Pop();
        pop.DoReset();
        pop.SetSprite(applySprites[Random.Range(0, applySprites.Count - 1)]);
        pop.SetRight(isRight);
        pop.Activate();
        pop.SetLocalPosition(index);
        pop.SetSortingOrder(-index);
        return pop;
    }

    public Vector2 GetLinePosition(bool isRight, int index)
    {
        originLine.SetRight(isRight);
        return originLine.GetPositionByIndex(index);
    }
}
