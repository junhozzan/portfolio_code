using Skill;

public class SkillAttack : SkillBase
{
    public readonly new SkillAttackCoreComponent core = null;

    public static SkillAttack Of()
    {
        return new SkillAttack();
    }

    private SkillAttack() : base()
    {
        this.core = base.core as SkillAttackCoreComponent;
    }

    protected override SkillCoreComponent CreateCoreComponent()
    {
        return SkillAttackCoreComponent.Of(this);
    }

    public override bool Play(SkillInfo skillInfo)
    {
        base.Play(skillInfo);
        return core.trigger.Exec();
    }
}
