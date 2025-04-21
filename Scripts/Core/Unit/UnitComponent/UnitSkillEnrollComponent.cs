using System.Collections.Generic;
using System;

namespace UnitComponent
{
    public class UnitSkillEnrollComponent : UnitBaseComponent
    {
        private readonly Dictionary<SkillType, Queue<int>> skillIDs = new Dictionary<SkillType, Queue<int>>();
        private readonly List<int> tempSortedIDs = new List<int>(16);
        private readonly List<int> tempBackIDs = new List<int>(16);

        public UnitSkillEnrollComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            ClearSkillIDs();
        }

        public override void OnDisable()
        {
            ClearSkillIDs();
            base.OnDisable();
        }

        private void ClearSkillIDs()
        {
            foreach (var skillIDs in skillIDs.Values)
            {
                skillIDs.Clear();
            }
        }

        public int GetSkillID(SkillType type)
        {
            var queue = GetQueue(type);
            if (queue.Count == 0)
            {
                return -1;
            }

            return queue.Peek();
        }

        public void Enqueue(SkillType type, int skillID)
        {
            GetQueue(type).Enqueue(skillID);
        }

        public void Dequeue(SkillType type, int skillID)
        {
            var queue = GetQueue(type);
            if (queue.Count == 0)
            {
                return;
            }

            if (skillID != queue.Peek())
            {
                return;
            }

            queue.Dequeue();
        }

        public void Sort(SkillType type, Unit target)
        {
            var queue = GetQueue(type);
            if (queue.Count == 0)
            {
                return;
            }

            Util.IncreaseCapacity(tempSortedIDs, queue.Count);
            Util.IncreaseCapacity(tempBackIDs, queue.Count);

            tempSortedIDs.Clear();
            tempBackIDs.Clear();

            foreach (var skillID in queue)
            {
                if (!owner.core.skill.TryGetSkill(skillID, out var skill))
                {
                    continue;
                }

                // 사용 하지 못하는 스킬이면 리스트 뒤로
                if (!SkillRule.IsTargetInSkillRange(owner, target, skill.resSkill))
                {
                    tempBackIDs.Add(skillID);
                    continue;
                }

                if (skill.resSkill.skillType == SkillType.SKILL_BUFF)
                {
                    // 이미 보유중인 버프면 리스트 뒤로
                    if (target.core.buff.HasBuffByFromResSkillID(skill.resSkill.id))
                    {
                        tempBackIDs.Add(skillID);
                        continue;
                    }
                }

                tempSortedIDs.Add(skillID);
            }

            tempSortedIDs.AddRange(tempBackIDs);

            queue.Clear();
            foreach (var sortedID in tempSortedIDs)
            {
                queue.Enqueue(sortedID);
            }

            tempBackIDs.Clear();
            tempSortedIDs.Clear();
        }

        public bool IsContains(SkillType type, int skillID)
        {
            var queue = GetQueue(type);
            if (queue.Count == 0)
            {
                return false;
            }

            return queue.Contains(skillID);
        }

        public ICollection<Queue<int>> GetAll()
        {
            return skillIDs.Values;
        }

        private Queue<int> GetQueue(SkillType type)
        {
            if (!skillIDs.TryGetValue(type, out var skills))
            {
                skillIDs.Add(type, skills = new Queue<int>());
            }

            return skills;
        }

        public float GetMinSkillRange(ICollection<SkillType> ignoreTypes = null)
        {
            var minRange = float.MaxValue;
            foreach (var skillType in skillIDs.Keys)
            {
                if (ignoreTypes != null && ignoreTypes.Contains(skillType))
                {
                    continue;
                }

                var skillID = GetSkillID(skillType);
                if (!owner.core.skill.TryGetSkill(skillID, out var skill))
                {
                    continue;
                }

                minRange = Math.Min(minRange, owner.core.skill.GetSkillRange(skill.resSkill));
            }

            return minRange;
        }
    }
}