namespace Skill
{
    /// <summary>
    /// 가장 마지막에 생성되어야 할당 되어야 함으로 각 코어에서 추가
    /// </summary>
    public class SkillFinishComponent : SkillBaseComponent
    {
        private bool isFinish = false;

        public SkillFinishComponent(SkillBase skill) : base(skill)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            isFinish = false;
        }

        public void Finish()
        {
            isFinish = true;
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            if (!isFinish)
            {
                return;
            }

            skill.OnDisable();
        }
    }
}