using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyPlayerComponent
{
    public class MyPlayerModeRankComponent : MyPlayerBaseComponent
    {
        private readonly Dictionary<int, RankInfo> modeRanks = new Dictionary<int, RankInfo>();

        public MyPlayerModeRankComponent(MyPlayer mp) : base(mp)
        {

        }

        public Coroutine RequestRank(int modeID, bool register)
        {
            var url = GetGGSheetURL(modeID);
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            var resMode = ResourceManager.Instance.mode.GetMode(modeID);
            if (resMode == null)
            {
                return null;
            }

            var rankInfo = GetRankInfo(modeID);
            if (rankInfo == null)
            {
                return null;
            }

            rankInfo.SetRequest(true);

            (string, object)[] p = null;
            if (register)
            {
                var mode = mp.core.mode.GetMode(resMode.id);
                p = new (string, object)[]
                {
                    ("reg", 1),
                    ("id", mp.core.profile.info.serial),
                    ("nick", mp.core.profile.info.nickName),
                    ("level", ResourceManager.Instance.level.GetExpToLevel(mode.GetExp())),
                    ("record", mode.GetBestScore()),
                    ("equiped", GetAppearEquipedItem()),
                    ("info", GetInfo()),
                };
            }
            else
            {
                p = new (string, object)[]
                {
                    ("reg", 0),
                    ("id", mp.core.profile.info.serial),
                };
            }

            return GGSheetManager.Instance.Post(url, (result, node) => ResponseRank(modeID, result, node), p);
        }

        private void ResponseRank(int modeID, GGSheetManager.NetworkState result, JSONNode node)
        {
            var rankInfo = GetRankInfo(modeID);
            if (rankInfo == null)
            {
                return;
            }

            rankInfo.SetRequest(false);

            switch (result)
            {
                case GGSheetManager.NetworkState.SUCCESS:
                    rankInfo.SetTop50(modeID, JSONCtrl.GetSafeNode(node, "top50"));
                    rankInfo.SetMyRank(modeID, JSONCtrl.GetSafeNode(node, "my"));

                    GameEvent.Instance.AddEvent(GameEventType.RESPONSE_RANK, true);
                    break;

                case GGSheetManager.NetworkState.FAIL:
                    GameEvent.Instance.AddEvent(GameEventType.RESPONSE_RANK, false);
                    break;
            }
        }

        public RankInfo GetRankInfo(int modeID)
        {
            var resMode = ResourceManager.Instance.mode.GetMode(modeID);
            if (resMode == null)
            {
                return null;
            }

            if (!modeRanks.TryGetValue(resMode.id, out var v))
            {
                modeRanks.Add(resMode.id, v = new RankInfo());
            }

            return v;
        }

        public List<int> GetTopRankerItemIDs(int modeID)
        {
            var rankInfo = GetRankInfo(modeID);
            if (rankInfo.top50.Count == 0)
            {
                return null;
            }

            return rankInfo.top50[0].equipedItems;
        }

        private string GetGGSheetURL(int modeID)
        {
            var ggurl = GameData.GGSHEET_URL.GetGGURL(mp.core.profile.info.createVersion);

            if (modeID == GameData.MODE_DATA.MODE_1_ID)
            {
                return ggurl.mode_1;
            }
            //else if (modeID == GameData.MODE_DATA.MODE_2_ID)
            //{
            //    return ggurl.mode_2;
            //}
            //else if (modeID == GameData.MODE_DATA.MODE_3_ID)
            //{
            //    return ggurl.mode_3;
            //}

            return string.Empty;
        }

        private string GetAppearEquipedItem()
        {
            var itemIDs = GetOptimizationItemIDs(mp.core.inventory.GetEquipedItemIDs());
            return string.Join("#", itemIDs);
        }

        private string GetInfo()
        {
            var createVersion = mp.core.profile.info.createVersion;
            var appVersion = Application.version;
            var lastLink = mp.core.profile.info.lastLink.ToString("yyMMddHHmm");
            var playTime = User.Instance.info.playTime.ToString("0");
            var sysLan = Application.systemLanguage;
            var ad = User.Instance.achieve.GetValue(AchieveType.SHOW_AD_COMPLETE);

            // 순서 고정 csv형태로
            return $"{createVersion}#{appVersion}#{lastLink}#{playTime}#{sysLan}#{ad}";
        }

        private ICollection<int> GetOptimizationItemIDs(ICollection<int> itemIDs)
        {
            var items = itemIDs.Select(x => ResourceManager.Instance.item.GetItem(x)).Where(x => x.GetSpumID() > 0);
            var avatars = items.Where(x => x.IsAvatar()).GroupBy(x => x.itemType).ToDictionary(x => x.Key, x => x.Last());
            var equips = items.Where(x => !x.IsAvatar()).GroupBy(x => x.itemType).ToDictionary(x => x.Key, x => x.Last());

            var result = avatars.Values.Select(x => x.id).ToHashSet();
            foreach (var item in equips.Values)
            {
                result.Add(avatars.TryGetValue(item.itemType, out var v) ? v.id : item.id);
            }

            return result;
        }
    }

    public class RankInfo
    {
        // 최초 1회 요청 유무 확인 변수
        public int requestCount { get; private set; } = 0;
        public bool isRequest { get; private set; } = false;

        public List<Rank> top50 { get; private set; } = new List<Rank>();
        public Rank myRank { get; private set; } = null;

        public void SetRequest(bool request)
        {
            isRequest = request;
            if (request)
            {
                ++requestCount;
            }
        }

        public void SetTop50(int modeID, JSONNode node)
        {
            top50.Clear();
            for (int i = 0; i < node.Count; ++i)
            {
                top50.Add(new Rank(modeID, node[i]));
            }
        }

        public void SetMyRank(int modeID, JSONNode node)
        {
            myRank = new Rank(modeID, node);
        }
    }

    public class Rank
    {
        public readonly int modeID = 0;
        public readonly int rank = 0;
        public readonly string user = string.Empty;
        public readonly string name = string.Empty;
        public readonly long record = -1;
        public readonly long level = -1;
        public readonly List<int> equipedItems = null;

        public Rank(int modeID, JSONNode node)
        {
            this.modeID = modeID;
            rank = JSONCtrl.GetInt(node, "rk", rank);
            user = JSONCtrl.GetString(node, "us", user);
            name = JSONCtrl.GetString(node, "ne", name);
            record = JSONCtrl.GetLong(node, "rd", record);
            level = JSONCtrl.GetLong(node, "lv", level);
            equipedItems = JSONCtrl.GetString(node, "eq", string.Empty)
                .Split("#")
                .Select(x => int.TryParse(x, out var v) ? v : -1)
                .ToList();
        }
    }
}