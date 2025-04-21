using UnityEngine;

namespace ModeComponent
{
    public class StageModeFieldComponent : ModeFieldGroundComponent
    {
        public StageModeFieldComponent(StageMode mode) : base(mode)
        {

        }

        public override Vector3 GetRandomGroundPosition()
        {
            if (mainLine == null || lastLine == null)
            {
                return Vector3.zero;
            }

            var pos = lastLine.GetBoundaryCenter();
            var guideVector = GetGuideVector();
            var offset = guideVector * 4f;

            return pos + offset;
        }

        //public override Vector3 GetRandomGroundPosition()
        //{
        //    if (mainLine == null || lastLine == null)
        //    {
        //        return Vector3.zero;
        //    }

        //    var fieldRectangle = GetFieldRectangle();
        //    var guideVector = GetGuideVector();
        //    var dot = Vector2.Lerp(mainLine.GetBoundaryCenter(), lastLine.GetBoundaryCenter(), 0.5f);
        //    var ranVec = Util.AddAngle(guideVector, Random.Range(-90f, 90f));
        //    var end = dot + ranVec * 1000f;
        //    var offset = guideVector * 4f;

        //    for (int i = 0, len = fieldRectangle.Length; i < len; ++i)
        //    {
        //        var a = fieldRectangle[i];
        //        var b = fieldRectangle[(i + 1) % len];
        //        if (!Util.TryGetCrossLine(a, b, dot, end, out var result))
        //        {
        //            continue;
        //        }

        //        // 벽에 부딪히면 부딪힌 점 반환
        //        return Vector3.Lerp(dot, result, Random.Range(0.1f, 0.9f)) + offset;
        //    }

        //    return lastLine.GetBoundaryCenter();
        //}
    }
}