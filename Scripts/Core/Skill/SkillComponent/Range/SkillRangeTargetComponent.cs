using System.Collections.Generic;

namespace Skill
{
    public class SkillRangeTargetComponent : SkillBaseComponent
    {
        private readonly new SkillRange skill = null;
        private readonly List<Unit> targets = new List<Unit>();
        private readonly List<Unit> emptyTargets = new List<Unit>();

        public SkillRangeTargetComponent(SkillRange skill) : base(skill)
        {
            this.skill = skill;

            GameEvent.Instance.CreateHandler(this, skill.IsUsed)
                .Add(GameEventType.CHANGE_UNIT_COUNT, Handle_UPDATE_UNIT_COUNT)
                ;
        }

        private void Handle_UPDATE_UNIT_COUNT(object[] args)
        {
            targets.Clear();
            targets.AddRange(FindTargets());
        }

        public override void DoReset()
        {
            base.DoReset();
            targets.Clear();
        }

        public void SetTargets()
        {
            targets.Clear();
            targets.AddRange(FindTargets());
        }

        private List<Unit> FindTargets()
        {
            var from = skill.core.profile.skillInfo._from;
            if (!UnitRule.IsAlive(from))
            {
                return emptyTargets;
            }

            return UnitManager.Instance.FindTargets(
                from,
                skill.core.profile.skillInfo.resFire.applyTargets,
                skill.core.hit.hitedTargetUIDs);
        }

        public List<Unit> GetTargets()
        {
            return targets;
        }
    }
}