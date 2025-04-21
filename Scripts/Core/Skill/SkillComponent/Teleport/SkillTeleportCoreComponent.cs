namespace Skill
{
    public class SkillTeleportCoreComponent : SkillCoreComponent
    {
        public readonly new SkillTeleportProfileComponent profile = null;
        public readonly SkillTeleportTriggerComponent trigger = null;
        public readonly SkillFinishComponent finish = null;

        protected SkillTeleportCoreComponent(SkillTeleport skill) : base(skill)
        {
            this.profile = base.profile as SkillTeleportProfileComponent;
            this.trigger = AddComponent<SkillTeleportTriggerComponent>(skill);
            this.finish = AddComponent<SkillFinishComponent>(skill);
        }

        public static SkillTeleportCoreComponent Of(SkillTeleport skill)
        {
            return new SkillTeleportCoreComponent(skill);
        }

        protected override SkillProfileComponent AddProfileComponent()
        {
            return AddComponent<SkillTeleportProfileComponent>(skill);
        }
    }
}