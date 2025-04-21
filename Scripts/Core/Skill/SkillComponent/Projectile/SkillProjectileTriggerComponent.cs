using System;
using System.Collections.Generic;

namespace Skill
{
    public class SkillProjectileTriggerComponent : SkillBaseComponent
    {
        private readonly new SkillProjectile skill = null;
        private readonly List<Func<float, bool>> onTriggers = new List<Func<float, bool>>(2);

        public SkillProjectileTriggerComponent(SkillProjectile skill) : base(skill)
        {
            this.skill = skill;
        }

        public override void DoReset()
        {
            base.DoReset();
            onTriggers.Clear();
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);

            if (onTriggers.Count == 0)
            {
                return;
            }

            foreach (var onUpdateTrigger in onTriggers)
            {
                if (!onUpdateTrigger(dt))
                {
                    continue;
                }

                skill.core.finish.Finish();
                return;
            }
        }

        public void SetOnTriggers()
        {
            foreach (var triggerType in skill.core.profile.resScript.triggerTypes)
            {
                Func<float, bool> func = null;
                switch (triggerType)
                {
                    case ResourceSkillProjectile.TriggerType.MOVING:
                        func = OnTrigger_MOVING;
                        break;
                    case ResourceSkillProjectile.TriggerType.ENDMOVE:
                        func = OnTrigger_ENDMOVE;
                        break;
                }

                if (func == null || onTriggers.Contains(func))
                {
                    continue;
                }

                onTriggers.Add(func);
            }
        }

        /// <summary>
        /// 이동중 충돌
        /// </summary>
        private bool OnTrigger_MOVING(float dt)
        {
            var targets = skill.core.target.GetTargets();
            if (targets.Count == 0)
            {
                return false;
            }

            for (int i = 0, cnt = targets.Count; i < cnt; ++i)
            {
                var target = targets[i];
                if (!IsTrigger(target))
                {
                    continue;
                }

                skill.core.hit.HitTarget(target);
                if (skill.core.hit.IsMaxHitTarget())
                {
                    return true;
                }

                if (!SkillRule.IsPenetrate(skill.core.profile.skillInfo, target))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 타겟 충돌시 호출
        /// </summary>
        private bool OnTrigger_ENDMOVE(float dt)
        {
            if (!skill.core.move.isEndMove)
            {
                return false;
            }

            var target = skill.core.target.GetTarget();
            if (UnitRule.IsAlive(target))
            {
                skill.core.hit.HitTarget(target);
            }

            return true;
        }

        private bool IsTrigger(Unit target)
        {
            if (!UnitRule.IsValid(target))
            {
                return false;
            }

            return skill.core.obj.IsTrigger(target);
        }
    }
}