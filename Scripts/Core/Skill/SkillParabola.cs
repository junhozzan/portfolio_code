using System.Collections;
using UnityEngine;

/// <summary>
/// not used
/// </summary>
public class SkillParabola : SkillBase
{
    private ResourceSkillParabola res = null;
    private CpProjectile obj = null;

    private readonly Parabola parabola = new Parabola();

    public static SkillParabola Of()
    {
        return new SkillParabola();
    }

    public override void DoReset()
    {
        base.DoReset();

        res = null;
        obj = null;
    }

    public override bool Play(SkillInfo skillInfo)
    {
        base.Play(skillInfo);
        res = skillInfo.resFire.script as ResourceSkillParabola;
        SetParabola();
        CreateObject();

        return true;
    }

    private void CreateObject()
    {
        if (string.IsNullOrEmpty(res.prefab))
        {
            return;
        }

        obj = ObjectManager.Instance.Pop<CpProjectile>(res.prefab);
        obj.SetLayer(res.layer);
        //obj.SetPosition(skillInfo.shotPosition);
    }

    private void SetParabola()
    {
        //var shotPos = skillInfo.shotPosition;
        //var targetPos = shotPos; // default;

        //var focus = res.focus;
        //if (focus != null)
        //{
        //    switch (focus.type)
        //    {
        //        case ResourceSkillParabola.Type.TARGET_POS:
        //            targetPos = skillInfo._target.core.transform.GetPosition();
        //            break;

        //        case ResourceSkillParabola.Type.RANDOM_RANGE:
        //            targetPos = shotPos + (Vector3)Random.insideUnitCircle * Random.Range(focus.min, focus.max);
        //            break;
        //    }
        //}

        //parabola.Set(shotPos, targetPos, res.parabolaDuration, res.parabolaHeigth);
    }

    public override void UpdateDt(float dt)
    {
        base.UpdateDt(dt);

        parabola.UpdateDt(dt);

        // 오브젝트의 이동 및 회전
        if (obj != null)
        {
            obj.SetPosition(parabola.movedPos);
        }

        // 포물선의 이동 종료
        if (parabola.IsArrive())
        {
            //PlayTail(null, parabola.movedPos);
            OnDisable();
        }
    }

    public override void OnDisable()
    {
        if (obj != null)
        {
            obj.DelayInactive(res.shadowTime);
            obj = null;
        }

        base.OnDisable();
    }

    private class Parabola
    {
        private Vector3 startPos = Vector3.zero;
        private Vector3 endPos = Vector3.zero;
        private float duration = 0f;
        private float height = 0f;
        private float elapseTime = 0f;

        public Vector3 movedPos { get; private set; } = Vector3.zero;
        public Vector3 movedDir { get; private set; } = Vector3.zero;

        public void Set(Vector3 startPos, Vector3 endPos, float duration, float height)
        {
            var vec = endPos - startPos;
            var distance = vec.magnitude; 

            this.startPos = startPos;
            this.endPos = endPos;
            this.duration = duration * (1f + distance / 1000f);
            this.height = height;

            this.movedPos = Vector3.zero;
            this.movedDir = vec.normalized;
            this.elapseTime = 0f;
        }

        public void UpdateDt(float dt)
        {
            elapseTime += dt;
            movedPos = ParabolaMove(startPos, endPos, elapseTime, duration, height);
        }

        /// <summary>
        /// 포물선 충돌 체크 (체공 시간 만료)
        /// </summary>
        public bool IsArrive()
        {
            return elapseTime >= duration; // 포물선 시간 만료로 충돌
        }


        private static Vector3 ParabolaMove(Vector3 startPos, Vector3 endPos, float elapseTime, float duration, float height)
        {
            var time = elapseTime / duration;
            var posX = Mathf.LerpUnclamped(startPos.x, endPos.x, time);
            var posZ = Mathf.LerpUnclamped(startPos.z, endPos.z, time);
            var posY = ParabolaCurve(height, time) + Mathf.LerpUnclamped(startPos.y, endPos.y, time);
            return new Vector3(posX, posY, posZ);
        }

        private static float ParabolaCurve(float height, float t)
        {
            return (1f - t) * 4 * height * t;
        }
    }
}
