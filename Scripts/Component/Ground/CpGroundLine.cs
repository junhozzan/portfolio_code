using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpGroundLine : MonoBehaviour
{
    [SerializeField] Animator animator = null;
    [SerializeField] CpGroundBlock[] blocks = null;
    [SerializeField] Vector3 spacing = Vector3.zero;

    public CpGroundLine linkedLine { get; private set; } = null;

    private readonly Vector2[] boundary = new Vector2[2];
    private static readonly Vector3 boundaryOffset = new Vector3(0, Env.Distance(50f));

    private static int lineUpHash = Animator.StringToHash("line_up");

    public void Init()
    {
        foreach (var b in blocks)
        {
            b.Init();
        }
    }

    public void DoReset()
    {
        foreach (var b in blocks)
        {
            b.DoReset();
        }

        animator.enabled = false;
        linkedLine = null;
    }

    public void SortBlocks()
    {
        if (blocks == null)
        {
            return;
        }

        var centerOffset = -spacing * blocks.Length / 2;
        for (int i = 0; i < blocks.Length; ++i)
        {
            blocks[i].transform.localPosition = centerOffset + spacing * i;
        }
    }

    public void Activate()
    {
        foreach (var b in blocks)
        {
            b.Activate();
        }
    }

    public void PlayLineUpAnimation()
    {
        animator.enabled = true;
        animator.Play(lineUpHash, 0);
    }

    public void Drop()
    {
        foreach (var b in blocks)
        {
            b.Drop();
        }
    }

    public void DoDrop(float t)
    {
        foreach (var b in blocks)
        {
            b.DoDrop(t);
        }
    }

    public void SetLinkNextLine(CpGroundLine nextLine)
    {
        this.linkedLine = nextLine;
    }

    public void SetLocalPosition(int index)
    {
        transform.localPosition = GetPositionByIndex(index);
    }

    public Vector2 GetPositionByIndex(int index)
    {
        return new Vector2(-spacing.x, spacing.y) * index;
    }

    public void SetSortingOrder(int start)
    {
        for (int i = 0; i < blocks.Length; ++i)
        {
            blocks[i].SetSortingOrder(start + (-i));
        }
    }

    public void ApplyOffsetPosition(Vector3 offset)
    {
        transform.position -= offset;
    }

    public Vector2[] GetBoundary()
    {
        boundary[0] = blocks[0].transform.position + boundaryOffset;
        boundary[1] = blocks[blocks.Length - 1].transform.position + boundaryOffset;
        return boundary;
    }

    public Vector3 GetBoundaryCenter()
    {
        var b = GetBoundary();
        return Vector3.Lerp(b[0], b[1], 0.5f);
    }

    public Vector2 GetGuideVector()
    {
        return new Vector2(-spacing.x, spacing.y).normalized;
    }

    public void SetSprite(Sprite sprite)
    {
        if (blocks == null)
        {
            return;
        }

        foreach (var block in blocks)
        {
            block.SetSprite(sprite);
        }
    }

    public void SetRight(bool isTrue)
    {
        if (isTrue)
        {
            if (spacing.x > 0)
            {
                spacing.x *= -1;
            }
        }
        else
        {
            if (spacing.x < 0)
            {
                spacing.x *= -1;
            }
        }

        SortBlocks();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            return;
        }

        if (transform.childCount == 0)
        {
            return;
        }

        blocks = transform.GetComponentsInChildren<CpGroundBlock>();

        SortBlocks();
        for (int i = 0; i < blocks.Length; ++i)
        {
            blocks[i].SetSortingOrder(-i);
        }
    }
#endif
}
