using UnityEngine;

namespace ModeComponent
{
    public class ModeFieldGroundComponent : ModeFieldComponent
    {
        public ModeFieldGroundComponent(Mode mode) : base(mode)
        {

        }

        protected CpField field = null;
        protected CpGround ground = null;
        protected CpGroundLine mainLine = null;
        protected CpGroundLine lastLine = null;

        protected int mainCheckIndex = 0;
        protected int currLineIndex = 0;
        protected int maxLineCount = 0;

        protected bool isRight = false;
        protected Vector2[] fieldRectangle = new Vector2[4];

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.MODE_START, Handle_MODE_START)
                ;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (field == null)
            {
                field = ObjectManager.Instance.Pop<CpField>(mode.core.profile.resMode.fieldPrefab);
            }
        }

        public override void OnDisable()
        {
            SetToZero();
            
            if (ground != null)
            {
                ground.Drop();
                ground = null;
            }

            if (field != null)
            {
                field.gameObject.SetActive(false);
                field = null;
            }

            base.OnDisable();
        }

        private void Handle_MODE_START(object[] args)
        {
            if (field == null)
            {
                return;
            }

            field.SetParallaxTarget(mode.core.camera.camera.transform);
        }

        public void SetToZero()
        {
            var myUnit = mode.core.ally.myUnit;
            if (myUnit == null)
            {
                return;
            }

            var pos = myUnit.core.transform.GetPosition();
            ground?.SetToZero(pos);
        }

        public void DropAndNewGround()
        {
            var prevGround = ground;
            ground = null;

            if (prevGround != null)
            {
                prevGround.Drop();
            }

            var groundInfo = GetRandomGroundInfo();
            if (groundInfo == null)
            {
                return;
            }

            var newGound = ObjectManager.Instance.Pop<CpGround>(groundInfo.perfab);
            if (newGound != null)
            {
                newGound.SetLayer(GameData.DEFAULT.LAYER_BACKGROUND);
                newGound.SetSprites(groundInfo.GetSprites());
                newGound.Open();
            }

            mainLine = null;
            lastLine = null;

            ground = newGound;
            currLineIndex = -1;
            mainCheckIndex = groundInfo.mainCheckIndex;
            maxLineCount = groundInfo.maxLineCount;
            //isRight = !isRight;

            InitPopLine(groundInfo.initMinIndex, groundInfo.initMaxIndex);
        }

        private void InitPopLine(int min, int max)
        {
            if (ground == null)
            {
                return;
            }

            for (int i = min; i <= max; ++i)
            {
                var line = ground.PopLine(i, isRight);
                RefreshLineInfo(i, line);
            }
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            UpdatePopLine();
            UpdateMainLine();
        }

        protected virtual void UpdateMainLine()
        {
            if (IsLoading())
            {
                return;
            }

            var pos = mainLine.GetBoundaryCenter();
            if (mode.core.camera.IsInView(pos, new Vector2(5f, 5f)))
            {
                return;
            }

            mainLine.gameObject.SetActive(false);
            mainLine = mainLine.linkedLine;
        }

        protected virtual void UpdatePopLine()
        {
            if (ground == null)
            {
                return;
            }

            for (int i = currLineIndex + 1; i < maxLineCount; ++i)
            {
                var pos = ground.GetLinePosition(isRight, i);
                if (!mode.core.camera.IsInView(pos, new Vector2(3f, 3f)))
                {
                    break;
                }

                var line = ground.PopLine(i, isRight);
                line.PlayLineUpAnimation();
                RefreshLineInfo(i, line);
            }
        }

        protected void RefreshLineInfo(int index, CpGroundLine line)
        {
            if (index < mainCheckIndex)
            {
                return;
            }

            if (mainLine == null)
            {
                mainLine = line;
            }

            var prevLine = lastLine;
            if (prevLine != null)
            {
                prevLine.SetLinkNextLine(line);
            }

            lastLine = line;
            currLineIndex = index;
        }

        public override bool IsLoading()
        {
            if (mainLine == null || lastLine == null)
            {
                return true;
            }

            if (ground == null)
            {
                return true;
            }

            if (ground.IsLoading())
            {
                return true;
            }

            return false;
        }

        public override Vector3 GetRandomGroundPosition()
        {
            if (mainLine == null || lastLine == null)
            {
                return Vector3.zero;
            }

            var fieldRectangle = GetFieldRectangle();
            var dot = Vector2.Lerp(mainLine.GetBoundaryCenter(), lastLine.GetBoundaryCenter(), 0.5f);
            var ranVec = Util.AddAngle(Vector2.zero, Random.Range(0, 360f));
            var end = dot + ranVec * 1000f;

            for (int i = 0, len = fieldRectangle.Length; i < len; ++i)
            {
                var a = fieldRectangle[i];
                var b = fieldRectangle[(i + 1) % len];
                if (!Util.TryGetCrossLine(a, b, dot, end, out var result))
                {
                    continue;
                }

                // 벽에 부딪히면 부딪힌 점 반환
                return Vector3.Lerp(dot, result, Random.Range(0.2f, 0.8f));
            }

            return lastLine.GetBoundaryCenter();
        }

        public Vector3 GetGuideVector()
        {
            return mainLine.GetGuideVector();
        }

        private ResourceMode.GroundInfo GetRandomGroundInfo()
        {
            var groundInfos = mode.core.profile.resMode.groundInfos;
            if (groundInfos == null || groundInfos.Count == 0)
            {
                return null;
            }

            return groundInfos[Random.Range(0, groundInfos.Count)];
        }

        protected Vector2[] GetFieldRectangle()
        {
            if (mainLine == null || lastLine == null)
            {
                return fieldRectangle;
            }

            var a = mainLine.GetBoundary();
            var b = lastLine.GetBoundary();

            fieldRectangle[0] = a[0];
            fieldRectangle[1] = a[1];
            fieldRectangle[2] = b[1];
            fieldRectangle[3] = b[0];

#if UNITY_EDITOR
            Debug.DrawLine(fieldRectangle[0], fieldRectangle[1]);
            Debug.DrawLine(fieldRectangle[1], fieldRectangle[2]);
            Debug.DrawLine(fieldRectangle[2], fieldRectangle[3]);
            Debug.DrawLine(fieldRectangle[3], fieldRectangle[0]);
#endif

            return fieldRectangle;
        }
    }
}