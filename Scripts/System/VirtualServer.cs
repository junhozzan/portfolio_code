using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;
using System.Linq;

public class VirtualServer
{
    private static DateTime nowDate => Main.Instance.time.now;
    private static long nowToEpochSecond => Main.Instance.time.nowToEpochSecond();

    private struct Params
    {
        private object[] args;
        public Action<object> callback;

        public Params(object[] args, Action<object> callback)
        {
            this.args = args;
            this.callback = callback;
        }

        public Type GetType(int index)
        {
            if (index < 0 || index >= args.Length)
            {
                return null;
            }

            return args[index] as Type;
        }

        public List<T> GetList<T>(int index)
        {
            if (index < 0 || index >= args.Length)
            {
                return null;
            }

            return args[index] as List<T>;
        }

        public int GetInt(int index, int _default = 0)
        {
            if (index < 0 || index >= args.Length)
            {
                return _default;
            }

            return Convert.ToInt32(args[index]);
        }

        public long GetLong(int index, long _default = 0L)
        {
            if (index < 0 || index >= args.Length)
            {
                return _default;
            }

            return Convert.ToInt64(args[index]);
        }

        public string GetString(int index, string _default = "")
        {
            if (index < 0 || index >= args.Length)
            {
                return _default;
            }

            return Convert.ToString(args[index]);
        }
    }

    private static ReadOnlyDictionary<Packet, Action<Params>> events =
        new Dictionary<Packet, Action<Params>>()
        {
            [Packet.LOGIN] = Login,
            [Packet.TIME_OF_DAY] = TimeOfDay,
            [Packet.CREATE_NICKNAME] = CreateNickName,
            [Packet.GET_USER_INFO] = GetUserInfo,
            [Packet.GET_MODE_DATAS] = GetModeDatas,
            [Packet.GET_ITEM_DATAS] = GetItemDatas,
            [Packet.GET_MISSION_DATAS] = GetMissionDatas,
            [Packet.GET_ATTENDANCE_DATAS] = GetAttendanceDatas,
            [Packet.GET_LAB_DATAS] = GetLabDatas,
            [Packet.GET_COLLECTION_DATAS] = GetCollectionDatas,
            [Packet.GET_STORE_DATAS] = GetStoreDatas,
            [Packet.GET_ADVERTISEMENT_DATAS] = GetAdvertisement,
            [Packet.GET_INVENTORY_INFO] = GetInventoryInfo,
            [Packet.GET_CARD_DATAS] = GetCardDatas,
            [Packet.GET_MERGE_DATAS] = GetMergeDatas,

            [Packet.SUCCESS_STAGE] = SuccessStage,
            [Packet.FAIL_STAGE] = FailStage,

            [Packet.SHOW_AD] = ShowAd,
            [Packet.EQUIP_ITEM] = EquipItem,
            [Packet.RELEASE_ITEM] = ReleaseItem,
            [Packet.REROLL_ITEM] = RerollItem,
            [Packet.ENHANCE_ITEM] = EnhanceItem,
            [Packet.ENHANCE_ITEM_ALL] = EnhanceItemAll,
            [Packet.AWAKEN_ITEM] = AwakenItem,
            [Packet.DISMANTLE_ITEM] = DismantleItem,
            [Packet.UPGRADE_LAB] = UpgradeLab,
            [Packet.PURCHASE_STORE_ITEM] = PurchaseStoreItem,
            [Packet.CLEAR_MISSION] = ClearMission,
            [Packet.CLEAR_ATTENDANCE] = ClearAttendance,
            [Packet.ENROLL_COLLECTION] = EnrollCollection,
            [Packet.RECORD_SCORE] = RecordScore,
            [Packet.RESET_STAGE_MODE] = ResetStageMode,

            [Packet.SELECT_MODE_CARD] = SelectModeCard,
            [Packet.CREATE_MERGE_NEW] = CreateMergeNew,
            [Packet.CREATE_MERGE_NEXT] = CreateMergeNext,

            [Packet.COMPLETE_GUIDE] = CompleteGuide,
        }
        .ReadOnly();

    public static void Send(Packet type, Action<object> callback, params object[] args)
    {
        if (!events.TryGetValue(type, out var func))
        {
            return;
        }

        func.Invoke(new Params(args, callback));
    }

    private static void CreateNickName(Params _params)
    {
        var uInfo = User.Instance.info;

        var nickName = _params.GetString(0);
        uInfo.SetNickName(nickName);

        PlatformManager.Instance.Save(uInfo);

        var arg = CREATE_NICKNAME.Of(TManager.Instance.Get<TUserInfo>().SetInfo(uInfo));
        _params.callback?.Invoke(arg);
    }

    private static void Login(Params _params)
    {
        CreateFirst();
        //ModeRetrunStage1();
        UpdateDayOffset();

        _params.callback?.Invoke(null);
    }

    private static void CreateFirst()
    {
        var uInfo = User.Instance.info;
        if (!string.IsNullOrEmpty(uInfo.serial))
        {
            return;
        }

        uInfo.SetCreateVersion(Application.version.ToDECIMAL());
        uInfo.SetSerial(Guid.NewGuid().ToString().Replace("-", "") + nowDate.ToString("yyMMddHHmmss"));

        PlatformManager.Instance.Save(uInfo);
    }

    //private static void ModeRetrunStage1()
    //{
    //    var resStageMode = ResourceManager.Instance.mode.GetMode(GameData.MODE_DATA.MODE_1_ID);
    //    if (resStageMode == null)
    //    {
    //        return;
    //    }

    //    var uMode = User.Instance.mode;

    //    var stageModeData = uMode.GetModeData(resStageMode.id);
    //    var startScore = (stageModeData.score / 10) * 10 + 1;
    //    stageModeData.SetScore(startScore);

    //    PlatformManager.Instance.Save(uMode);
    //}

    private static void UpdateDayOffset()
    {
        var uInfo = User.Instance.info;
        var uAchieve = User.Instance.achieve;
        var uMission = User.Instance.mission;
        var uStore = User.Instance.store;
        var uAttendance = User.Instance.attendance;

        var now = nowDate;
        var lastLink = uInfo.lastLink;
        var dayOffset = Util.GetDayOffset(now, lastLink);

        if (dayOffset > 0)
        {
            var isWeeklyReset = false;
            if (dayOffset >= 7)
            {
                isWeeklyReset = true;
            }
            else
            {
                for (int i = 1, offset = Math.Min(dayOffset, 7); i <= offset; ++i)
                {
                    var lastLinkDate = lastLink;
                    if (lastLinkDate.AddDays(-i).DayOfWeek != DayOfWeek.Sunday)
                    {
                        continue;
                    }

                    isWeeklyReset = true;
                    break;
                }
            }

            // 임무 초기화
            foreach (var resMission in ResourceManager.Instance.mission.GetMissions())
            {
                if (resMission.resetType == MissionResetType.NONE)
                {
                    continue;
                }

                if (resMission.resetType == MissionResetType.WEEK && !isWeeklyReset)
                {
                    continue;
                }

                var misionData = uMission.GetData(resMission.id);
                var resetValue = misionData.level >= resMission.maxLevel ? 0 : misionData.value;

                misionData.SetValue(resetValue);
                misionData.SetLevel(0);
            }

            // 상점 초기화
            foreach (var resStoreItem in ResourceManager.Instance.store.GetStoreItems())
            {
                if (resStoreItem.IsLimitFree())
                {
                    continue;
                }

                if (resStoreItem.IsInfinity())
                {
                    continue;
                }

                if (resStoreItem.IsWeeklyReset() && !isWeeklyReset)
                {
                    continue;
                }

                var storeData = uStore.GetData(resStoreItem.id);
                storeData.SetPurchaseCount(0);
            }

            // 출석
            foreach (var resAttendance in ResourceManager.Instance.attendance.GetAttendances())
            {
                if (!resAttendance.IsValidDate(now))
                {
                    continue;
                }

                var attendanceData = uAttendance.GetData(resAttendance.id);
                if (resAttendance.isLoop
                    && (attendanceData.flag & resAttendance.completeFlag) == resAttendance.completeFlag)
                {
                    attendanceData.Reset();
                }

                attendanceData.SetCount(attendanceData.count + 1);
            }

            uAchieve.AddValue(AchieveType.ATTENDANCE, 1);

            // 임무 업데이트
            UpdateMissions(uMission,
                MissionCtrl.Input.Of(1, true, MissionKeyType.ATTENDANCE)
            );
        }

        uInfo.SetLastLink(now);

        PlatformManager.Instance.Save(uInfo, uMission, uStore, uAttendance, uAchieve);
    }

