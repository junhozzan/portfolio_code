namespace ModeComponent
{
    public class DamageModeScoreComponent : ModeScoreComponent
    {
        public long score = 0;

        public DamageModeScoreComponent(Mode mode) : base(mode)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.ENEMY_ATTACKED, Handle_ENEMY_ATTACKED)
                ;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            score = 0;
        }

        private void Handle_ENEMY_ATTACKED(object[] args)
        {
            var tArg = GameEvent.GetSafeS<EnemyAttacked>(args, 0);
            if (tArg == null)
            {
                return;
            }

            score += tArg.Value.damaged;
            GameEvent.Instance.AddEvent(GameEventType.REFRESH_SCORE);
        }

        public override long GetScore()
        {
            return score;
        }
    }
}