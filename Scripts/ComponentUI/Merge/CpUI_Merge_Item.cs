using UnityEngine;
using DG.Tweening;

namespace UIMerge
{
    public class CpUI_Merge_Item : UIBase
    {
        [SerializeField] UIImage image = null;
        [SerializeField] UIImage effect = null;

        public long uid { get; private set; } = 0;
        public ResourceMerge resMerge { get; private set; } = null;
        public int locate { get; private set; } = 0;

        public void Set(ResourceMerge resMerge)
        {
            effect.gameObject.SetActive(false);

            this.uid = GetUID();
            this.resMerge = resMerge;
            image.SetSprite(resMerge.atlas, resMerge.sprite);
            image.SetNativeSize();
        }

        public void ShowMergeEffect()
        {
            effect.gameObject.SetActive(true);
            effect.SetSprite(resMerge.atlas, resMerge.sprite);
            effect.SetNativeSize();

            var size = Vector3.one * 1.5f;
            DOTween.To(
                null,
                t =>
                {
                    effect.transform.localScale = Vector3.Lerp(Vector3.one, size, t);
                    effect.SetAlpha((1f - t) * image.GetAlpha());
                },
                1f,
                0.3f)
                .From(0f);
        }

        public void DragStart()
        {
            transform.SetAsLastSibling();
        }

        public void Drag(Vector2 size, Vector2 position)
        {
            transform.localPosition = position;
            this.locate = Util.ConvertRateToLocate(position.x / size.x, position.y / size.y);
        }

        public void SetLocate(Vector2 size, int locate)
        {
            this.locate = locate;
            transform.localPosition = Util.ConvertLocateToPos(size, locate);
            transform.SetAsLastSibling();
        }

        public void DoCombineAction(Vector2 pos, float time)
        {
            var startPos = transform.localPosition;

            DOTween.To(
                null,
                t =>
                {
                    transform.localPosition = Vector2.Lerp(startPos, pos, t);
                },
                1f,
                time)
                .SetEase(Ease.InExpo)
                .From(0f)
                .OnComplete(() => gameObject.SetActive(false));
        }

        public bool Overlap(CpUI_Merge_Item other)
        {
            var rect = image.GetImageRect().rect;
            rect.center = transform.localPosition;

            var otRect = other.image.GetImageRect().rect;
            otRect.center = other.transform.localPosition;
            
            return rect.Overlaps(otRect);
        }

        public void SetFade(bool b)
        {
            image.SetAlpha(b ? 0.3f : 1f);
        }

        private static long increaseUID = 0;
        private static long GetUID()
        {
            if (increaseUID >= long.MaxValue)
            {
                increaseUID = 0;
            }

            return ++increaseUID;
        }
    }
}