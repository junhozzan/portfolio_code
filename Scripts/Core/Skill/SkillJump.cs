using Skill;

public class SkillJump : SkillBase
{
    public readonly new SkillJumpCoreComponent core = null;

    public static SkillJump Of()
    {
        return new SkillJump();
    }

    private SkillJump() : base()
    {
        this.core = base.core as SkillJumpCoreComponent;
    }

    protected override SkillCoreComponent CreateCoreComponent()
    {
        return SkillJumpCoreComponent.Of(this);
    }

    public override bool Play(SkillInfo skillInfo)
    {
        base.Play(skillInfo);

        return core.trigger.Exec();
    }
}