    private static void TimeOfDay(Params _params)
    {
        UpdateDayOffset();
        _params.callback?.Invoke(null);
    }

    private static void GetUserInfo(Params _params)
    {
        var arg = GET_USER_INFO.Of(TManager.Instance.Get<TUserInfo>().SetInfo(User.Instance.info));
        _params.callback?.Invoke(arg);
    }

    private static void GetModeDatas(Params _params)
    {
        var arg = GET_MODE_DATAS.Of(User.Instance.mode.GetDatas().Select(x => TManager.Instance.Get<TMode>().SetData(x)));
        _params.callback?.Invoke(arg);
    }

    private static void GetItemDatas(Params _params)
    {
        var arg = GET_ITEM_DATAS.Of(User.Instance.item.GetDatas().Select(x => TManager.Instance.Get<TItem>().SetData(x)));
        _params.callback?.Invoke(arg);
    }

    private static void GetMissionDatas(Params _params)
    {
        var arg = GET_MISSION_DATAS.Of(User.Instance.mission.GetDatas().Select(x => TManager.Instance.Get<TMission>().SetData(x)));
        _params.callback?.Invoke(arg);
    }

    private static void GetAttendanceDatas(Params _params)
    {
        var arg = GET_ATTENDANCE_DATAS.Of(User.Instance.attendance.GetDatas().Select(x => TManager.Instance.Get<TAttendance>().SetData(x)));
        _params.callback?.Invoke(arg);
    }

    private static void GetLabDatas(Params _params)
    {
        var arg = GET_LAB_DATAS.Of(User.Instance.lab.GetDatas().Select(x => TManager.Instance.Get<TLab>().SetData(x)));
        _params.callback?.Invoke(arg);
    }

    private static void GetCollectionDatas(Params _params)
    {
        var arg = GET_COLLECTION_DATAS.Of(User.Instance.collection.GetDatas().Select(x => TManager.Instance.Get<TCollection>().SetData(x)));
        _params.callback?.Invoke(arg);
    }

    private static void GetStoreDatas(Params _params)
    {
        var arg = GET_STORE_DATAS.Of(User.Instance.store.GetDatas().Select(x => TManager.Instance.Get<TStore>().SetData(x)));
        _params.callback?.Invoke(arg);
    }

    private static void GetAdvertisement(Params _params)
    {
        var arg = GET_ADVERTISEMENT_DATAS.Of(User.Instance.ad.GetDatas().Select(x => TManager.Instance.Get<TAdvertisement>().SetData(x)));
        _params.callback?.Invoke(arg);
    }

    private static void GetInventoryInfo(Params _params)
    {
        var arg = GET_INVENTORY_INFO.Of(TManager.Instance.Get<TInventory>().SetInfo(User.Instance.inventory));
        _params.callback?.Invoke(arg);
    }

    private static void GetCardDatas(Params _params)
    {
        var arg = GET_CARD_DATAS.Of(User.Instance.card.GetDatas().Select(x => TManager.Instance.Get<TCard>().SetData(x)));
        _params.callback?.Invoke(arg);
    }

    private static void GetMergeDatas(Params _params)
    {
        var arg = GET_MERGE_DATAS.Of(User.Instance.merge.GetDatas().Select(x => TManager.Instance.Get<TMerge>().SetData(x)));
        _params.callback?.Invoke(arg);
    }

    private static void SuccessStage(Params _params)
    {
        var uMode = User.Instance.mode;
        var uMission = User.Instance.mission;
        var uItem = User.Instance.item;

        var mode = ModeManager.Instance.mode as StageMode;
        if (mode == null)
        {
            return;
        }

        var modeData = uMode.GetModeData(mode.core.profile.resMode.id);
        var titems = new List<TItem>();
        var mapGetInfos = new Dictionary<int, int>();

        var index = mode.core.profile.StageToIndex();
        if (mode.core.profile.resMode.stages.TryGetValue(index, out var stage))
        {
            ApplyPack(uItem, stage.packIDs, 1, ref titems, ref mapGetInfos);
        }

        var addStage = 1;
        var addExp = mode.core.profile.GetAddExp();
        var addGold = mode.core.profile.GetAddGold();
        if (addGold > 0 && uItem.TryGetItemData(GameData.ITEM_DATA.GOLD, out var goldItemData))
        {
            goldItemData.SetAmount(goldItemData.amount + addGold);
            titems.Add(TManager.Instance.Get<TItem>().SetData(goldItemData).AddUpdateFlag(TItem.UpdateFlag.AMOUNT));
        }

        modeData.AddStageScore(addStage);
        modeData.AddExp(addExp);

        // 미션 업데이트
        var updatedMissions = UpdateMissions(uMission,
            MissionCtrl.Input.Of(modeData.bestScore, false, MissionKeyType.MODE_SCORE, mode.core.profile.resMode.id)
        );

        PlatformManager.Instance.Save(uMode, uMission, uItem);

        var args = SUCCESS_STAGE.Of(
            TManager.Instance.Get<TMode>().SetData(modeData),
            updatedMissions,
            titems,
            GetInfosByMap(mapGetInfos),
            addGold
            );

        _params.callback?.Invoke(args);
    }

    private static void FailStage(Params _params)
    {
        var uMode = User.Instance.mode;

        var mode = ModeManager.Instance.mode;
        if (mode == null)
        {
            return;
        }

        var modeData = uMode.GetModeData(mode.core.profile.resMode.id);
        var current = modeData.score;
        var startScore = ((current - 1) / 10) * 10 + 1;
        if (startScore == current)
        {
            startScore = Math.Max(startScore - 10, 1);
        }

        modeData.SetScore(startScore);

        PlatformManager.Instance.Save(uMode);

        var arg = FAIL_STAGE.Of(
            TManager.Instance.Get<TMode>().SetData(modeData)
            );

        _params.callback?.Invoke(arg);
    }

    private static void RecordScore(Params _params)
    {
        var uMode = User.Instance.mode;
        var uMission = User.Instance.mission;

        var mode = ModeManager.Instance.mode;
        if (mode == null)
        {
            return;
        }
  
        var modeData = uMode.GetModeData(mode.core.profile.resMode.id);
        modeData.SetBestScore(mode.core.score.GetScore());

        var updatedMissions = UpdateMissions(uMission,
            MissionCtrl.Input.Of(modeData.bestScore, false, MissionKeyType.MODE_SCORE, mode.core.profile.resMode.id)
        );

        PlatformManager.Instance.Save(uMode, uMission);

        var arg = RECORD_SCORE.Of(
            TManager.Instance.Get<TMode>().SetData(modeData),
            updatedMissions
            );

        _params.callback?.Invoke(arg);
    }

    private static void ResetStageMode(Params _params)
    {
        var resStageMode = ResourceManager.Instance.mode.GetMode(GameData.MODE_DATA.MODE_1_ID);
        if (resStageMode == null)
        {
            return;
        }

        var uMode = User.Instance.mode;
        var stageModeData = uMode.GetModeData(resStageMode.id);
        var startScore = (stageModeData.score / 10) * 10 + 1;
        stageModeData.SetScore(startScore);

        PlatformManager.Instance.Save(uMode);

        var arg = RESET_STAGE_MODE.Of(
            TManager.Instance.Get<TMode>().SetData(stageModeData)
            );

        _params.callback?.Invoke(arg);
    }

    private static void ShowAd(Params _params)
    {
        var uAd = User.Instance.ad;
        var uItem = User.Instance.item;
        var uAchieve = User.Instance.achieve;

        var nowEpoch = nowToEpochSecond;
        var adID = _params.GetInt(0);
        var resAd = ResourceManager.Instance.ad.GetAd(adID);
        if (resAd == null)
        {
            return;
        }

        var adData = uAd.GetAdData(resAd.id);
        adData.SetCoolTime(nowEpoch + resAd.coolTime);

        var titems = new List<TItem>();
        var mapGetInfos = new Dictionary<int, int>();
        if (resAd.isShow)
        {
            ApplyPack(uItem, GameData.DEFAULT.SHOW_AD_PACK_ID, 1, ref titems, ref mapGetInfos);
            uAchieve.AddValue(AchieveType.SHOW_AD_COMPLETE, 1);
        }

        PlatformManager.Instance.Save(uAd, uItem, uAchieve);

        var arg = SHOW_AD.Of(
            TManager.Instance.Get<TAdvertisement>().SetData(adData),
            titems,
            GetInfosByMap(mapGetInfos)
        );

        _params.callback?.Invoke(arg);
    }

