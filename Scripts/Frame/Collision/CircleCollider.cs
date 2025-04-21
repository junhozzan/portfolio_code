using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCollider : MyCollider
{
    [SerializeField] Transform trRadius = null;

    public override void Init()
    {
        base.Init();
        collInfo.SetVolume(Vector2.Distance(trCenter.position, trRadius.position));
    }

    protected override void UpdateVolume()
    {
        collInfo.SetVolume(Vector2.Distance(trCenter.position, trRadius.position));
    }

    public override bool IsNearDistance(CollisionInfo targetCollInfo)
    {
        // 타켓과의 거리 <= 상대 부피 + 본인 부피 + 이동 델타
        return (targetCollInfo.currPos - collInfo.currPos).sqrMagnitude <= targetCollInfo.sqrVolume + collInfo.sqrVolume + collInfo.sqrMoveDelta;
    }

    public override bool IsTrigger(CollisionInfo targetCollInfo)
    {
        var targetPos = targetCollInfo.currPos;
        if ((targetPos - collInfo.currPos).sqrMagnitude <= Mathf.Pow(targetCollInfo.volume + collInfo.volume, 2))
        {
            return true;
        }

        var cross = Util.DotAndLineCross(targetPos, collInfo.moveDirection, collInfo.currPos);
        var edge = targetPos + ((cross - targetPos).normalized * targetCollInfo.volume);
        return Util.IsCollisionLineAndLine(targetPos, edge, collInfo.currPos, collInfo.prevPos);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(trCenter.position, Vector2.Distance(trCenter.position, trRadius.position));
    }
}