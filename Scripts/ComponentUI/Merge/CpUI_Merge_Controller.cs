using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIMerge
{
    public class CpUI_Merge_Controller : UIBase, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private CpUI_Merge_Item controllItem = null;
        private long controllUID = 0;
        private Vector2 offset = Vector2.zero;
        private Action onDragStart = null;
        private Action onDragEnd = null;

        public void Init(Action onDragStart, Action onDragEnd)
        {
            this.onDragStart = onDragStart;
            this.onDragEnd = onDragEnd;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var go = eventData.pointerCurrentRaycast.gameObject;
            if (go == null || go.transform.parent == null)
            {
                return;
            }

            if (!go.transform.parent.TryGetComponent(out CpUI_Merge_Item item))
            {
                return;
            }

            controllItem = item;
            controllItem.DragStart();
            controllUID = item.uid;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 pos))
            {
                offset = pos - (Vector2)controllItem.transform.localPosition;
            }
            else
            {
                offset = Vector2.zero;
            }

            onDragStart?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (controllItem == null)
            {
                return;
            }

            if (controllItem.uid != controllUID)
            {
                controllItem = null;
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 pos))
            {
                return;
            }

            var viewSize = GetSize();

            pos -= offset;
            pos.x = Mathf.Clamp(pos.x, 0f, viewSize.x);
            pos.y = Mathf.Clamp(pos.y, 0f, viewSize.y);

            controllItem.Drag(viewSize, pos);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (controllItem == null)
            {
                return;
            }

            onDragEnd?.Invoke();
            controllItem = null;
        }

        public Vector2 GetSize()
        {
            return rectTransform.rect.size;
        }

        public Vector2 GetCenterPosition()
        {
            return rectTransform.rect.size * 0.5f;
        }

        public bool IsControllItem(long uid)
        {
            return controllUID == uid;
        }

        public CpUI_Merge_Item GetControllItem()
        {
            return controllItem;
        }
    }
}