    private static void EquipItem(Params _params)
    {
        var uInventory = User.Instance.inventory;
        var uMission = User.Instance.mission;
        var uItem = User.Instance.item;

        var equipResItemID = _params.GetInt(0);
        var equipResItem = ResourceManager.Instance.item.GetItem(equipResItemID);
        if (equipResItem == null)
        {
            return;
        }
        
        var duplicationItem = uItem.GetDatas()
            .Where(x => uInventory.equipedItemIDs.Contains(x.id))
            .FirstOrDefault(x =>
            {
                var resItem = ResourceManager.Instance.item.GetItem(x.id);
                if (resItem == null)
                {
                    return false;
                }
            
                return equipResItem.itemGroupID == resItem.itemGroupID;
            });

        if (equipResItem.isSwitch && duplicationItem != null)
        {
            // 중복 아이템 해제
            uInventory.SetReleaseItem(duplicationItem.id);
        }

        var currEquipPoint = 0;
        var countByType = 1; // 장착 될 아이템 선행 카운트
        var countByGrade = 1; // 장착 될 아이템 선행 카운트
        foreach (var resItemID in uInventory.equipedItemIDs)
        {
            var resItem = ResourceManager.Instance.item.GetItem(resItemID);
            if (resItem == null)
            {
                continue;
            }

            currEquipPoint += resItem.equipPoint;

            // 장착 장비로 변경점이 생기는 임무만 업데이트
            if (resItem.itemType == equipResItem.itemType)
            {
                countByType += 1;
            } 

            // 장착 장비로 변경점이 생기는 임무만 업데이트
            if (resItem.grade == equipResItem.grade)
            {
                countByGrade += 1;
            }
        }

        var maxEquipPoint = GameData.DEFAULT.MAX_ITEM_EQUIP_POINT;
        if (currEquipPoint + equipResItem.equipPoint > maxEquipPoint)
        {
            Main.Instance.ShowFloatingMessage("key_ex_over_equippoint".L());
            return;
        }

        uInventory.SetEquipItem(equipResItem.id);

        PlatformManager.Instance.Save(uInventory, uMission);

        var arg = EQUIP_ITEM.Of(TManager.Instance.Get<TInventory>().SetInfo(uInventory));

        _params.callback?.Invoke(arg);
    }

    private static void ReleaseItem(Params _params)
    {
        var uInventory = User.Instance.inventory;

        var resItemID = _params.GetInt(0);
        uInventory.SetReleaseItem(resItemID);

        PlatformManager.Instance.Save(uInventory);

        var arg = EQUIP_ITEM.Of(TManager.Instance.Get<TInventory>().SetInfo(uInventory));

        _params.callback?.Invoke(arg);
    }

    private static void RerollItem(Params _params)
    {
        var uItem = User.Instance.item;
        var uAchieve = User.Instance.achieve;

        var resItemID = _params.GetInt(0);
        var resItem = ResourceManager.Instance.item.GetItem(resItemID);
        if (resItem == null)
        {
            return;
        }

        if (!uItem.TryGetItemData(resItem.id, out var itemData))
        {
            return;
        }

        if (!uItem.TryGetItemData(resItem.reroll.costItemID, out var costItemData))
        {
            return;
        }

        var needCost = resItem.reroll.value;
        var currAmount = costItemData.amount;
        if (currAmount < needCost)
        {
            return;
        }

        var options = new decimal[GameData.DEFAULT.MAX_ITEM_OPTION_COUNT];
        for (int i = 0; i < options.Length; ++i)
        {
            options[i] = UnityEngine.Random.Range(0, 11) * 0.1m;
        }

        itemData.SetOptions(options);
        costItemData.SetAmount(currAmount - needCost);

        uAchieve.AddValue(AchieveType.REROLL_ITEM, 1);

        PlatformManager.Instance.Save(uItem, uAchieve);

        var arg = UPDATE_ITEM.Of(
            new List<TItem>()
            {
                TManager.Instance.Get<TItem>().SetData(itemData).AddUpdateFlag(TItem.UpdateFlag.ALL),
                TManager.Instance.Get<TItem>().SetData(costItemData).AddUpdateFlag(TItem.UpdateFlag.AMOUNT)
            },
            null);

        _params.callback?.Invoke(arg);
    }

    private static void EnhanceItem(Params _params)
    {
        var uItem = User.Instance.item;
        var uMission = User.Instance.mission;
        var uAchieve = User.Instance.achieve;

        var resItemID = _params.GetInt(0);
        var resItem = ResourceManager.Instance.item.GetItem(resItemID);
        if (resItem == null)
        {
            return;
        }

        if (!uItem.TryGetItemData(resItem.id, out var itemData))
        {
            return;
        }

        if (!uItem.TryGetItemData(resItem.enhance.costItemID, out var costItemData))
        {
            return;
        }

        var add = _params.GetLong(1);
        var currExp = itemData.exp;
        var currLevel = resItem.GetExpToLevel(currExp);
        var nextExp = currExp + add;
        var nextLevel = resItem.GetExpToLevel(nextExp);
        var needCost = resItem.enhance.GetNeedCost(currLevel, nextLevel);
        var currAmount = costItemData.amount;
        if (currAmount < needCost)
        {
            // 강화에 필요한 재화가 부족합니다.
            Main.Instance.ShowFloatingMessage("key_ex_need_cost_lack".L());
            return;
        }

        itemData.SetExp(nextExp);
        costItemData.SetAmount(currAmount - needCost);

        uAchieve.AddValue(AchieveType.ENHANCE_ITEM, add);

        var updatedMissions = UpdateMissions(uMission,
            MissionCtrl.Input.Of(add, true, MissionKeyType.ENHANCE_ITEM, (int)resItem.itemType, resItem.grade)
        );

        PlatformManager.Instance.Save(uItem, uMission, uAchieve);

        var arg = UPDATE_ITEM.Of(
            new List<TItem>()
            {
                TManager.Instance.Get<TItem>().SetData(itemData).AddUpdateFlag(TItem.UpdateFlag.ALL),
                TManager.Instance.Get<TItem>().SetData(costItemData).AddUpdateFlag(TItem.UpdateFlag.AMOUNT)
            },
            updatedMissions);

        _params.callback?.Invoke(arg);
    }

    private static void EnhanceItemAll(Params _params)
    {
        var uItem = User.Instance.item;
        var uMission = User.Instance.mission;
        var uAchieve = User.Instance.achieve;

        var titems = new List<TItem>();
        var tmissions = new List<TMission>();
        var totalEnhance = 0L;
        foreach (var resItem in ResourceManager.Instance.item.GetItems())
        {
            if (!resItem.enhance.use)
            {
                continue;
            }

            if (!uItem.TryGetItemData(resItem.id, out var itemData))
            {
                continue;
            }

            if (!uItem.TryGetItemData(resItem.enhance.costItemID, out var costItemData))
            {
                continue;
            }

            var atItemEnhance = 0L;

            // 현재 초월 없이 강화할 수 있는 최대 레벨량은 50
            for (int i = 0; i < 50; ++i)
            {
                var add = 1;
                var currExp = itemData.exp;
                var currLevel = resItem.GetExpToLevel(currExp);
                var nextExp = currExp + add;
                var nextLevel = resItem.GetExpToLevel(nextExp);
                var needCost = resItem.enhance.GetNeedCost(currExp, nextExp);
                var currAmount = costItemData.amount;
                if (currAmount < needCost)
                {
                    // 강화에 필요한 재화가 부족.
                    break;
                }

                atItemEnhance += add;

                itemData.SetExp(nextExp);
                costItemData.SetAmount(currAmount - needCost);

                titems.Add(TManager.Instance.Get<TItem>().SetData(itemData).AddUpdateFlag(TItem.UpdateFlag.ALL));
                titems.Add(TManager.Instance.Get<TItem>().SetData(costItemData).AddUpdateFlag(TItem.UpdateFlag.AMOUNT));
            }

            totalEnhance += atItemEnhance;

            var updatedMissions = UpdateMissions(uMission,
                    MissionCtrl.Input.Of(atItemEnhance, true, MissionKeyType.ENHANCE_ITEM, (int)resItem.itemType, resItem.grade)
                );

            tmissions.AddRange(updatedMissions);
        }

        uAchieve.AddValue(AchieveType.ENHANCE_ITEM, totalEnhance);

        PlatformManager.Instance.Save(uItem, uMission, uAchieve);

        var arg = UPDATE_ITEM.Of(titems, tmissions);
        _params.callback?.Invoke(arg);
    }

