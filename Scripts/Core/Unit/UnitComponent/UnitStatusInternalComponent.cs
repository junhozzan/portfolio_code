namespace UnitStatusInternal
{
    public class UnitStatusInternalComponent
    {
        protected readonly Unit owner;
        protected readonly ResourceStatus resStatus;

        private bool isActivate = false;

        public static UnitStatusInternalComponent Of(Unit owner, StatusType type)
        {
            return new UnitStatusInternalComponent(owner, type);
        }

        protected UnitStatusInternalComponent(Unit owner, StatusType type)
        {
            this.owner = owner;
            this.resStatus = ResourceManager.Instance.status.GetStatus(Util.EnumToInt(type));
        }

        public virtual void DoReset()
        {
            isActivate = false;
        }

        public virtual void Refresh(UnitBuff buff)
        {
            isActivate = buff != null;
        }

        public virtual bool IsActivate()
        {
            return isActivate;
        }

        public virtual float Compare(UnitBuff buff)
        {
            return buff.GetUntilAt();
        }
    }
}
