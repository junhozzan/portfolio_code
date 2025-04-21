using System;
using behaviorTree;

namespace ModeComponent
{
    public class ModeAIComponent : ModeBaseComponent
    {
        protected readonly BehaviorTree ai = null;
        protected readonly BlackBoard blackBoard = null;

        protected const string BBKEY_AI_DELAY = "AI_DELAY_NEXT";
        protected const string BBKEY_MODE_START = "MODE_START";
        protected const string BBKEY_MODE_PLAY = "MODE_PLAY";
        protected const string BBKEY_SET_NEW_GOUNRD = "SET_NEW_GROUND";
        protected const string BBKEY_MONSTER_NEXT_SPAWN_AT = "MONSTER_NEXT_SPAWN_AT";

#if UNITY_EDITOR
        public BehaviorTree _ai => ai;
#endif

        public ModeAIComponent(Mode mode) : base(mode)
        {
            blackBoard = BlackBoard.Of();
            ai = CreateAI();
        }

        public override void DoReset()
        {
            base.DoReset();
            blackBoard.Clear();
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            if (ai == null)
            {
                return;
            }

            ai.Tick(dt);
        }

        protected int AI_IsPause()
        {
            var delay = blackBoard.GetFloat(BBKEY_AI_DELAY);
            if (delay > Main.Instance.time.realtimeSinceStartup)
            {
                return Node.Status.SUCCESS;
            }

            if (UIGuide.CpUI_Guide.IsPlay())
            {
                return Node.Status.SUCCESS;
            }

            return Node.Status.FAILURE;
        }

        protected int AI_ModeStartable()
        {
            if (mode.core.state.CheckState(ModeStateComponent.ModeState.JOIN_FIELD) 
                || mode.core.state.CheckState(ModeStateComponent.ModeState.JOIN_MY_UNIT))
            {
                return Node.Status.FAILURE;
            }

            return Node.Status.SUCCESS;
        }

        protected void AI_ModeStart()
        {
            if (blackBoard.GetBool(BBKEY_MODE_START))
            {
                return;
            }

            blackBoard.SetBool(BBKEY_MODE_START, true);
            GameEvent.Instance.AddEvent(GameEventType.MODE_START);
        }

        protected int AI_CheckPlayable()
        {
            if (mode.core.profile.resMode == null || mode.core.field.IsLoading() || mode.core.ally.IsLoading())
            {
                return Node.Status.FAILURE;
            }

            if (!mode.core.state.CheckState(ModeStateComponent.ModeState.PLAYING))
            {
                return Node.Status.FAILURE;
            }

            return Node.Status.SUCCESS;
        }

        protected void AI_ModePlay()
        {
            if (blackBoard.GetBool(BBKEY_MODE_PLAY))
            {
                return;
            }

            blackBoard.SetBool(BBKEY_MODE_PLAY, true);
            GameEvent.Instance.AddEvent(GameEventType.MODE_PLAY);
        }

        protected virtual int AI_CheckMonsterSpawnable()
        {
            if (mode.IsLoading())
            {
                return Node.Status.FAILURE;
            }

            var aliveSpawnMonster = mode.core.enemy.GetAliveEnemyCount();
            if (aliveSpawnMonster >= mode.core.enemy.GetMaxFieldSpawnCount())
            {
                return Node.Status.FAILURE;
            }

            var totalSpawnMonster = mode.core.enemy.GetTotalSpawnEnemyCount();
            if (totalSpawnMonster >= mode.core.enemy.GetMaxSpawnCount())
            {
                return Node.Status.FAILURE;
            }

            var nextSpawnAt = blackBoard.GetDateTime(BBKEY_MONSTER_NEXT_SPAWN_AT, DateTime.MinValue);
            if (nextSpawnAt > Main.Instance.time.now)
            {
                return Node.Status.FAILURE;
            }

            return Node.Status.SUCCESS;
        }

        protected void AI_SpawnMosnter()
        {
            blackBoard.SetDateTime(BBKEY_MONSTER_NEXT_SPAWN_AT, Main.Instance.time.now.AddSeconds(mode.core.profile.resMode.spawnDelayTime));
            mode.core.enemy.SpawnEnemyUnit();
        }

        protected virtual BehaviorTree CreateAI()
        {
            return null;
        }
    }
}