    private static void AwakenItem(Params _params)
    {
        var uItem = User.Instance.item;
        var uMission = User.Instance.mission;
        var uAchieve = User.Instance.achieve;

        var resItemID = _params.GetInt(0);
        var resItem = ResourceManager.Instance.item.GetItem(resItemID);
        if (resItem == null)
        {
            return;
        }

        if (!uItem.TryGetItemData(resItem.id, out var itemData))
        {
            return;
        }

        if (!uItem.TryGetItemData(resItem.awaken.costItemID, out var costItemData))
        {
            return;
        }

        var current = itemData.awaken;
        if (resItem.awaken.maxAwaken > 0 && current >= resItem.awaken.maxAwaken)
        {
            Main.Instance.ShowFloatingMessage("key_ex_limit_max".L());
            return;
        }

        var add = _params.GetInt(1);
        var newAwaken = current + add;
        var needCost = add * resItem.awaken.GetNeedValue(itemData.awaken);
        var currAmount = costItemData.amount;
        if (currAmount < needCost)
        {
            return;
        }

        itemData.SetAwaken(newAwaken);
        costItemData.SetAmount(currAmount - needCost);

        uAchieve.AddValue(AchieveType.AWAKEN_ITEM, add);

        var updatedMissions = UpdateMissions(uMission,
            MissionCtrl.Input.Of(add, true, MissionKeyType.AWAKEN_ITEM)
        );

        PlatformManager.Instance.Save(uItem, uMission, uAchieve);

        var arg = UPDATE_ITEM.Of(
            new List<TItem>()
            {
                TManager.Instance.Get<TItem>().SetData(itemData).AddUpdateFlag(TItem.UpdateFlag.ALL),
                TManager.Instance.Get<TItem>().SetData(costItemData).AddUpdateFlag(TItem.UpdateFlag.AMOUNT)
            },
            updatedMissions);

        _params.callback?.Invoke(arg);
    }

    private static void DismantleItem(Params _params)
    {
        var uItem = User.Instance.item;
        var uMission = User.Instance.mission;
        var uAchieve = User.Instance.achieve;

        var resItemID = _params.GetInt(0);
        var resItem = ResourceManager.Instance.item.GetItem(resItemID);
        if (resItem == null)
        {
            return;
        }

        if (!uItem.TryGetItemData(resItem.id, out var itemData))
        {
            return;
        }

        // PACK과 COUNT로 해서 수정해야할것같다. 25 02 01
        if (!uItem.TryGetItemData(resItem.dismantle.getItemID, out var getItemData))
        {
            return;
        }

        var useAmount = Math.Min(_params.GetLong(1), itemData.amount);
        itemData.SetAmount(itemData.amount - useAmount);

        var getAmount = resItem.dismantle.value * useAmount;
        getItemData.SetAmount(getItemData.amount + getAmount);

        uAchieve.AddValue(AchieveType.DISMANTLE_ITEM, useAmount);

        var updatedMissions = UpdateMissions(uMission,
            MissionCtrl.Input.Of(useAmount, true, MissionKeyType.DISMANTLE_ITEM)
        );

        PlatformManager.Instance.Save(uItem, uMission, uAchieve);

        var getInfos = new List<GetInfo>()
        {
            GetInfo.Of(resItem.dismantle.getItemID, getAmount)
        };

        var arg = DISMANTLE_ITEM.Of(
            new List<TItem>()
            {
                TManager.Instance.Get<TItem>().SetData(itemData).AddUpdateFlag(TItem.UpdateFlag.ALL),
                TManager.Instance.Get<TItem>().SetData(getItemData).AddUpdateFlag(TItem.UpdateFlag.ALL)
            },
            updatedMissions,
            getInfos);

        _params.callback?.Invoke(arg);
    }

    private static void UpgradeLab(Params _params)
    {
        var uLab = User.Instance.lab;
        var uItem = User.Instance.item;
        var uMission = User.Instance.mission;
        var uAchieve = User.Instance.achieve;

        var resLabId = _params.GetInt(0);
        var resLab = ResourceManager.Instance.lab.GetLab(resLabId);
        if (resLab == null)
        {
            return;
        }

        if (!uItem.TryGetItemData(resLab.costItemID, out var costItemData))
        {
            return;
        }

        var add = _params.GetInt(1);
        var labData = uLab.GetData(resLab.id);
        var currLevel = labData.level;
        var nextLevel = currLevel + add;

        var currAmount = costItemData.amount;
        var needWealth = resLab.GetNeedCost(currLevel, nextLevel);
        if (currAmount < needWealth)
        {
            Main.Instance.ShowFloatingMessage("key_ex_leak_wealth".L());
            return;
        }

        labData.SetLevel(nextLevel);
        costItemData.SetAmount(currAmount - needWealth);

        uAchieve.AddValue(AchieveType.UPGRADE_LAB, add);

        var updatedMissions = UpdateMissions(uMission,
            MissionCtrl.Input.Of(labData.level, false, MissionKeyType.UPGRADE_LAB, resLab.id)
        );

        PlatformManager.Instance.Save(uLab, uItem, uMission, uAchieve);

        var arg = UPDATE_LAB.Of(
            TManager.Instance.Get<TLab>().SetData(labData),
            new List<TItem>() { TManager.Instance.Get<TItem>().SetData(costItemData).AddUpdateFlag(TItem.UpdateFlag.AMOUNT) },
            updatedMissions
        );

        _params.callback?.Invoke(arg);
    }

    private static void PurchaseStoreItem(Params _params)
    {
        var uStore = User.Instance.store;
        var uItem = User.Instance.item;
        var uMission = User.Instance.mission;
        var uAchieve = User.Instance.achieve;

        var resStoreItemId = _params.GetInt(0);
        var resStoreItem = ResourceManager.Instance.store.GetStoreItem(resStoreItemId);
        if (resStoreItem == null)
        {
            return;
        }

        var storeData = uStore.GetData(resStoreItem.id);
        if (!resStoreItem.IsLimitFree() && storeData.purchaseCount >= resStoreItem.limitCount)
        {
            Main.Instance.ShowFloatingMessage("key_ex_over_purchase".L());
            return;
        }

        if (!uItem.TryGetItemData(resStoreItem.costItemID, out var costItemData))
        {
            return;
        }

        var currAmount = costItemData.amount;
        if (currAmount < resStoreItem.costValue)
        {
            Main.Instance.ShowFloatingMessage("key_ex_leak_wealth".L());
            return;
        }

        costItemData.SetAmount(currAmount - resStoreItem.costValue);

        storeData.SetPurchaseCount(storeData.purchaseCount + 1);
        storeData.SetTotalPurchaseCount(storeData.totalPurchaseCount + 1);

        var titems = new List<TItem>();
        var mapGetInfos = new Dictionary<int, int>();
        ApplyPack(uItem, resStoreItem.packID, resStoreItem.count, ref titems, ref mapGetInfos);

        // 사용한 코스트 아이템
        titems.Add(TManager.Instance.Get<TItem>().SetData(costItemData).AddUpdateFlag(TItem.UpdateFlag.AMOUNT));

        uAchieve.AddValue(AchieveType.PURCHASE_STORE, 1);

        var updatedMissions = UpdateMissions(uMission,
            MissionCtrl.Input.Of(1, true, MissionKeyType.PURCHASE_STORE)
        );

        PlatformManager.Instance.Save(uStore, uItem, uMission);

        var arg = PURCHASE_STORE_ITEM.Of(
            TManager.Instance.Get<TStore>().SetData(storeData),
            updatedMissions,
            titems,
            GetInfosByMap(mapGetInfos)
        );

        _params.callback?.Invoke(arg);
    }

