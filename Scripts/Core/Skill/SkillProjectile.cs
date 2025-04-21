using UnityEngine;
using System;

namespace Skill
{
    public class SkillProjectile : SkillBase
    {
        public readonly new SkillProjectileCoreComponent core = null;

        public static SkillProjectile Of()
        {
            return new SkillProjectile();
        }

        private SkillProjectile() : base()
        {
            this.core = base.core as SkillProjectileCoreComponent;
        }

        protected override SkillCoreComponent CreateCoreComponent()
        {
            return SkillProjectileCoreComponent.Of(this);
        }

        public override bool Play(SkillInfo skillInfo)
        {
            base.Play(skillInfo);

            core.target.SetTarget();
            core.obj.CreateObject();
            core.move.SetOnMove();
            core.trigger.SetOnTriggers();

            return true;
        }
    }
}