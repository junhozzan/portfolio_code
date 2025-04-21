using System.Collections.Generic;

namespace UnitComponent
{
    public abstract class UnitBaseComponent : XComponent
    {
        protected readonly Unit owner = null;

        public UnitBaseComponent(Unit owner)
        {
            this.owner = owner;
        }
    }
}