    private static void ClearMission(Params _params)
    {
        var uMission = User.Instance.mission;
        var uItem = User.Instance.item;

        var missionIDs = _params.GetList<int>(0);
        if (missionIDs == null)
        {
            return;
        }

        var isClearAll = _params.GetInt(1) == 1;
        var missionDatas = new List<UserMission.MissionData>();
        var titems = new List<TItem>();
        var mapGetInfos = new Dictionary<int, int>();

        foreach (var missionID in missionIDs)
        {
            if (!ResourceManager.Instance.mission.IsEnableMission(missionID))
            {
                continue;
            }

            var resMission = ResourceManager.Instance.mission.GetMission(missionID);
            if (resMission == null)
            {
                continue;
            }

            var missionData = uMission.GetData(resMission.id);
            missionDatas.Add(missionData);

            while (missionData.level < resMission.maxLevel)
            {
                var maxPoint = resMission.GetMaxPoint(missionData.level);
                if (missionData.value < maxPoint)
                {
                    break;
                }

                missionData.SetLevel(missionData.level + 1);

                ApplyPack(uItem, resMission.packIDs, 1, ref titems, ref mapGetInfos);

                if (!isClearAll)
                {
                    break;
                }
            }
        }

        PlatformManager.Instance.Save(uMission, uItem);

        var arg = CLEAR_MISSION.Of(
            missionDatas.Select(x => TManager.Instance.Get<TMission>().SetData(x)).ToArray(),
            titems,
            GetInfosByMap(mapGetInfos)
            );

        _params.callback?.Invoke(arg);
    }

    private static void ClearAttendance(Params _params)
    {
        var uAttendance = User.Instance.attendance;
        var uItem = User.Instance.item;

        var attendanceID = _params.GetInt(0);
        var resAttendance = ResourceManager.Instance.attendance.GetAttendance(attendanceID);
        if (resAttendance == null)
        {
            return;
        }

        var days = _params.GetList<int>(1);
        if (days == null || days.Count == 0)
        {
            return;
        }

        var attendanceData = uAttendance.GetData(resAttendance.id);
        var titems = new List<TItem>();
        var mapGetInfos = new Dictionary<int, int>();

        foreach (var day in days)
        {
            if (day > attendanceData.count)
            {
                continue;
            }

            var flag = (long)(1 << day);
            if ((attendanceData.flag & flag) == flag)
            {
                continue;
            }

            var reward = resAttendance.rewards.TryGetValue(day, out var v) ? v : null;
            if (reward == null)
            {
                continue;
            }

            attendanceData.SetFlag(attendanceData.flag | flag);
            ApplyPack(uItem, reward.GetPackIDs(), 1, ref titems, ref mapGetInfos);
        }

        if ((attendanceData.flag & resAttendance.completeFlag) == resAttendance.completeFlag)
        {
            attendanceData.SetCompleteDate(Main.Instance.time.nowToEpochSecond());
        }

        PlatformManager.Instance.Save(uAttendance, uItem);

        var arg = CLEAR_ATTENDANCE.Of(
            TManager.Instance.Get<TAttendance>().SetData(attendanceData),
            titems,
            GetInfosByMap(mapGetInfos)
            );

        _params.callback?.Invoke(arg);
    }

    private static void EnrollCollection(Params _params)
    {
        var uItem = User.Instance.item;
        var uCollection = User.Instance.collection;

        var resIDs = _params.GetList<int>(0);
        if (resIDs == null || resIDs.Count == 0)
        {
            return;
        }

        var tcollections = new List<TCollection>();

        foreach (var resID in resIDs)
        {
            var resCollection = ResourceManager.Instance.collection.GetCollection(resID);
            if (resCollection == null)
            {
                continue;
            }

            var enroll = true;
            foreach (var data in resCollection.datas)
            {
                if (!uItem.TryGetItemData(data.itemID, out var item))
                {
                    enroll = false;
                    break;
                }

                if (item.exp < data.itemLevel)
                {
                    enroll = false;
                    break;
                }
            }

            if (!enroll)
            {
                continue;
            }

            var collectionData = uCollection.GetData(resID);
            collectionData.SetComplete(true);

            var tcollection = TManager.Instance.Get<TCollection>()
                .SetData(collectionData);

            tcollections.Add(tcollection);
        }

        PlatformManager.Instance.Save(uItem, uCollection);

        var arg = ENROLL_COLLECTION.Of(
            tcollections
            );

        _params.callback?.Invoke(arg);
    }

    private static void SelectModeCard(Params _params)
    {
        var uCard = User.Instance.card;
        var uItem = User.Instance.item;

        var mode = ModeManager.Instance.mode;
        if (mode == null)
        {
            return;
        }

        var cardID = _params.GetInt(0);
        var resCard = ResourceManager.Instance.mode.GetModeCard(cardID);
        if (resCard == null)
        {
            return;
        }

        var tcards = new List<TCard>();
        var titems = new List<TItem>();

        UserCard.CardData addCardData(ResourceModeCard resCard, bool isBonus, int add)
        {
            var cardData = uCard.GetCardData(resCard.id);
            cardData.AddCount(isBonus, 1);

            tcards.Add(TManager.Instance.Get<TCard>().SetData(cardData));

            foreach (var itemID in resCard.itemIDs)
            {
                if (!uItem.TryGetItemData(itemID, out var itemData))
                {
                    itemData = uItem.AddNewItem(itemID, 1);
                }

                itemData.SetExp(cardData.GetLevel());
                titems.Add(TManager.Instance.Get<TItem>().SetData(itemData).AddUpdateFlag(TItem.UpdateFlag.ALL));
            }

            return cardData;
        }

        var originCardData = addCardData(resCard, false, 1);
        var resBonusCard = ResourceManager.Instance.mode.GetModeCard(resCard.GetBonusCardIDByLevel(originCardData.GetLevel()));
        if (resBonusCard != null)
        {
            addCardData(resBonusCard, true, 1);
        }

        PlatformManager.Instance.Save(uCard, uItem);

        _params.callback?.Invoke(SELECT_MODE_CARD.Of(tcards, titems));
    }

