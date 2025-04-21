using System;
using System.Linq;
using UnityEngine;
using behaviorTree;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public abstract class AIScript : Node
{
    public readonly Unit owner = null;
    public readonly BlackBoard blackBoard = null;
    public readonly Node ai = null;

    protected static readonly ReadOnlyCollection<SkillType> cachedSkillTypes = Util.GetEnumArray<SkillType>()
        .Where(x => x != SkillType.NONE)
        .ReadOnly();

    public AIScript(Unit unit)
    {
        this.owner = unit;
        this.blackBoard = new BlackBoard();
        this.ai = CreateAI();

#if UNITY_EDITOR
        this.debugOption = new DebugOption(this, name: GetType().Name);
#endif
    }

    public abstract Node CreateAI();

    public override void Reset()
    {
        base.Reset();
        ai.Reset();
        blackBoard.Clear();
    }

    protected override int Update(float dt)
    {
        return ai.Tick(dt);
    }

    protected int Pause()
    {
        var mode = ModeManager.Instance.mode;
        if (mode == null)
        {
            return Status.SUCCESS;
        }

        if (mode.IsLoading())
        {
            return Status.SUCCESS;
        }

        if (!mode.core.state.CheckState(ModeComponent.ModeStateComponent.ModeState.PLAYING))
        {
            return Status.SUCCESS;
        }

        if (UIGuide.CpUI_Guide.IsPlay())
        {
            return Status.SUCCESS;
        }

        return Status.FAILURE;
    }

    protected int IsAlive()
    {
        if (!UnitRule.IsAlive(owner))
        {
            return Status.FAILURE;
        }

        return Status.SUCCESS;
    }

    protected int IsActivity()
    {
        if (owner.core.jump.IsJumpState())
        {
            return Status.FAILURE;
        }

        return Status.SUCCESS;
    }

    protected void ResearchTarget()
    {
        foreach (var key in cachedSkillTypes)
        {
            var target = owner.core.target.GetTarget(key);
            if (target == null)
            {
                continue;
            }

            var targetUID = owner.core.target.GetTargetUID(key); // 타켓 변화 검증 변수
            if (!UnitRule.IsAlive(target) || (targetUID > 0 && target.core.profile.tunit.uid != targetUID))
            {
                owner.core.target.SetTarget(key, null);
            }
        }
    }

    protected void RemoveSkill()
    {
        var isRemoveSkill = false;
        var skills = owner.core.skill.GetSkills();
        // NONE 제외
        for (int i = 0, cnt = cachedSkillTypes.Count; i < cnt;)
        {
            var skillType = cachedSkillTypes[i];
            var skillID = owner.core.skill.enroll.GetSkillID(skillType);
            if (skillID < 0 || skills.ContainsKey(skillID))
            {
                ++i;
                continue;
            }

            owner.core.skill.enroll.Dequeue(skillType, skillID);
            isRemoveSkill = true;
        }

        if (isRemoveSkill)
        {
            blackBoard.SetFloat(BBKEY_MIN_SKILL_RANGE, owner.core.skill.enroll.GetMinSkillRange());
        }
    }

    protected void RegistSkill()
    {
        var isAddSkill = false;
        var skills = owner.core.skill.GetSkills();
        foreach (var skill in skills.Values)
        {
            // 쿨타임 확인
            if (!owner.core.skill.IsCoolTime(skill.resSkill.id))
            {
                continue;
            }

            if (owner.core.skill.enroll.IsContains(skill.resSkill.skillType, skill.resSkill.id))
            {
                continue;
            }

            // 사용 가능한 타겟 확인
            var targets = GetTargetsFromSkill(skill);
            var target = SkillRule.GetNearTarget(targets, owner.core.transform.GetPosition());
            if (!UnitRule.IsAlive(target))
            {
                continue;
            }

            owner.core.skill.enroll.Enqueue(skill.resSkill.skillType, skill.resSkill.id);
            isAddSkill = true;
        }

        if (isAddSkill)
        {
            blackBoard.SetFloat(BBKEY_MIN_SKILL_RANGE, owner.core.skill.enroll.GetMinSkillRange());
        }
    }

    protected int IsExistSkill(SkillType skillType)
    {
        var skillID = owner.core.skill.enroll.GetSkillID(skillType);
        if (skillID < 0)
        {
            return Status.FAILURE;
        }

        return Status.SUCCESS;
    }

    protected virtual List<Unit> GetTargetsFromSkill(UnitSkill skill)
    {
        return UnitManager.Instance.FindTargets(owner, skill.resSkill.fire.applyTargets, owner.core.target.GetIgnoreTargetUIDs());
    }

    protected int FindTargetFromSkill(SkillType skillType)
    {
        var target = owner.core.target.GetTarget(skillType);
        var priorityTarget = owner.core.target.GetPriorityTarget();

        if (!UnitRule.IsAlive(target)
            || (priorityTarget != null && priorityTarget != target))
        {
            var skillID = owner.core.skill.enroll.GetSkillID(skillType);
            if (!owner.core.skill.TryGetSkill(skillID, out var skill))
            {
                return Status.FAILURE;
            }

            var filterTargets = new List<Unit>();
            var targets = GetTargetsFromSkill(skill);
            if (skill.resSkill.skillType == SkillType.SKILL_BUFF)
            {
                foreach (var _target in targets)
                {
                    var filter = _target.core.buff.HasBuffByFromResSkillID(skill.resSkill.id);
                    if (filter)
                    {
                        continue;
                    }

                    filterTargets.Add(_target);
                }
            }
            else
            {
                filterTargets.AddRange(targets);
            }

            if (priorityTarget != null && filterTargets.Contains(priorityTarget))
            {
                target = priorityTarget;
            }
            else
            {
                target = SkillRule.GetNearTarget(filterTargets, owner.core.transform.GetPosition());
            }

            owner.core.target.SetTarget(skillType, target);

            filterTargets.Clear();
        }

        if (!UnitRule.IsAlive(target))
        {
            return Status.FAILURE;
        }

        return Status.SUCCESS;
    }

    protected int IsTargetInSkillRange(SkillType skillType)
    {
        var skillID = owner.core.skill.enroll.GetSkillID(skillType);
        if (!owner.core.skill.TryGetSkill(skillID, out var skill))
        {
            return Status.FAILURE;
        }

        var target = owner.core.target.GetTarget(skillType);
        if (!skill.IsTargetInSkillRange(target))
        {
            return Status.FAILURE;
        }

        return Status.SUCCESS;
    }

    protected int SortSkill(SkillType skillType)
    {
        if (skillType != SkillType.SKILL_ATTACK)
        {
            return Status.FAILURE;
        }

        var target = owner.core.target.GetTarget(skillType);
        if (!UnitRule.IsAlive(target))
        {
            return Status.FAILURE;
        }

        var rtss = Main.Instance.time.realtimeSinceStartup;
        var atUpdate = blackBoard.GetFloat(BBKEY_AT_SORT_UPDATE, 0f);
        if (atUpdate > rtss)
        {
            return Status.FAILURE;
        }

        blackBoard.SetFloat(BBKEY_AT_SORT_UPDATE, rtss + 0.4f);
        owner.core.skill.enroll.Sort(skillType, target);

        return Status.FAILURE;
    }

    protected int IsUsableSkill()
    {
        if (!owner.core.status.IsSkillUsable())
        {
            return Status.FAILURE;
        }

        return Status.SUCCESS;
    }

    protected int UseSkill(SkillType skillType)
    {
        var skillID = owner.core.skill.enroll.GetSkillID(skillType);
        if (!owner.core.skill.TryGetSkill(skillID, out var skill))
        {
            return Status.FAILURE;
        }

        var target = owner.core.target.GetTarget(skillType);
        if (!owner.core.skill.UseSkill(target, skill.resSkill))
        {
            return Status.FAILURE;
        }

        var dir = target == owner ? (Vector3)owner.core.move.GetMoveDirection() : (target.core.transform.GetPosition() - owner.core.transform.GetPosition());
        owner.core.move.SetDirection(dir);
        owner.core.transform.UpdateFlip();
        owner.core.buff.HandleUseSkill(target);
        owner.core.target.SetTarget(skillType, null);
        owner.core.skill.enroll.Dequeue(skillType, skillID);

        if (skill.resSkill.isUseMotion)
        {
            owner.core.anim.Attack(skill);
        }

        return Status.SUCCESS;
    }

    protected int IsMovableToTarget()
    {
        if (!owner.core.status.IsMovable())
        {
            return Status.FAILURE;
        }

        var target = owner.core.target.GetFollowTarget();
        if (!UnitRule.IsAlive(target))
        {
            return Status.FAILURE;
        }

        return Status.SUCCESS;
    }

    protected int IsAttackMotion()
    {
        if (!owner.core.anim.IsAttackMotion())
        {
            return Status.FAILURE;
        }

        return Status.SUCCESS;
    }

    protected int IsNearTarget()
    {
        var target = owner.core.target.GetFollowTarget();
        if ((target.core.transform.GetPosition() - owner.core.transform.GetPosition()).sqrMagnitude > Math.Pow(target.core.profile.tunit.resUnit.radius + owner.core.profile.tunit.resUnit.radius, 2))
        {
            return Status.FAILURE;
        }

        return Status.SUCCESS;
    }

    protected int SetMovePointFromTarget()
    {
        var target = owner.core.target.GetFollowTarget();
        if (!UnitRule.IsAlive(target))
        {
            return Status.FAILURE;
        }

        var targetPos = target.core.transform.GetPosition();
        var dir = (targetPos - owner.core.transform.GetPosition()).normalized;

        owner.core.move.SetDirection(dir);
        owner.core.move.SetMovePoint(targetPos);
        owner.core.transform.UpdateFlip();

        return Status.SUCCESS;
    }

    protected int IsTargetInMinSkillRange()
    {
        var target = owner.core.target.GetFollowTarget();
        if (!UnitRule.IsAlive(target))
        {
            return Status.FAILURE;
        }

        var minRage = blackBoard.GetFloat(BBKEY_MIN_SKILL_RANGE);
        if (!minRage.HasValue)
        {
            return Status.FAILURE;
        }

        if (!SkillRule.InRange(owner, target, minRage.Value))
        {
            return Status.FAILURE;
        }

        return Status.SUCCESS;
    }

    protected void MoveToPoint(float dt)
    {
        owner.core.move.MoveTo(dt);
        owner.core.anim.Run();
    }

    protected int StopMove()
    {
        owner.core.move.Stop();
        owner.core.anim.Idle();

        return Status.SUCCESS;
    }

    protected Node UseSkillNodes()
    {
        return 
            new Selector(
                //UseSkillNode(SkillType.SKILL_BUFF),
                UseSkillNode(SkillType.SKILL_TELEPORT),
                UseSkillNode(SkillType.SKILL_JUMP),
                UseSkillNode(SkillType.SKILL_ATTACK),
                UseSkillNode(SkillType.DEFAULT_ATTACK)
            );
    }

    private Node UseSkillNode(SkillType skillType)
    {
        return 
            new Sequence(
                new AIResultAction(() => IsExistSkill(skillType), "IsExistSkill"),
                new AIAction(ResearchTarget),
                new AIResultAction(() => FindTargetFromSkill(skillType), "FindTargetFromSkill"),
                new Selector(
                    new Sequence(
                        new AIResultAction(() => IsTargetInSkillRange(skillType), "IsTargetInSkillRange"),
                        new AIResultAction(StopMove)
                        ),
                    new AIResultAction(() => SortSkill(skillType), "SortSkill")
                    ),
                new AIResultAction(IsUsableSkill),
                new AIResultAction(StopMove),
                new AIResultAction(() => UseSkill(skillType), "UseSkill")
            );
    }

    protected Node DefaultMove()
    {
        return
            new Sequence(
                new AIResultAction(IsMovableToTarget),
                new AIResultAction(SetMovePointFromTarget),
                new Selector(
                    new Sequence(
                        new AIResultAction(IsTargetInMinSkillRange),
                        new AIResultAction(StopMove)
                    ),
                    new Sequence(
                        new AIResultAction(IsNearTarget),
                        new AIResultAction(StopMove)
                    ),
                    new AIAction(MoveToPoint)
                )
            );
    }

    protected const string BBKEY_MIN_SKILL_RANGE = "MIN_SKILL_RANGE";
    protected const string BBKEY_AT_SORT_UPDATE = "AT_SORT_UPDATE";
}
