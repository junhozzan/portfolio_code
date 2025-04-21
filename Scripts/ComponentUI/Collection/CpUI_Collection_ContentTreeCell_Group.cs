using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UICollection
{
    public class CpUI_Collection_ContentTreeCell_Group : CpUI_Collection_ContentTreeCell_Base
    {
        [SerializeField] UIText titleText = null;
        [SerializeField] UIText progressText = null;
        [SerializeField] UIImage progressFill = null;
        [SerializeField] UIImage progressShadowFill = null;
        [SerializeField] GameObject arrow = null;

        private ResourceCollectionGroup resCollectionGroup
        {
            get
            {
                return parent.cellInfo.resCollectionGroup;
            }
        }

        private bool isExpand
        {
            get 
            {
                return parent.cellInfo.osaItem.isExpand;
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            RefreshTitleText();
            RefreshProgress();
            RefreshExpand();
        }

        private void RefreshTitleText()
        {
            if (resCollectionGroup == null)
            {
                return;
            }

            if (titleText == null)
            {
                return;
            }

            titleText.SetText(resCollectionGroup.GetName());
            titleText.SetTextColor(resCollectionGroup.nameColor);
        }

        private void RefreshProgress()
        {
            if (resCollectionGroup == null)
            {
                return;
            }

            var completeCount = 0;
            var canEnrollCount = 0;
            var maxCount = resCollectionGroup.collectionIDs.Count;
            foreach (var resCollectionID in resCollectionGroup.collectionIDs)
            {
                var resCollection = ResourceManager.Instance.collection.GetCollection(resCollectionID);
                if (resCollection == null)
                {
                    continue;
                }

                var collection = MyPlayer.Instance.core.collection.GetCollection(resCollection.id);
                if (collection.IsComplete())
                {
                    ++completeCount;
                }
                else if (MyPlayer.Instance.core.collection.CanEnrollData(resCollection.id))
                {
                    ++canEnrollCount;
                }
            }

            if (progressText != null)
            {
                progressText.SetText($"{completeCount}/{maxCount}");
            }

            if (progressFill != null)
            {
                progressFill.SetFillAmount(completeCount / (float)maxCount);
            }

            if (progressShadowFill != null)
            {
                progressShadowFill.SetFillAmount((completeCount + canEnrollCount) / (float)maxCount);
            }
        }

        private void Update()
        {
            if (progressShadowFill != null)
            {
                var v = Mathf.Abs((Time.realtimeSinceStartup % 2) - 1f);
                progressShadowFill.SetAlpha(Mathf.Max(1.4f - v, 0.4f));
            }
        }
        private void RefreshExpand()
        {
            if (arrow == null)
            {
                return;
            }

            var rot = arrow.transform.rotation;
            rot.z = isExpand ? 0 : 180;

            arrow.transform.rotation = rot;
        }

        public override bool IsSelectable()
        {
            return true;
        }

        public override float GetSize()
        {
            return rectTransform.rect.size.y;
        }
    }
}