    private static void CreateMergeNew(Params _params)
    {
        var uMerge = User.Instance.merge;
        var uItem = User.Instance.item;

        var createResID = 1;
        var resMerge = ResourceManager.Instance.merge.GetMerge(createResID);
        if (resMerge == null)
        {
            return;
        }

        var tmerges = new List<TMerge>();
        var titems = new List<TItem>();

        var locate = Util.ConvertRateToLocate(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
        var mergeData = uMerge.Get(resMerge.id);
        mergeData.Add(1, locate);

        tmerges.Add(TManager.Instance.Get<TMerge>().SetData(mergeData));

        foreach (var itemID in resMerge.itemIDs)
        {
            var resItem = ResourceManager.Instance.item.GetItem(itemID);
            if (resItem == null)
            {
                continue;
            }

            var updateFlag = TItem.UpdateFlag.NONE;
            if (!uItem.TryGetItemData(resItem.id, out var itemData))
            {
                updateFlag |= TItem.UpdateFlag.NEW;
            }

            itemData = uItem.AddNewItem(resItem.id, 1);

            var prevExp = itemData.exp;
            var prevLevel = resItem.GetExpToLevel(prevExp);
            var nextExp = itemData.amount;
            var nextLevel = resItem.GetExpToLevel(nextExp);

            itemData.SetExp(nextExp);

            updateFlag |= prevLevel == nextLevel ? TItem.UpdateFlag.AMOUNT : TItem.UpdateFlag.ALL;
            titems.Add(TManager.Instance.Get<TItem>().SetData(itemData).AddUpdateFlag(updateFlag));
        }

        PlatformManager.Instance.Save(uMerge, uItem);

        _params.callback?.Invoke(CREATE_MERGE_NEW.Of(tmerges, titems, resMerge.id, locate));
    }

    private static void CreateMergeNext(Params _params)
    {
        var uMerge = User.Instance.merge;
        var uItem = User.Instance.item;

        var mergeID = _params.GetInt(0);
        var resMerge = ResourceManager.Instance.merge.GetMerge(mergeID);
        if (resMerge == null)
        {
            return;
        }

        var titems = new List<TItem>();
        var tmerges = new List<TMerge>();

        var locateList = _params.GetList<int>(1);
        var combineData = uMerge.Get(resMerge.id);
        combineData.Remove(2);
        combineData.SetLocateList(locateList);
        tmerges.Add(TManager.Instance.Get<TMerge>().SetData(combineData));

        var locate = _params.GetInt(2);
        var newData = uMerge.Get(resMerge.nextMergeID);
        newData.Add(1, locate);
        tmerges.Add(TManager.Instance.Get<TMerge>().SetData(newData));

        var resNextMerge = ResourceManager.Instance.merge.GetMerge(resMerge.nextMergeID);
        if (resNextMerge != null)
        {
            foreach (var itemID in resNextMerge.itemIDs)
            {
                var resItem = ResourceManager.Instance.item.GetItem(itemID);
                if (resItem == null)
                {
                    continue;
                }

                var updateFlag = TItem.UpdateFlag.NONE;
                if (!uItem.TryGetItemData(resItem.id, out var itemData))
                {
                    updateFlag |= TItem.UpdateFlag.NEW;
                }

                itemData = uItem.AddNewItem(resItem.id, 1);

                var prevExp = itemData.exp;
                var prevLevel = resItem.GetExpToLevel(prevExp);
                var nextExp = itemData.amount;
                var nextLevel = resItem.GetExpToLevel(nextExp);
                itemData.SetExp(nextExp);

                updateFlag |= prevLevel == nextLevel ? TItem.UpdateFlag.AMOUNT : TItem.UpdateFlag.ALL;
                titems.Add(TManager.Instance.Get<TItem>().SetData(itemData).AddUpdateFlag(updateFlag));
            }
        }

        var resNextPack = ResourceManager.Instance.pack.GetPack(resMerge.nextPackID);
        if (resNextPack != null)
        {
            var mapGetInfos = new Dictionary<int, int>();
            ApplyPack(uItem, resNextPack.id, 1, ref titems, ref mapGetInfos);

            foreach (var itemID in mapGetInfos.Keys)
            {
                var resItem = ResourceManager.Instance.item.GetItem(itemID);
                if (resItem == null)
                {
                    continue;
                }

                if (!uItem.TryGetItemData(resItem.id, out var itemData))
                {
                    continue;
                }

                itemData.SetExp(itemData.amount);
                titems.Add(TManager.Instance.Get<TItem>().SetData(itemData).AddUpdateFlag(TItem.UpdateFlag.ALL));
            }
        }

        PlatformManager.Instance.Save(uMerge, uItem);

        _params.callback?.Invoke(CREATE_MERGE_NEXT.Of(tmerges, titems, resMerge.nextMergeID));
    }

    private static void CompleteGuide(Params _params)
    {
        var uInfo = User.Instance.info;
        var guide = _params.GetLong(0);

        uInfo.AddGuideFlag(guide);

        PlatformManager.Instance.Save(uInfo);

        _params.callback?.Invoke(COMPLETE_GUIDE.Of(TManager.Instance.Get<TUserInfo>().SetInfo(uInfo)));
    }

    private static void ApplyPack(UserItem uItem, int packID, int count, ref List<TItem> titems, ref Dictionary<int, int> mapGetInfos)
    {
        ApplyPack(uItem, new List<int> { packID }, count, ref titems, ref mapGetInfos);
    }

    private static void ApplyPack(UserItem uItem, ICollection<int> packIDs, int count, ref List<TItem> titems, ref Dictionary<int, int> mapGetInfos)
    {
        foreach (var packID in packIDs)
        {
            var resPack = ResourceManager.Instance.pack.GetPack(packID);
            if (resPack == null)
            {
                continue;
            }

            for (int i = 0; i < count; ++i)
            {
                ApplyPackInternal(uItem, resPack, titems, mapGetInfos);
            }
        }
    }

    private static void ApplyPackInternal(UserItem uItem, ResourcePack resPack, List<TItem> titems, Dictionary<int, int> mapGetInfos)
    {
        var members = GetPackMembers(resPack);
        foreach (var member in members)
        {
            if (member == null)
            {
                if (_DEBUG)
                {
                    Debug.Log($"pack member is null, pack id:{resPack.id}");
                }
                continue;
            }

            switch (resPack.packItemType)
            {
                case PackItemType.PACK:
                    var resInPack = ResourceManager.Instance.pack.GetPack(member.id);
                    if (resInPack == null)
                    {
                        continue;
                    }

                    for (int i = 0; i < member.amount; ++i)
                    {
                        ApplyPackInternal(uItem, resInPack, titems, mapGetInfos);
                    }
                    break;

                case PackItemType.ITEM:
                    var resItem = ResourceManager.Instance.item.GetItem(member.id);
                    if (resItem == null)
                    {
                        continue;
                    }

                    var updateFlag = TItem.UpdateFlag.NONE;
                    if (!uItem.TryGetItemData(resItem.id, out var itemData))
                    {
                        updateFlag |= TItem.UpdateFlag.NEW;
                    }

                    itemData = uItem.AddNewItem(resItem.id, member.amount);
                    updateFlag |= TItem.UpdateFlag.AMOUNT;

                    var titem = TManager.Instance.Get<TItem>()
                        .SetData(itemData)
                        .AddUpdateFlag(updateFlag)
                        ;

                    titems.Add(titem);

                    if (!mapGetInfos.ContainsKey(resItem.id))
                    {
                        mapGetInfos.Add(resItem.id, 0);
                    }

                    mapGetInfos[resItem.id] += member.amount;
                    break;
            }
        }
    }

    private static List<ResourcePack.Member> GetPackMembers(ResourcePack pack)
    {
        var tempPackMembers = new List<ResourcePack.Member>();
        if (pack.fixedMembers.Count > 0)
        {
            tempPackMembers.AddRange(pack.fixedMembers);
        }

        var ranPick = pack.GetRandomPick();
        if (ranPick != null)
        {
            tempPackMembers.Add(ranPick);
        }

        return tempPackMembers;
    }

    private static IList<GetInfo> GetInfosByMap(Dictionary<int, int> mapGetInfos)
    {
        return mapGetInfos.Select(x => GetInfo.Of(x.Key, x.Value)).ToList();
    }

    private static List<TMission> UpdateMissions(UserMission uMission, params MissionCtrl.Input[] inputParams)
    {
        var missionParams = MissionCtrl.GetUpdateParams(inputParams);
        if (missionParams.Count == 0)
        {
            return null;
        }

        var missions = new List<TMission>();
        foreach (var param in missionParams)
        {
            var data = uMission.GetData(param.resID);
            if (param.isAddValue)
            {
                data.SetValue(data.value + param.value);
            }
            else
            {
                data.SetValue(data.value > param.value ? (long)data.value : param.value);
            }

            missions.Add(TManager.Instance.Get<TMission>().SetData(data));
        }

        return missions;
    }

    public static bool TryGet<T>(object e, out T result) where T : class
    {
        if (e is T t)
        {
            result = t;
            return true;
        }

        result = default;
        return false;
    }

#if USE_DEBUG
    protected const bool _DEBUG = true;
#else
    protected const bool _DEBUG = false;
#endif
}

public enum Packet
{
    LOGIN,
    TIME_OF_DAY,
    CREATE_NICKNAME,

    GET_USER_INFO,
    GET_MODE_DATAS,
    GET_ATTENDANCE_DATAS,
    GET_ITEM_DATAS,
    GET_MISSION_DATAS,
    GET_LAB_DATAS,
    GET_COLLECTION_DATAS,
    GET_STORE_DATAS,
    GET_ADVERTISEMENT_DATAS,
    GET_INVENTORY_INFO,
    GET_CARD_DATAS,
    GET_MERGE_DATAS,

    SUCCESS_STAGE,
    FAIL_STAGE,

    SHOW_AD,
    EQUIP_ITEM,
    RELEASE_ITEM,
    REROLL_ITEM,
    ENHANCE_ITEM,
    ENHANCE_ITEM_ALL,
    AWAKEN_ITEM,
    DISMANTLE_ITEM,
    UPGRADE_LAB,
    PURCHASE_STORE_ITEM,
    CLEAR_MISSION,
    CLEAR_ATTENDANCE,
    ENROLL_COLLECTION,
    RECORD_SCORE,
    RESET_STAGE_MODE,

    SELECT_MODE_CARD,
    CREATE_MERGE_NEW,
    CREATE_MERGE_NEXT,

    COMPLETE_GUIDE,
}

public class CREATE_SERAIL
{
    public readonly TUserInfo tinfo;

    public static CREATE_SERAIL Of(TUserInfo tinfo)
    {
        return new CREATE_SERAIL(tinfo);
    }

