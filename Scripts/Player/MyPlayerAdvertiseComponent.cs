using System;
using System.Collections.Generic;
using System.Linq;

namespace MyPlayerComponent
{
    public class MyPlayerAdvertiseComponent : MyPlayerBaseComponent
    {
        private readonly Dictionary<int, TAdvertisement> advertisements = new Dictionary<int, TAdvertisement>();

        public MyPlayerAdvertiseComponent(MyPlayer mp) : base(mp)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.GET_ADVERTISEMENT_DATAS, Handle_GET_ADVERTISEMENT_DATAS)
                .Add(GameEventType.UPDATE_AD, Handle_UPDATE_AD)
                ;
        }

        private void Handle_GET_ADVERTISEMENT_DATAS(object[] args)
        {
            var tArg = GameEvent.GetSafe<GET_ADVERTISEMENT_DATAS>(args, 0);
            if (tArg == null)
            {
                return;
            }
            
            Clear();
            UpdateAds(tArg.tadvertisements);
        }

        private void Handle_UPDATE_AD(object[] args)
        {
            var tadvertisement = GameEvent.GetSafe<TAdvertisement>(args, 0);
            if (tadvertisement == null)
            {
                return;
            }

            UpdateAd(tadvertisement);
        }

        private void Clear()
        {
            advertisements.Clear();
        }

        private void UpdateAd(TAdvertisement tad)
        {
            if (!advertisements.TryGetValue(tad.resID, out var v))
            {
                advertisements.Add(tad.resID, null);
            }

            if (v != null)
            {
                v.OnDisable();
            }

            advertisements[tad.resID] = tad;
        }

        private void UpdateAds(IEnumerable<TAdvertisement> ads)
        {
            foreach (var tad in ads)
            {
                UpdateAd(tad);
            }
        }

        public TAdvertisement GetAd(int id)
        {
            if (!advertisements.TryGetValue(id, out var v))
            {
                advertisements.Add(id, v = TManager.Instance.Get<TAdvertisement>().SetResID(id));
            }

            return v;
        }
    }
}