
namespace ModeComponent
{
    public class KillModeScoreComponent : ModeScoreComponent
    {
        private long enemyKillCount = 0;

        public KillModeScoreComponent(Mode mode) : base(mode)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.DEAD_UNIT, Handle_DEAD_UNIT)
                ;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            enemyKillCount = 0;
        }

        private void Handle_DEAD_UNIT(object[] args)
        {
            var unit = GameEvent.GetSafe<Unit>(args, 0);
            if (unit == null)
            {
                return;
            }

            if (!mode.core.enemy.IsSpawnedEnemy(unit))
            {
                return;
            }

            ++enemyKillCount;
            GameEvent.Instance.AddEvent(GameEventType.REFRESH_SCORE);
        }

        public override long GetScore()
        {
            return enemyKillCount;
        }
    }
}