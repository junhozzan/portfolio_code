using System.Collections;
using UnityEngine;

namespace Skill
{
    public class SkillAttackProfileComponent : SkillProfileComponent
    {
        public new ResourceSkillAttack resScript = null;

        public SkillAttackProfileComponent(SkillBase skill) : base(skill)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            resScript = null;
        }

        public override void Set(SkillInfo skillInfo)
        {
            base.Set(skillInfo);
            this.resScript = base.resScript as ResourceSkillAttack;
        }
    }
}