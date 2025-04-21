using behaviorTree;

namespace ModeComponent
{
    public class DamageModeAIComponent : ModeAIComponent
    {
        private readonly new DamageMode mode = null;

        public DamageModeAIComponent(Mode mode) : base(mode)
        {
            this.mode = mode as DamageMode;
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

        private int AI_CheckSuccess()
        {
            if (!mode.core.time.IsEnd())
            {
                return Node.Status.FAILURE;
            }

            return Node.Status.SUCCESS;
        }

        private void AI_Success()
        {
            VirtualServer.Send(Packet.RECORD_SCORE,
                (arg) =>
                {
                    ModeManager.Instance.Enter(GameData.MODE_DATA.MODE_1_ID);

                    if (!VirtualServer.TryGet(arg, out RECORD_SCORE tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MODE, tArg.tmode);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MISSION, tArg.tmissions);
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
                            // (play)
                            new Sequence(
                                new AIResultAction(AI_CheckPlayable),
                                new Selector(
                                    // (update)
                                    new Sequence(
                                        new Inverter(new AIResultAction(AI_CheckSuccess)),
                                        // (spawn monster)
                                        new Succeeder(
                                            new Sequence(
                                                new AIResultAction(AI_CheckMonsterSpawnable),
                                                new AIAction(AI_SpawnMosnter)
                                                )
                                            )
                                        ),
                                    // (success)
                                    new Sequence(
                                        new AIResultAction(AI_CheckSuccess),
                                        new AIAction(AI_Success)
                                        )
                                    )
                                )
                            )
                        )
                    );
        }
    }
}
