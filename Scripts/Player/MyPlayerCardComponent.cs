using System.Collections.Generic;

namespace MyPlayerComponent
{
    public class MyPlayerCardComponent : MyPlayerBaseComponent
    {
        private readonly Dictionary<int, TCard> cards = new Dictionary<int, TCard>();

        public MyPlayerCardComponent(MyPlayer mp) : base(mp)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.GET_CARD_DATAS, Handle_GET_CARD_DATAS)
                .Add(GameEventType.UPDATE_CARD, Handle_UPDATE_CARD)
                ;
        }

        private void Handle_GET_CARD_DATAS(object[] args)
        {
            var tArg = GameEvent.GetSafe<GET_CARD_DATAS>(args, 0);
            if (tArg == null)
            {
                return;
            }

            cards.Clear();
            UpdateCards(tArg.tcards);
        }

        private void Handle_UPDATE_CARD(object[] args)
        {
            var tcards = GameEvent.GetSafe<ICollection<TCard>>(args, 0);
            if (tcards == null)
            {
                return;
            }

            UpdateCards(tcards);
        }

        private void UpdateCard(TCard newCard)
        {
            if (cards.TryGetValue(newCard.resID, out var card))
            {
                card.OnDisable();
            }
            else
            {
                cards.Add(newCard.resID, null);
            }

            cards[newCard.resID] = newCard;
        }

        private void UpdateCards(IEnumerable<TCard> cards)
        {
            foreach (var tcard in cards)
            {
                UpdateCard(tcard);
            }
        }

        public TCard GetCard(int id)
        {
            if (!cards.TryGetValue(id, out var v))
            {
                cards.Add(id, v = TManager.Instance.Get<TCard>().SetResID(id));
            }

            return v;
        }
    }
}