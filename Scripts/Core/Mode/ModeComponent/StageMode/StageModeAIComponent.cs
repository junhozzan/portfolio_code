using behaviorTree;
using System;

namespace ModeComponent
{
    public class StageModeAIComponent : ModeAIComponent
    {
        private readonly new StageMode mode = null;

        public StageModeAIComponent(StageMode mode) : base(mode)
        {
            this.mode = mode;
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.MODE_SUCCESS, Handle_MODE_SUCCESS)
                .Add(GameEventType.MODE_FAIL, Handle_MODE_FAIL)
                ;
        }

        protected void AI_DropAndNewGround()
        {
            if (blackBoard.GetBool(BBKEY_SET_NEW_GOUNRD))
            {
                return;
            }

            blackBoard.SetBool(BBKEY_SET_NEW_GOUNRD, true);

            mode.core.camera.SetToZero();
            mode.core.field.SetToZero();
            mode.core.ally.SetToZero();

            mode.core.field.DropAndNewGround();
        }

        private void Handle_MODE_SUCCESS(object[] args)
        {
            blackBoard.SetBool(BBKEY_SET_NEW_GOUNRD, mode.core.profile.GetStage() % 100 != 1);
            blackBoard.SetBool(BBKEY_MODE_START, false);
            blackBoard.SetBool(BBKEY_MODE_PLAY, false);
            blackBoard.SetDateTime(BBKEY_MONSTER_NEXT_SPAWN_AT, DateTime.MinValue);
            blackBoard.SetFloat(BBKEY_AI_DELAY, Main.Instance.time.realtimeSinceStartup + 0.1f, true);
        }

        private void Handle_MODE_FAIL(object[] args)
        {
            blackBoard.SetBool(BBKEY_SET_NEW_GOUNRD, false);
            blackBoard.SetBool(BBKEY_MODE_START, false);
            blackBoard.SetBool(BBKEY_MODE_PLAY, false);
            blackBoard.SetDateTime(BBKEY_MONSTER_NEXT_SPAWN_AT, DateTime.MinValue);
            blackBoard.SetFloat(BBKEY_AI_DELAY, Main.Instance.time.realtimeSinceStartup + 2f, true);
        }

        private int AI_CheckSuccess()
        {
            var monsterCount = mode.core.enemy.GetAliveEnemyCount();
            var totalSpawnMonster = mode.core.enemy.GetTotalSpawnEnemyCount();
            var spawnMax = mode.core.enemy.GetMaxSpawnCount();

            if (totalSpawnMonster < spawnMax || monsterCount > 0)
            {
                return Node.Status.FAILURE;
            }

            return Node.Status.SUCCESS;
        }

        protected virtual void AI_Success()
        {
            VirtualServer.Send(Packet.SUCCESS_STAGE,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out SUCCESS_STAGE tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MODE, tArg.tmode);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MISSION, tArg.tmissions);
                    GameEvent.Instance.AddEvent(GameEventType.GET_MODE_REWARD, tArg.getInfos, tArg.addGold);
                    GameEvent.Instance.AddEvent(GameEventType.MODE_SUCCESS);
                });
        }

        protected int AI_CheckFail()
        {
            if (mode.core.ally.IsMainUnitAlive())
            {
                return Node.Status.FAILURE;
            }

            return Node.Status.SUCCESS;
        }

        protected virtual void AI_Fail()
        {
            VirtualServer.Send(Packet.FAIL_STAGE,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out FAIL_STAGE tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MODE, tArg.tmode);
                    GameEvent.Instance.AddEvent(GameEventType.MODE_FAIL);
                });
        }

        protected override BehaviorTree CreateAI()
        {
            return
                new BehaviorTree(
                    new Selector(
                        new AIResultAction(AI_IsPause),
                        new Sequence(
                            // (init)
                            new AIAction(AI_DropAndNewGround),
                            new AIResultAction(AI_ModeStartable),
                            new AIAction(AI_ModeStart),
                            new AIResultAction(AI_CheckPlayable),
                            new AIAction(AI_ModePlay),
                            // (spawn monster)
                            new Succeeder(
                                new Sequence(
                                    new AIResultAction(AI_CheckMonsterSpawnable),
                                    new AIAction(AI_SpawnMosnter)
                                    )
                                ),
                            new Selector(
                                // (success)
                                new Sequence(
                                    new AIResultAction(AI_CheckSuccess),
                                    new AIAction(AI_Success)
                                    ),
                                // (fail)
                                new Sequence(
                                    new AIResultAction(AI_CheckFail),
                                    new AIAction(AI_Fail)
                                    )
                                )
                            )
                        )
                    );
        }
    }
}