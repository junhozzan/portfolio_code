using System.Collections;
using UnityEngine;

namespace ModeComponent
{
    public class ModeSavedComponent : ModeBaseComponent
    {
        public MyUnit myUnit = null;
        public Vector3 cameraFollowPosition = Vector3.zero;
        public Vector3 cameraPosition = Vector3.zero;

        public ModeSavedComponent(Mode mode) : base(mode)
        {

        }

        public void Set(Mode mode)
        {
            myUnit = mode.core.ally.myUnit;

            cameraFollowPosition = mode.core.camera.followCameraPos;
            cameraPosition = mode.core.camera.camera.GetPosition();
        }
    }
}