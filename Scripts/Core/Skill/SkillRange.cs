using Skill;
using UnityEngine;

public class SkillRange : SkillBase
{
    public readonly new SkillRangeCoreComponent core = null;

    public static SkillRange Of()
    {
        return new SkillRange();
    }

    private SkillRange() : base()
    {
        this.core = base.core as SkillRangeCoreComponent;
    }

    protected override SkillCoreComponent CreateCoreComponent()
    {
        return SkillRangeCoreComponent.Of(this);
    }

    public override bool Play(SkillInfo skillInfo)
    {
        base.Play(skillInfo);
        core.target.SetTargets();
        core.obj.CreateObject();

        return true;
    }
}
