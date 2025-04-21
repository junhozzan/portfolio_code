using UnityEngine;
using UnityEngine.EventSystems;
using System;
namespace CCommand
{
    public class PointerDownCmd : MonoBehaviour, IPointerCmd, IPointerDownHandler
    {
        public Action _onEvent { get; set; } = null;
        public bool _bUse { get; set; } = true;
        public PointerEventData _pointerEvent { get; set; } = null;

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.LogFormat("delta:{0}", eventData.delta);
            //Debug.LogFormat("position:{0}", eventData.position);
            //Debug.LogFormat("dragging:{0}", eventData.dragging);
            //Debug.LogFormat("IsPointerMoving:{0}", eventData.IsPointerMoving());
            //Debug.Log("###############################");
            if (!_bUse)
            {
                return;
            }

            _pointerEvent = eventData;

            if (_onEvent != null)
            {
                _onEvent();
            }
        }
    }
}
