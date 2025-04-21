using UnityEngine;

namespace Skill
{
    public class SkillRangeObjectComponent : SkillBaseComponent
    {
        private readonly new SkillRange skill = null;
        private CpRange obj = null;

        public SkillRangeObjectComponent(SkillRange skill) : base(skill)
        {
            this.skill = skill;
        }

        public override void DoReset()
        {
            base.DoReset();
            obj = null;
        }

        public void CreateObject()
        {
            var res = skill.core.profile.resScript;
            if (string.IsNullOrEmpty(res.prefab))
            {
                Debug.Log(S.Red("## range skill object prefab is null"));
                return;
            }

            obj = ObjectManager.Instance.Pop<CpRange>(res.prefab);
            obj.SetLayer(res.layer);
            obj.Set(skill.core.profile.skillInfo.shotPosition, 1f);
            obj.TweenDynamicScale(res.scaleFrom, res.scaleTo, res.scaleDuration);
        }

        public void SetPosition(Vector3 pos)
        {
            obj.SetPosition(pos);
        }

        public Vector3 GetPosition()
        {
            return obj.GetPosition();
        }

        public bool IsTrigger(Unit target)
        {
            return obj.IsTrigger(target.core.transform.GetMyCollider());
        }
    }
}