using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UICollection
{
    public class CpUI_Collection_ContentTreeCell_Base : UIBase
    {
        protected CpUI_Collection_ContentTreeCell parent = null;

        public virtual void Init(CpUI_Collection_ContentTreeCell parent)
        {
            this.parent = parent;
        }

        public virtual void Refresh()
        {
            gameObject.SetActive(true);
        }

        public virtual float GetSize()
        {
            return rectTransform.rect.size.y;
        }

        public virtual bool IsSelectable()
        {
            return false;
        }
    }
}
