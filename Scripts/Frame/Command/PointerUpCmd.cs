using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace CCommand
{
    public class PointerUpCmd : MonoBehaviour, IPointerCmd, IPointerUpHandler
    {
        public Action _onEvent { get; set; } = null;
        public bool _bUse { get; set; } = true;
        public PointerEventData _pointerEvent { get; set; } = null;

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_bUse)
            {
                return;
            }

            if (_onEvent != null)
            {
                _onEvent();
            }
        }
    }
}
