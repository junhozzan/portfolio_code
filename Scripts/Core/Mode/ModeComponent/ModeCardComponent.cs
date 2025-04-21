using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace ModeComponent
{
    public class ModeCardComponent : ModeBaseComponent
    {
        public ModeCardComponent(Mode mode) : base(mode)
        {

        }

        public long GetAllCardCount()
        {
            var tmode = MyPlayer.Instance.core.mode.GetMode(mode.resMode.id);
            return mode.core.profile.GetModeLevel() + tmode.GetAddCardCount();
        }

        public int GetCardLevelByID(int cardID)
        {
            var tcard = MyPlayer.Instance.core.card.GetCard(cardID);
            return tcard.GetLevel();
        }

        public long ReceivableCardCount()
        {
            var totalCount = 0;
            foreach (var cardID in mode.resMode.modeCardIDs)
            {
                totalCount += MyPlayer.Instance.core.card.GetCard(cardID).count;
            }

            return GetAllCardCount() - totalCount;
        }

        public ICollection<TCard> GetCardInfos()
        {
            var tmode = MyPlayer.Instance.core.mode.GetMode(mode.resMode.id);
            return mode.resMode.modeCardIDs
                .Select(cardID => MyPlayer.Instance.core.card.GetCard(cardID))
                .ToArray();
        }
    }
}