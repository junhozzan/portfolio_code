using System.Collections.ObjectModel;
using System.Linq;
using UnitStatusInternal;

namespace UnitComponent
{
    public class UnitStatusComponent : UnitBaseComponent
    {
        private readonly ReadOnlyDictionary<StatusType, UnitStatusInternalComponent> unitStatus;

        private static ReadOnlyCollection<StatusType> cachedStatusTypes = Util.GetEnumArray<StatusType>()
            .Where(x => x != StatusType.NONE)
            .ReadOnly();

        public UnitStatusComponent(Unit owner) : base(owner)
        {
            this.unitStatus = cachedStatusTypes
                .ToDictionary(x => x, x => ToInternalComponent(owner, x))
                .ReadOnly();
        }

        protected virtual UnitStatusInternalComponent ToInternalComponent(Unit owner, StatusType type)
        {
            switch (type)
            {
                case StatusType.FREEZING: return UnitStatusFreezingComponent.Of(owner, type);
            }

            return UnitStatusInternalComponent.Of(owner, type);
        }

        public override void DoReset()
        {
            base.DoReset();
            foreach (var v in unitStatus.Values)
            {
                v.DoReset();
            }
        }

        public bool IsInvincible()
        {
            if (IsActivate(StatusType.INVINCIBLE))
            {
                return true;
            }

            return false;
        }

        public bool IsMovable()
        {
            return true;
        }

        public bool IsSkillUsable()
        {
            return true;
        }

        private UnitStatusInternalComponent GetStatus(StatusType type)
        {
            return unitStatus.TryGetValue(type, out var status) ? status : null;
        }

        private bool IsActivate(StatusType type)
        {
            var status = GetStatus(type);
            if (status == null)
            {
                return false;
            }

            return status.IsActivate();
        }

        public void Refresh(StatusType type)
        {
            if (!unitStatus.TryGetValue(type, out var status))
            {
                return;
            }

            var buff = owner.core.buff.buffScript.GetBuffsByScriptType(BuffScriptType.STATUS)
                .OrderByDescending(status.Compare)
                .FirstOrDefault();

            status.Refresh(buff);
        }
    }
}