    private CREATE_SERAIL(TUserInfo tinfo)
    {
        this.tinfo = tinfo;
    }
}

public class CREATE_NICKNAME
{
    public readonly TUserInfo tinfo;

    public static CREATE_NICKNAME Of(TUserInfo tinfo)
    {
        return new CREATE_NICKNAME(tinfo);
    }

    private CREATE_NICKNAME(TUserInfo tinfo)
    {
        this.tinfo = tinfo;
    }
}

public class GET_USER_INFO
{
    public readonly TUserInfo tinfo;

    public static GET_USER_INFO Of(TUserInfo tinfo)
    {
        return new GET_USER_INFO(tinfo);
    }

    private GET_USER_INFO(TUserInfo tinfo)
    {
        this.tinfo = tinfo;
    }
}

public class GET_MODE_DATAS
{
    public readonly IEnumerable<TMode> tmodes;

    public static GET_MODE_DATAS Of(IEnumerable<TMode> tmodes)
    {
        return new GET_MODE_DATAS(tmodes);
    }

    private GET_MODE_DATAS(IEnumerable<TMode> tmodes)
    {
        this.tmodes = tmodes;
    }
}

public class GET_ITEM_DATAS
{
    public readonly IEnumerable<TItem> titems;

    public static GET_ITEM_DATAS Of(IEnumerable<TItem> titems)
    {
        return new GET_ITEM_DATAS(titems);
    }

    private GET_ITEM_DATAS(IEnumerable<TItem> titems)
    {
        this.titems = titems;
    }
}

public class GET_MISSION_DATAS
{
    public readonly IEnumerable<TMission> tmissions;

    public static GET_MISSION_DATAS Of(IEnumerable<TMission> tmissions)
    {
        return new GET_MISSION_DATAS(tmissions);
    }

    private GET_MISSION_DATAS(IEnumerable<TMission> tmissions)
    {
        this.tmissions = tmissions;
    }
}

public class GET_ATTENDANCE_DATAS
{
    public readonly IEnumerable<TAttendance> tattendances;

    public static GET_ATTENDANCE_DATAS Of(IEnumerable<TAttendance> attendances)
    {
        return new GET_ATTENDANCE_DATAS(attendances);
    }

    private GET_ATTENDANCE_DATAS(IEnumerable<TAttendance> attendances)
    {
        this.tattendances = attendances;
    }
}

public class GET_LAB_DATAS
{
    public readonly IEnumerable<TLab> tlabs;

    public static GET_LAB_DATAS Of(IEnumerable<TLab> labs)
    {
        return new GET_LAB_DATAS(labs);
    }

    private GET_LAB_DATAS(IEnumerable<TLab> labs)
    {
        this.tlabs = labs;
    }
}

public class GET_COLLECTION_DATAS
{
    public readonly IEnumerable<TCollection> tcollections;

    public static GET_COLLECTION_DATAS Of(IEnumerable<TCollection> tcollections)
    {
        return new GET_COLLECTION_DATAS(tcollections);
    }

    private GET_COLLECTION_DATAS(IEnumerable<TCollection> tcollections)
    {
        this.tcollections = tcollections;
    }
}

public class GET_STORE_DATAS
{
    public readonly IEnumerable<TStore> tstores;

    public static GET_STORE_DATAS Of(IEnumerable<TStore> tstores)
    {
        return new GET_STORE_DATAS(tstores);
    }

    private GET_STORE_DATAS(IEnumerable<TStore> tstores)
    {
        this.tstores = tstores;
    }
}

public class GET_ADVERTISEMENT_DATAS
{
    public readonly IEnumerable<TAdvertisement> tadvertisements;

    public static GET_ADVERTISEMENT_DATAS Of(IEnumerable<TAdvertisement> tadvertisements)
    {
        return new GET_ADVERTISEMENT_DATAS(tadvertisements);
    }

    private GET_ADVERTISEMENT_DATAS(IEnumerable<TAdvertisement> tadvertisements)
    {
        this.tadvertisements = tadvertisements;
    }
}

public class GET_INVENTORY_INFO
{
    public readonly TInventory tinventory;

    public static GET_INVENTORY_INFO Of(TInventory tinventory)
    {
        return new GET_INVENTORY_INFO(tinventory);
    }

    private GET_INVENTORY_INFO(TInventory tinventory)
    {
        this.tinventory = tinventory;
    }
}

public class GET_CARD_DATAS
{
    public readonly IEnumerable<TCard> tcards;

    public static GET_CARD_DATAS Of(IEnumerable<TCard> tcards)
    {
        return new GET_CARD_DATAS(tcards);
    }

    private GET_CARD_DATAS(IEnumerable<TCard> tcards)
    {
        this.tcards = tcards;
    }
}

public class GET_MERGE_DATAS
{
    public readonly IEnumerable<TMerge> tmergs;

    public static GET_MERGE_DATAS Of(IEnumerable<TMerge> tmergs)
    {
        return new GET_MERGE_DATAS(tmergs);
    }

    private GET_MERGE_DATAS(IEnumerable<TMerge> tmergs)
    {
        this.tmergs = tmergs;
    }
}

public class JOIN_UNIT
{
    public readonly Type unitType;
    public readonly TUnit tunit;
    public readonly Vector3 position;

    public static JOIN_UNIT Of(Type unitType, TUnit tunit, Vector3 position)
    {
        return new JOIN_UNIT(unitType, tunit, position);
    }

    private JOIN_UNIT(Type unitType, TUnit tunit, Vector3 position)
    {
        this.unitType = unitType;
        this.tunit = tunit;
        this.position = position;
    }
}

public class SHOW_AD
{
    public TAdvertisement tadvertisement;
    public readonly ICollection<TItem> titems;
    public readonly IList<GetInfo> getInfos;

    public static SHOW_AD Of(TAdvertisement tadvertisement, ICollection<TItem> titems, IList<GetInfo> getInfos)
    {
        return new SHOW_AD(tadvertisement, titems, getInfos);
    }

    private SHOW_AD(TAdvertisement tadvertisement, ICollection<TItem> titems, IList<GetInfo> getInfos)
    {
        this.tadvertisement = tadvertisement;
        this.titems = titems;
        this.getInfos = getInfos;
    }
}

public class SUCCESS_STAGE
{
    public readonly TMode tmode;
    public readonly ICollection<TMission> tmissions;
    public readonly ICollection<TItem> titems;
    public readonly IList<GetInfo> getInfos;
    public readonly long addGold;

    public static SUCCESS_STAGE Of(TMode tmode, ICollection<TMission> tmissions, ICollection<TItem> titems, IList<GetInfo> getInfos, long addGold)
    {
        return new SUCCESS_STAGE(tmode, tmissions, titems, getInfos, addGold);
    }

    private SUCCESS_STAGE(TMode tmode, ICollection<TMission> tmissions, ICollection<TItem> titems, IList<GetInfo> getInfos, long addGold)
    {
        this.tmode = tmode;
        this.tmissions = tmissions;
        this.titems = titems;
        this.getInfos = getInfos;
        this.addGold = addGold;
    }
}

public class FAIL_STAGE
{
    public readonly TMode tmode;

    public static FAIL_STAGE Of(TMode tmode)
    {
        return new FAIL_STAGE(tmode);
    }

    private FAIL_STAGE(TMode tmode)
    {
        this.tmode = tmode;
    }
}

public class SUCCESS_DUNBREAK
{
    public readonly TMode tmode;
    public readonly ICollection<TMission> tmissions;
    public readonly ICollection<TItem> titems;
    public readonly IList<GetInfo> getInfos;
    public readonly long addGold;

    public static SUCCESS_DUNBREAK Of(TMode tmode, ICollection<TMission> tmissions, ICollection<TItem> titems, IList<GetInfo> getInfos, long addGold)
    {
        return new SUCCESS_DUNBREAK(tmode, tmissions, titems, getInfos, addGold);
    }

    private SUCCESS_DUNBREAK(TMode tmode, ICollection<TMission> tmissions, ICollection<TItem> titems, IList<GetInfo> getInfos, long addGold)
    {
        this.tmode = tmode;
        this.tmissions = tmissions;
        this.titems = titems;
        this.getInfos = getInfos;
        this.addGold = addGold;
    }
}

public class FAIL_DUNBREAK
{
    public readonly TMode tmode;

