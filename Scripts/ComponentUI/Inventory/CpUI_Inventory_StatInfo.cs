using UnityEngine;

namespace UIInventory
{
    public class CpUI_Inventory_StatInfo : UIBase
    {
        [SerializeField] RectTransform rect = null;
        [SerializeField] UIText textOrigin = null;

        private ObjectPool<UIText> textPool = null;

        public void Init()
        {
            textPool = ObjectPool<UIText>.Of(textOrigin, rect);
        }

        public void On()
        {
            Refresh();
        }

        public void Refresh()
        {
            textPool.Clear();

            ShowStatNormal();
            ShowStatSpecial();
        }

        private void ShowStatNormal()
        {
            var infos = ShowStat.GetShowStats(MyUnit.Instance);
            if (infos.Count > 0)
            {
                foreach (var info in infos)
                {
                    SetText(info.Item1, info.Item2);
                }
            }
        }

        private void ShowStatSpecial()
        {
            var infos = ShowStat.GetShowSpecialAbilities(MyUnit.Instance);
            if (infos.Count > 0)
            {
                SetText(string.Empty, Color.white);
                foreach (var info in infos)
                {
                    SetText(info.Item1, info.Item2);
                }
            }
        }

        public void SetText(string str, Color color)
        {
            var text = textPool.Pop();
            text.gameObject.SetActive(true);
            text.transform.SetAsLastSibling();
            text.SetText(str);
            text.SetTextColor(color);
        }
    }
}