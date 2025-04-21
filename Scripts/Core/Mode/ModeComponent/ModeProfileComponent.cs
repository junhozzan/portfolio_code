namespace ModeComponent
{
    public class ModeProfileComponent : ModeBaseComponent
    {
        public readonly ResourceMode resMode = null;

        public ModeProfileComponent(Mode mode) : base(mode)
        {
            this.resMode = mode.resMode;
        }

        public virtual long GetStage()
        {
            return 1;
        }

        public virtual long GetModeLevel()
        {
            return 1;
        }

        public virtual float GetExpRate()
        {
            return 0f;
        }

        public virtual int StageToIndex()
        {
            return (int)(GetStage() % 10);
        }
    }
}