    public static FAIL_DUNBREAK Of(TMode tmode)
    {
        return new FAIL_DUNBREAK(tmode);
    }

    private FAIL_DUNBREAK(TMode tmode)
    {
        this.tmode = tmode;
    }
}

public class RECORD_SCORE
{
    public readonly TMode tmode;
    public readonly ICollection<TMission> tmissions;

    public static RECORD_SCORE Of(TMode tmode, ICollection<TMission> tmissions)
    {
        return new RECORD_SCORE(tmode, tmissions);
    }

    private RECORD_SCORE(TMode tmode, ICollection<TMission> tmissions)
    {
        this.tmode = tmode;
        this.tmissions = tmissions;
    }
}

public class EQUIP_ITEM
{
    public readonly TInventory tinventory;

    public static EQUIP_ITEM Of(TInventory tinventory)
    {
        return new EQUIP_ITEM(tinventory);
    }

    private EQUIP_ITEM(TInventory tinventory)
    {
        this.tinventory = tinventory;
    }
}

public class UPDATE_ITEM
{
    public readonly ICollection<TItem> titems;
    public readonly ICollection<TMission> tmissions;

    public static UPDATE_ITEM Of(ICollection<TItem> titems, ICollection<TMission> tmissions)
    {
        return new UPDATE_ITEM(titems, tmissions);
    }

    private UPDATE_ITEM(ICollection<TItem> titems, ICollection<TMission> tmissions)
    {
        this.titems = titems;
        this.tmissions = tmissions;
    }
}

public class DISMANTLE_ITEM
{
    public readonly ICollection<TItem> titems;
    public readonly ICollection<TMission> tmissions;
    public readonly IList<GetInfo> getInfos;

    public static DISMANTLE_ITEM Of(ICollection<TItem> titems, ICollection<TMission> tmissions, IList<GetInfo> getInfos)
    {
        return new DISMANTLE_ITEM(titems, tmissions, getInfos);
    }

    private DISMANTLE_ITEM(ICollection<TItem> titems, ICollection<TMission> tmissions, IList<GetInfo> getInfos)
    {
        this.titems = titems;
        this.tmissions = tmissions;
        this.getInfos = getInfos;
    }
}

public class UPDATE_LAB
{
    public readonly TLab tlab;
    public readonly ICollection<TItem> titems;
    public readonly ICollection<TMission> tmissions;

    public static UPDATE_LAB Of(TLab tlab, ICollection<TItem> titems, ICollection<TMission> tmissions)
    {
        return new UPDATE_LAB(tlab, titems, tmissions);
    }

    private UPDATE_LAB(TLab tlab, ICollection<TItem> titems, ICollection<TMission> tmissions)
    {
        this.tlab = tlab;
        this.titems = titems;
        this.tmissions = tmissions;
    }
}

public class PURCHASE_STORE_ITEM
{
    public readonly TStore tstore;
    public readonly ICollection<TItem> titems;
    public readonly ICollection<TMission> tmissions;
    public readonly IList<GetInfo> getInfos;

    public static PURCHASE_STORE_ITEM Of(TStore tstore, ICollection<TMission> tmissions, ICollection<TItem> titems, IList<GetInfo> getInfos)
    {
        return new PURCHASE_STORE_ITEM(tstore, tmissions, titems, getInfos);
    }

    private PURCHASE_STORE_ITEM(TStore tstore, ICollection<TMission> tmissions, ICollection<TItem> titems, IList<GetInfo> getInfos)
    {
        this.tstore = tstore;
        this.tmissions = tmissions;
        this.titems = titems;
        this.getInfos = getInfos;
    }
}

public class CLEAR_MISSION
{
    public readonly ICollection<TMission> tmissions;
    public readonly ICollection<TItem> titems;
    public readonly IList<GetInfo> getInfos;

    public static CLEAR_MISSION Of(ICollection<TMission> tmissions, ICollection<TItem> titems, IList<GetInfo> getInfos)
    {
        return new CLEAR_MISSION(tmissions, titems, getInfos);
    }

    private CLEAR_MISSION(ICollection<TMission> tmissions, ICollection<TItem> titems, IList<GetInfo> getInfos)
    {
        this.tmissions = tmissions;
        this.titems = titems;
        this.getInfos = getInfos;
    }
}

public class CLEAR_ATTENDANCE
{
    public readonly TAttendance tattendance;
    public readonly ICollection<TItem> titems;
    public readonly IList<GetInfo> getInfos;

    public static CLEAR_ATTENDANCE Of(TAttendance tattendance, ICollection<TItem> titems, IList<GetInfo> getInfos)
    {
        return new CLEAR_ATTENDANCE(tattendance, titems, getInfos);
    }

    private CLEAR_ATTENDANCE(TAttendance tattendance, ICollection<TItem> titems, IList<GetInfo> getInfos)
    {
        this.tattendance = tattendance;
        this.titems = titems;
        this.getInfos = getInfos;
    }
}

public class ENROLL_COLLECTION
{
    public readonly ICollection<TCollection> tcollections;

    public static ENROLL_COLLECTION Of(ICollection<TCollection> tcollections)
    {
        return new ENROLL_COLLECTION(tcollections);
    }

    private ENROLL_COLLECTION(ICollection<TCollection> tcollections)
    {
        this.tcollections = tcollections;
    }
}

public class RESET_STAGE_MODE
{
    public readonly TMode tmode;

    public static RESET_STAGE_MODE Of(TMode tmode)
    {
        return new RESET_STAGE_MODE(tmode);
    }

    private RESET_STAGE_MODE(TMode tmode)
    {
        this.tmode = tmode;
    }
}

public class SELECT_MODE_CARD
{
    public readonly ICollection<TCard> tcards;
    public readonly ICollection<TItem> titems;

    public static SELECT_MODE_CARD Of(ICollection<TCard> tcards, ICollection<TItem> titems)
    {
        return new SELECT_MODE_CARD(tcards, titems);
    }

    private SELECT_MODE_CARD(ICollection<TCard> tcards, ICollection<TItem> titems)
    {
        this.tcards = tcards;
        this.titems = titems;
    }
}

public class CREATE_MERGE_NEW
{
    public readonly ICollection<TMerge> tmergs;
    public readonly ICollection<TItem> titems;
    public readonly int createID;
    public readonly int locate;

    public static CREATE_MERGE_NEW Of(ICollection<TMerge> tmergs, ICollection<TItem> titems, int createID, int locate)
    {
        return new CREATE_MERGE_NEW(tmergs, titems, createID, locate);
    }

    private CREATE_MERGE_NEW(ICollection<TMerge> tmergs, ICollection<TItem> titems, int createID, int locate)
    {
        this.tmergs = tmergs;
        this.titems = titems;
        this.createID = createID;
        this.locate = locate;
    }
}

public class CREATE_MERGE_NEXT
{
    public readonly ICollection<TMerge> tmergs;
    public readonly ICollection<TItem> titems;
    public readonly int createID;

    public static CREATE_MERGE_NEXT Of(ICollection<TMerge> tmergs, ICollection<TItem> titems, int createID)
    {
        return new CREATE_MERGE_NEXT(tmergs, titems, createID);
    }

    private CREATE_MERGE_NEXT(ICollection<TMerge> tmergs, ICollection<TItem> titems, int createID)
    {
        this.tmergs = tmergs;
        this.titems = titems;
        this.createID = createID;
    }
}

public class COMPLETE_GUIDE
{
    public readonly TUserInfo tinfo;

    public static COMPLETE_GUIDE Of(TUserInfo tinfo)
    {
        return new COMPLETE_GUIDE(tinfo);
    }

    private COMPLETE_GUIDE(TUserInfo tinfo)
    {
        this.tinfo = tinfo;
    }
}

public struct EnemyAttacked
{
    public readonly long damaged;
    public readonly bool firstHit;
    public readonly long unitUID;
    public readonly float newHp;
    public readonly float prevHp;

    public static EnemyAttacked Of(long damaged, bool firstHit, long unitUID, float newHp, float prevHp)
    {
        return new EnemyAttacked(damaged, firstHit, unitUID, newHp, prevHp);
    }

    private EnemyAttacked(long damaged, bool firstHit, long unitUID, float newHp, float prevHp)
    {
        this.damaged = damaged;
        this.firstHit = firstHit;
        this.unitUID = unitUID;
        this.newHp = newHp;
        this.prevHp = prevHp;
    }
}
