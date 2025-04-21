using System.Collections.Generic;
using UnityEngine;

namespace UnitComponent
{
    public class UnitSkillCoolTimeComponent : UnitBaseComponent
    {
        private readonly Dictionary<int, float> coolTimes = new Dictionary<int, float>();

        public float minCoolTime { get; private set; } = 0f;

        // 쿨타임이 60초를 넘어가는 스킬은 없다.
        private const float SKILL_COOL_MAX_VALUE = 60f;

        public UnitSkillCoolTimeComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            coolTimes.Clear();
            minCoolTime = 0f;
        }

        public override void OnDisable()
        {
            coolTimes.Clear();
            base.OnDisable();
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            UpdateCoolTime(dt);
        }

        private void UpdateCoolTime(float dt)
        {
            var skills = owner.core.skill.GetSkills();
            foreach (var skillID in skills.Keys)
            {
                if (!coolTimes.TryGetValue(skillID, out var v))
                {
                    continue;
                }

                if (v >= SKILL_COOL_MAX_VALUE)
                {
                    continue;
                }

                coolTimes[skillID] = v + dt;
            }
        }

        public void RefreshMinCoolTime()
        {
            var skills = owner.core.skill.GetSkills();
            var value = SKILL_COOL_MAX_VALUE;
            foreach (var skillID in skills.Values)
            {
                value = Mathf.Min(skillID.GetMaxCoolTime(), value);
            }

            minCoolTime = value;
        }

        public void AddToMaxCoolTime(ResourceSkill resSkill)
        {
            if (coolTimes.ContainsKey(resSkill.id))
            {
                return;
            }

            coolTimes.Add(resSkill.id, resSkill.initCool ? SKILL_COOL_MAX_VALUE : 0f);
        }

        public void ResetCoolTime(int skillID)
        {
            if (!coolTimes.ContainsKey(skillID))
            {
                return;
            }

            coolTimes[skillID] = 0f;
        }

        public float GetCoolTime(int skillID)
        {
            return coolTimes.TryGetValue(skillID, out var v) ? v : 0f;
        }
    }
}