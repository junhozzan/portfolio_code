using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitComponent
{
    public class UnitSkillComponent : UnitBaseComponent
    {
        public readonly UnitSkillBonusComponent skillBonus = null;
        public readonly UnitSkillCoolTimeComponent coolTime = null;
        public readonly UnitSkillEnrollComponent enroll = null;

        private readonly SimplePool<UnitSkill> skillPool = SimplePool<UnitSkill>.Of(UnitSkill.Of, 16);
        private readonly Dictionary<int, UnitSkill> skills = new Dictionary<int, UnitSkill>();
        private readonly List<(int, long)> tempSkillInfos = new List<(int, long)>();

        public UnitSkillComponent(Unit owner) : base(owner)
        {
            skillBonus = AddComponent<UnitSkillBonusComponent>(owner);
            coolTime = AddComponent<UnitSkillCoolTimeComponent>(owner);
            enroll = AddComponent<UnitSkillEnrollComponent>(owner);
        }

        public override void DoReset()
        {
            base.DoReset();
            ClearSkills();
        }

        public override void OnDisable()
        {
            ClearSkills();
            base.OnDisable();
        }

        private void ClearSkills()
        {
            foreach (var skill in skills.Values)
            {
                skill.OnDisable();
            }

            skills.Clear();
            skillPool.Clear();
        }

        private void BuildSkillIDs()
        {
            tempSkillInfos.Clear();
            BuildSkillIDsByItems();
            BuildSkillIDsByBuffs();
        }

        private void BuildSkillIDsByItems()
        {
            var equipedItemIDs = owner.core.item.GetEquipedItemIDs();
            foreach (var item in owner.core.item.GetAllItems())
            {
                var resItem = item.resItem;
                if (resItem == null)
                {
                    continue;
                }

                if (equipedItemIDs.Contains(item.id))
                {
                    InputSkill(tempSkillInfos, item.GetLevel(), resItem.equip.targetAbilities, owner, owner);
                }

                InputSkill(tempSkillInfos, item.GetLevel(), resItem.hold.targetAbilities, owner, owner);
            }
        }

        private void BuildSkillIDsByBuffs()
        {
            foreach (var buff in owner.core.buff.buffScript.GetBuffsByScriptType(BuffScriptType.ADD_SKILL))
            {
                foreach (var script in buff.GetBuffScripts(BuffScriptType.ADD_SKILL))
                {
                    var tscript = script as BuffScript.BuffScriptAddSkill;
                    tempSkillInfos.Add((tscript.GetSkillID(buff.tbuff.GetLevel()), buff.tbuff.GetLevel()));
                }
            }
        }

        public void Refresh()
        {
            skillBonus.Refresh();

            ClearSkills();
            BuildSkillIDs();

            foreach (var info in tempSkillInfos)
            {
                var resSkill = ResourceManager.Instance.skill.GetSkill(info.Item1);
                if (resSkill == null)
                {
                    continue;
                }

                var tskill = TManager.Instance.Get<TSkill>()
                    .Set(resSkill.id)
                    .SetLevel(info.Item2)
                    .SetMaxCoolTime(GetSkillCoolTime(resSkill));

                Add(tskill);
            }

            coolTime.RefreshMinCoolTime();
        }

        private void Add(TSkill tskill)
        {
            var skill = skillPool.Pop();
            skill.SetOwner(owner);
            skill.UpdateTSkill(tskill);

            if (!skills.ContainsKey(skill.resSkill.id))
            {
                skills.Add(skill.resSkill.id, skill);
                coolTime.AddToMaxCoolTime(skill.resSkill);
            }
        }

        public float GetCoolTimePercentage(int skillID)
        {
            if (!TryGetSkill(skillID, out var skill))
            {
                return 0f;
            }

            var maxCoolTime = skill.GetMaxCoolTime();
            if (maxCoolTime <= 0f)
            {
                return 1f;
            }

            return coolTime.GetCoolTime(skillID) / maxCoolTime;
        }

        public bool IsCoolTime(int skillID)
        {
            return GetCoolTimePercentage(skillID) >= 1f;
        }

        public bool UseSkill(Unit target, ResourceSkill resSkill)
        {
            if (resSkill == null)
            {
                return false;
            }

            var skillInfo = SkillManager.Instance.PopInfo();
            skillInfo.SetResourceSkill(resSkill);
            skillInfo.SetFire(resSkill.fire);
            skillInfo.SetFrom(owner);
            skillInfo.SetTarget(target);

            if (!SkillManager.Instance.Play(skillInfo))
            {
                return false;
            }

            coolTime.ResetCoolTime(resSkill.id);
            PlayEffect(resSkill.effect, owner.core.transform, 20);

            return true;
        }

        public Dictionary<int, UnitSkill> GetSkills()
        {
            return skills;
        }

        public bool TryGetSkill(int skillID, out UnitSkill skill)
        {
            return skills.TryGetValue(skillID, out skill);
        }

        public float GetSkillCoolTime(ResourceSkill resSkill)
        {
            var skillBonuses = skillBonus.Get(ResourceSkillBonus.SkillBonusType.REDUCE_COOL_TIME_PERCENT);
            var percentage = 0f;
            var add = 0f;
            foreach (var e in skillBonuses)
            {
                var skillBonus = e.Item1;
                var level = e.Item2;

                if (!skillBonus.targetSkillIDs.Contains(resSkill.id))
                {
                    continue;
                }

                percentage += skillBonus.GetPercentage(level);
                add += skillBonus.GetAdd(level);
            }

            return Math.Max(resSkill.coolTime * (1f - percentage) - add, 0f);
        }

        public float GetSkillRange(ResourceSkill resSkill)
        {
            var skillBonuses = skillBonus.Get(ResourceSkillBonus.SkillBonusType.INCREASE_SKILL_RANGE);
            var percentage = 0f;
            var add = 0f;
            foreach (var e in skillBonuses)
            {
                var skillBonus = e.Item1;
                var level = e.Item2;

                if (!skillBonus.targetSkillIDs.Contains(resSkill.id))
                {
                    continue;
                }

                percentage += skillBonus.GetPercentage(level);
                add += skillBonus.GetAdd(level);
            }

            return resSkill.skillRange * Mathf.Max(1f + percentage, 0f) + add;
        }

        public virtual void PlayEffect(ResourceEffect effect, IEffectPointer effectPointer, int maximum = 1000)
        {
            PlayEffectSound(effect);

            if (string.IsNullOrEmpty(effect.prefab))
            {
                return;
            }

            if (effectPointer == null)
            {
                return;
            }

            var point = effectPointer.GetPoint(effect.pointType);
            if (!point.HasValue)
            {
                return;
            }

            var obj = ObjectManager.Instance.Pop<CpEffect>(effect.prefab, maximum: maximum);
            if (obj == null)
            {
                return;
            }

            obj.SetPosition(point.Value);
            obj.SetLayer(effect.layer);
            obj.SetFlip(owner.core.move.GetMoveDirection().x < 0);
            if (!effect.IsInfinity())
            {
                obj.DelayInactive(effect.time);
            }
        }

        protected virtual void PlayEffectSound(ResourceEffect effect)
        {
            if (string.IsNullOrEmpty(effect.sound))
            {
                return;
            }

            SoundManager.Instance.PlaySfx(effect.sound);
        }

        protected static void InputSkill(
            List<(int, long)> modeSkills,
            long level,
            ICollection<ResourceTargetAbility> targetAbilities,
            Unit from,
            Unit target)
        {
            foreach (var ability in targetAbilities)
            {
                if (!UnitRule.IsTargetable(from, target, ability.applyTargets))
                {
                    continue;
                }

                foreach (var skillID in ability.skillIDs)
                {
                    modeSkills.Add((skillID, level));
                }
            }
        }

#if UNITY_EDITOR
        private static System.Text.StringBuilder sbDebugState = new System.Text.StringBuilder();
        public string DebugString()
        {
            sbDebugState.Clear();
            foreach (var skill in skills.Values)
            {
                sbDebugState.Append($"resSkillID:[{skill.resSkill.id}] isCoolTime:[{IsCoolTime(skill.resSkill.id)}]\n");
            }

            return sbDebugState.ToString();
        }
#endif
    }
}