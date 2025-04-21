
namespace UnitComponent
{
    public class UnitDeadComponent : UnitBaseComponent
    {
        public bool isDead { get; private set; } = false;

        public UnitDeadComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            isDead = false;
        }


        public void Resurrection()
        {
            isDead = false;
        }

        public void CheckDead(Unit from)
        {
            if (isDead)
            {
                return;
            }

            isDead = true;

            from.core.kill.Add(owner);
            owner.core.killed.Add(from);

            Dead();
            DeadEnd();
        }

        protected virtual void Dead()
        {
            owner.core.buff.HandleDead();
            owner.core.anim.Dead();
            owner.core.move.Stop();
            owner.core.jump.Stop();

            PlayDeathSound();
            GameEvent.Instance.AddEvent(GameEventType.DEAD_UNIT, owner);
        }

        private void PlayDeathSound()
        {
            if (string.IsNullOrEmpty(owner.core.profile.tunit.resUnit.deathSound))
            {
                return;
            }

            SoundManager.Instance.PlaySfx(owner.core.profile.tunit.resUnit.deathSound);
        }

        protected virtual void DeadEnd()
        {

        }
    }
}