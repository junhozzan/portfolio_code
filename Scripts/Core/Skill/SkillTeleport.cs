using Skill;

public class SkillTeleport : SkillBase
{
    public readonly new SkillTeleportCoreComponent core = null;

    public static SkillTeleport Of()
    {
        return new SkillTeleport();
    }

    private SkillTeleport() : base()
    {
        this.core = base.core as SkillTeleportCoreComponent;
    }

    protected override SkillCoreComponent CreateCoreComponent()
    {
        return SkillTeleportCoreComponent.Of(this);
    }

    public override bool Play(SkillInfo skillInfo)
    {
        base.Play(skillInfo);
        return core.trigger.Exec();
    }
}
