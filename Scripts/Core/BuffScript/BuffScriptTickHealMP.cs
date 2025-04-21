
namespace BuffScript
{
    public class BuffScriptTickHealMP : BuffScriptBase
    {
        private ResourceBuffScriptTickHealMP resBuffScript = null;
        private readonly Tick tick = new Tick();

        public static BuffScriptTickHealMP Of()
        {
            return new BuffScriptTickHealMP();
        }

        public override void Initialize()
        {
            tick.SetCall(Heal);
        }
        public override void DoReset()
        {
            base.DoReset();
            tick.DoReset();
        }
        public override void SetResource(ResourceBuffScriptBase resBuffScript)
        {
            base.SetResource(resBuffScript);
            this.resBuffScript = resBuffScript as ResourceBuffScriptTickHealMP;
            tick.SetResource(this.resBuffScript.startDelay, this.resBuffScript.interval);
        }

        public override void On()
        {
            tick.On();
        }

        public override void UpdateDt(float dt)
        {
            tick.UpdateDt(dt);
        }

        private void Heal()
        {
            buff.owner.core.mana.AddMp(buff.owner.core.stat.GetLongValue(eAbility.RECOVERY_MP));
        }
    }
}