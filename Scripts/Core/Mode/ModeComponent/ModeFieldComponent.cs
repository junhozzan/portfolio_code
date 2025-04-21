using UnityEngine;

namespace ModeComponent
{
    public class ModeFieldComponent : ModeBaseComponent
    {
        public ModeFieldComponent(Mode mode) : base(mode)
        {

        }

        public virtual bool IsLoading()
        {
            return false;
        }

        public virtual Vector3 GetRandomGroundPosition()
        {
            return Vector3.zero;
        }
    }
}