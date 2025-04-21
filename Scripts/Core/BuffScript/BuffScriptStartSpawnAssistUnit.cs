namespace BuffScript
{
    public class BuffScriptStartSpawnAssistUnit : BuffScriptSpawnUnit
    {
        protected ResourceBuffScriptStartSpawnAssistUnit resBuffScript = null;

        private Unit spawnedUnit = null;

        public static BuffScriptStartSpawnAssistUnit Of()
        {
            return new BuffScriptStartSpawnAssistUnit();
        }

        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptStartSpawnAssistUnit;
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.MODE_PLAY, Handle_MODE_PLAY);
        }

        public void Handle_MODE_PLAY(object[] args)
        {

        }

        public override void On()
        {
            base.On();

        }

        private void Spawn()
        {
            if (spawnedUnit != null)
            {
                return;
            }
        }
    }
} 