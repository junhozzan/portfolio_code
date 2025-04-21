using UnityEngine;

namespace UIModeCard
{
    public class CpUI_ModeCardSelectProfiler : UIBase
    {
        [SerializeField] UIText textOrigin = null;
        [SerializeField] Transform textParent = null;

        private ObjectPool<UIText> textPool = null;

        public void Init()
        {
            textPool = ObjectPool<UIText>.Of(textOrigin, textParent);
        }

        public void Refresh()
        {
            textPool.Clear();

            //ShowCardInfo();
            ShowStatNormal();
            ShowStatSpecial();
        }

        private void ShowCardInfo()
        {
            var mode = ModeManager.Instance.mode;
            foreach (var cardID in mode.core.profile.resMode.modeCardIDs)
            {
                var card = MyPlayer.Instance.core.card.GetCard(cardID);
                if (card.GetLevel() == 0)
                {
                    continue;
                }

                var resCard = ResourceManager.Instance.mode.GetModeCard(cardID);
                if (resCard == null)
                {
                    continue;
                }
            }
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