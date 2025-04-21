using System.Collections.Generic;
using System;

namespace ModeComponent
{
    public class ModeUICardComponent : ModeBaseComponent
    {
        private readonly List<ResourceModeCard> tempPickableCards = new List<ResourceModeCard>();
        private readonly List<ResourceModeCard> tempReturnCards = new List<ResourceModeCard>();

        public ModeUICardComponent(Mode mode) : base(mode)
        {

        }

        public void OpenCardSelect()
        {
            var cards = GetRandom3ModeCards();
            if (cards.Count == 0)
            {
                return;
            }

            UIModeCard.CpUI_ModeCardSelect.Instance.On(mode, cards);
        }

        public bool IsOpenedCardSelect()
        {
            return UIModeCard.CpUI_ModeCardSelect.Instance.IsActive();
        }

        private List<ResourceModeCard> GetRandom3ModeCards()
        {
            var cardInfos = mode.core.card.GetCardInfos();

            tempPickableCards.Clear();
            tempReturnCards.Clear();

            foreach (var cardID in mode.resMode.modeCardIDs)
            {
                var resModeCard = ResourceManager.Instance.mode.GetModeCard(cardID);
                if (resModeCard == null)
                {
                    continue;
                }

                var card = MyPlayer.Instance.core.card.GetCard(resModeCard.id);
                var level = card.GetLevel();
                if (level >= resModeCard.maxCount)
                {
                    continue;
                }

                if (!resModeCard.IsValid(level, cardInfos))
                {
                    continue;
                }

                tempPickableCards.Add(resModeCard);
            }

            // 아이템 최대 갯수 (최대 3)
            var returnCount = Math.Min(3, tempPickableCards.Count);
            for (int i = 0; i < returnCount; ++i)
            {
                // 등장값의 합
                var totalAppear = 0;
                for (int k = 0, kCnt = tempPickableCards.Count; k < kCnt; ++k)
                {
                    totalAppear += tempPickableCards[k].appear;
                }

                var pick = UnityEngine.Random.Range(0, totalAppear);
                var check = 0;
                foreach (var card in tempPickableCards)
                {
                    check += card.appear;
                    if (pick >= check)
                    {
                        continue;
                    }

                    tempReturnCards.Add(card);
                    tempPickableCards.Remove(card);
                    break;
                }
            }

            return tempReturnCards;
        }
    }
}