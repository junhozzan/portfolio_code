using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace UIMerge
{
    /*
     * UI에서 정보를 들고 있지 않도록 코드 수정 필요
     */
    public class CpUI_Merge : UIMonoBehaviour
    {
        private static CpUI_Merge instance = null;
        public static CpUI_Merge Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_Merge>("pf_ui_merge_bottom");
                }

                return instance;
            }
        }

        [SerializeField] GameObject mergeButton = null;
        [SerializeField] GameObject createButton = null;
        [SerializeField] UIToggle autoPlayToggle = null;
        [SerializeField] CpUI_Merge_Item originItem = null;
        [SerializeField] CpUI_Merge_Controller controller = null;
        [SerializeField] CpUI_Merge_CircleGage circleGage = null;

        private ObjectPool<CpUI_Merge_Item> itemPool = null;

        private readonly Dictionary<int, List<CpUI_Merge_Item>> items = new Dictionary<int, List<CpUI_Merge_Item>>();
        private readonly List<int> sortedKeys = new List<int>();
        private readonly Dictionary<int, List<int>> savedAllLocate = new Dictionary<int, List<int>>();

        private long cellCount = 0;
        private float gageTime = 0f;
        private float autoMergeTime = 0f;
        private float autoCreateTime = 0f;

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.BASE, true);
            UsingUpdate();

            controller.Init(OnDragStart, OnDragEnd);
            circleGage.Init();

            autoPlayToggle.Init();
            autoPlayToggle.AddValueChanged(OnAutoPlayValueChanage);

            itemPool = ObjectPool<CpUI_Merge_Item>.Of(originItem, originItem.transform.parent);
            Cmd.Add(createButton, eCmdTrigger.OnClick, Cmd_Create);
            Cmd.Add(mergeButton, eCmdTrigger.OnClick, Cmd_Merge);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.JOIN_MY_UNIT, Handle_JOIN_MY_UNIT)
                .Add(GameEventType.GET_MERGE_DATAS, Refresh)
                .Add(GameEventType.CREATE_MERGE_ITEM, Handle_CREATE_MERGE_ITEM)
                .Add(GameEventType.UPDATE_LAB, Handle_UPDATE_LAB)
                ;
        }

        private void Handle_JOIN_MY_UNIT(object[] args)
        {
            AddCellCount(GetMaxCell());
            SetGageTime(Main.Instance.time.realtimeSinceStartup);

            if (!MyPlayer.Instance.core.profile.IsCompleteGuide(GuideType.CREATE_ITEM))
            {
                MyPlayer.Instance.core.profile.ShowGuide(GuideType.CREATE_ITEM);
            }
        }

        private void Handle_CREATE_MERGE_ITEM(object[] args)
        {
            var createID = GameEvent.GetSafeS<int>(args, 0);
            var locate = GameEvent.GetSafeS<int>(args, 1);
            if (createID.HasValue && locate.HasValue)
            {
                var item = PopItem(createID.Value, locate.Value);
                if (item != null)
                {
                    item.ShowMergeEffect();
                }
            }
        }

        private void Handle_UPDATE_LAB(object[] args)
        {
            RefreshCell();
        }

        private void OnAutoPlayValueChanage(bool b)
        {
            Option.isAutoPlay = b;
            Option.Save();
        }

        public override void UpdateDt(float unDt, DateTime now)
        {
            base.UpdateDt(unDt, now);
            
            // 가이드 진행중일때는 어떤 실행도 하지 않는다.
            if (UIGuide.CpUI_Guide.IsPlay())
            {
                return;
            }

            UpdateGage(unDt);
            UpdateAutoCreate();
            UpdateAutoMerge();
        }

        private void UpdateGage(float unDt)
        {
            var curr = Main.Instance.time.realtimeSinceStartup;
            var maxCell = GetMaxCell();
            if (cellCount >= maxCell)
            {
                SetGageTime(curr);
                return;
            }

            var speed = GetGageSpeed();
            var add = speed > 0 ? GameData.DEFAULT.MERGE_CHARGE_TIME / speed : 0;
            for (var a = gageTime + add; a <= curr; a += add)
            {
                AddCellCount(1);
                SetGageTime(a);

                if (cellCount >= maxCell)
                {
                    break;
                }
            }

            RefreshGage();
        }

        private void OnDragStart()
        {
            var dragItem = controller.GetControllItem();
            if (dragItem == null)
            {
                return;
            }

            foreach (var kv in items)
            {
                foreach (var item in kv.Value)
                {
                    item.SetFade(dragItem.resMerge.id != item.resMerge.id);
                }
            }
        }

        private void RestoreFadeItem()
        {
            foreach (var kv in items)
            {
                foreach (var item in kv.Value)
                {
                    item.SetFade(false);
                }
            }
        }

        private void OnDragEnd()
        {
            RestoreFadeItem();
            var dragItem = controller.GetControllItem();
            if (dragItem == null)
            {
                return;
            }

            if (!items.TryGetValue(dragItem.resMerge.id, out var v))
            {
                return;
            }

            CpUI_Merge_Item fixedItem = null;

            if (v.Count >= 2)
            {
                var cachedDistance = float.MaxValue;
                foreach (var item in v)
                {
                    if (item == dragItem)
                    {
                        continue;
                    }

                    if (!dragItem.Overlap(item))
                    {
                        continue;
                    }

                    var distance = (dragItem.transform.localPosition - item.transform.localPosition).sqrMagnitude;
                    if (distance > cachedDistance)
                    {
                        continue;
                    }

                    cachedDistance = distance;
                    fixedItem = item;
                }
            }

            if (fixedItem == null)
            {
                RefreshLocate(dragItem.resMerge.id);
                return;
            }

            MergeItem(dragItem, fixedItem, Vector2.Lerp(dragItem.transform.localPosition, fixedItem.transform.localPosition, 0.0f), 0.08f);
        }

        public void AddCellCount(long add)
        {
            cellCount = Math.Min(cellCount + add, GetMaxCell());
            RefreshCell();
        }

        private void RemoveCellCount(long remove)
        {
            cellCount = Math.Max(cellCount - remove, 0);
            RefreshCell();
        }

        private void RefreshCell()
        {
            var max = GetMaxCell();
            circleGage.SetCell(cellCount, max);
        }

        private void RefreshGage()
        {
            var curr = Main.Instance.time.realtimeSinceStartup;
            var speed = GetGageSpeed();
            var fill = speed > 0 ? (curr - gageTime) / (GameData.DEFAULT.MERGE_CHARGE_TIME / speed) : 1f;
            circleGage.SetGageFill(fill);
        }

        private void SetGageTime(float time)
        {
            gageTime = time;
            RefreshGage();
        }

        private void UpdateAutoMerge()
        {
            var curr = Main.Instance.time.realtimeSinceStartup;
            if (autoMergeTime > curr)
            {
                return;
            }

            var speed = GetMergeSpeed();
            autoMergeTime = curr + (speed > 0f ? GameData.DEFAULT.MERGE_NEXT_TIME / speed : 0f);
            if (!Option.isAutoPlay)
            {
                return;
            }

            AutoMerge();
        }

        private void UpdateAutoCreate()
        {
            var curr = Main.Instance.time.realtimeSinceStartup;
            if (autoCreateTime > curr)
            {
                return;
            }

            var speed = GetCreateSpeed();
            autoCreateTime = curr + (speed > 0 ? GameData.DEFAULT.MERGE_CREATE_TIME / speed : 0);
            if (!Option.isAutoPlay)
            {
                return;
            }

            if (cellCount <= 0)
            {
                return;
            }

            CreateItem();
        }

        private void Clear()
        {
            itemPool.Clear();
            items.Clear();
            foreach (var list in items.Values)
            {
                list.Clear();
            }
        }

        public void On()
        {
            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            Debug.Log($"Init : {Option.isAutoPlay}");
            autoPlayToggle.SetOn(Option.isAutoPlay);

            Refresh();
        }

        protected override void RefreshInternal()
        {
            base.RefreshInternal();

            Clear();
            var merges = MyPlayer.Instance.core.merge.GetMerges();
            foreach (var merge in merges)
            {
                for (int i = 0; i < merge.count; ++i)
                {
                    if (!savedAllLocate.TryGetValue(merge.id, out var v))
                    {
                        savedAllLocate.Add(merge.id, v = new List<int>());
                    }

                    if (i >= v.Count)
                    {
                        v.Add(i < merge.locateList.Count ? merge.locateList[i] : Util.ConvertRateToLocate(0.5f, 0.5f));
                    }

                    PopItem(merge.id, v[i]);
                }
            }
        }

        private void AutoMerge()
        {
            var dragItem = controller.GetControllItem();
            foreach (var key in sortedKeys)
            {
                // 드래그중인 아이템과 같은 종류의 아이템이라면 자동 합성 하지 않는다.
                // 합성 하는데 아이템이 사라지면 불편.
                if (dragItem != null && dragItem.resMerge.id == key)
                {
                    continue;
                }

                if (!items.TryGetValue(key, out var v) || v.Count < 2)
                {
                    continue;
                }

                var resMerge = ResourceManager.Instance.merge.GetMerge(key);
                if (resMerge == null)
                {
                    continue;
                }

                // 다음 생성 아이템이 없으면 자동 합성 x
                if (resMerge.nextMergeID == -1)
                {
                    continue;
                }

                CpUI_Merge_Item a = null;
                CpUI_Merge_Item b = null;
                var cachedDistance = float.MaxValue;

                foreach (var item in v)
                {
                    if (a == null)
                    {
                        a = item;
                        continue;
                    }

                    var distance = (a.transform.localPosition - item.transform.localPosition).sqrMagnitude;
                    if (distance > cachedDistance)
                    {
                        continue;
                    }

                    cachedDistance = distance;
                    b = item;
                }

                if (b == null)
                {
                    continue;
                }

                var centerPos = controller.GetCenterPosition();
                var distanceA = (centerPos - (Vector2)a.transform.localPosition).sqrMagnitude;
                var distanceB = (centerPos - (Vector2)b.transform.localPosition).sqrMagnitude;

                MergeItem(a, b, Vector2.Lerp(a.transform.localPosition, b.transform.localPosition, distanceA > distanceB ? UnityEngine.Random.Range(0.05f, 0.5f) : UnityEngine.Random.Range(0.05f, 0.95f)), 0.2f);
                break;
            }
        }

        private void MergeItem(CpUI_Merge_Item itemA, CpUI_Merge_Item itemB, Vector2 pos, float actionTime)
        {
            RemoveItem(itemA);
            RemoveItem(itemB);

            var mergeID = itemA.resMerge.id;
            var locate = Util.ConvertRateToLocate(pos.x / controller.GetSize().x, pos.y / controller.GetSize().y);
            var locateList = RefreshLocate(mergeID);

            itemA.DoCombineAction(pos, actionTime);
            itemB.DoCombineAction(pos, actionTime);

            DelayCall(actionTime, () =>
            {
                VirtualServer.Send(Packet.CREATE_MERGE_NEXT,
                    (arg) =>
                    {
                        if (!VirtualServer.TryGet(arg, out CREATE_MERGE_NEXT tArg))
                        {
                            return;
                        }

                        GameEvent.Instance.AddEvent(GameEventType.CREATE_MERGE_ITEM, tArg.createID, locate);
                        GameEvent.Instance.AddEvent(GameEventType.UPDATE_MERGE, tArg.tmergs);
                        GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                    },
                    mergeID,
                    locateList,
                    locate);
            });
        }

        private void DelayCall(float time, Action call)
        {
            DOTween.To(
                null,
                t => { },
                0f,
                time)
                .From(1f)
                .OnComplete(call.Invoke);
        }

        private void RemoveItem(CpUI_Merge_Item item)
        {
            if (!items.TryGetValue(item.resMerge.id, out var v))
            {
                return;
            }

            v.Remove(item);
        }

        private void CreateItem()
        {
            RemoveCellCount(1);

            VirtualServer.Send(Packet.CREATE_MERGE_NEW,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out CREATE_MERGE_NEW tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.CREATE_MERGE_ITEM, tArg.createID, tArg.locate);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MERGE, tArg.tmergs);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                });
        }

        private CpUI_Merge_Item PopItem(int resID, int locate)
        {
            var resMerge = ResourceManager.Instance.merge.GetMerge(resID);
            if (resMerge == null)
            {
                return null;
            }

            if (!items.TryGetValue(resMerge.id, out var v))
            {
                items.Add(resID, v = new List<CpUI_Merge_Item>());
                sortedKeys.Add(resID);
                sortedKeys.Sort((a, b) => a.CompareTo(b));
            }

            var item = itemPool.Pop();
            item.Set(resMerge);
            item.SetLocate(controller.GetSize(), locate);
            var dragItem = controller.GetControllItem();
            item.SetFade(dragItem == null ? false : dragItem.resMerge.id != item.resMerge.id);

            v.Add(item);

            if (!MyPlayer.Instance.core.profile.IsCompleteGuide(GuideType.MERGE_ITEM))
            {
                if (v.Count >= 2)
                {
                    MyPlayer.Instance.core.profile.ShowGuide(GuideType.MERGE_ITEM);
                }
            }

            return item;
        }

        private float GetCreateSpeed()
        {
            var rise = 1f;
            var mode = ModeManager.Instance.mode;
            if (mode != null)
            {
                var myUnit = mode.core.ally.myUnit;
                if (myUnit != null)
                {
                    rise = myUnit.core.stat.GetValue(eAbility.MERGE_CREATE_SPEED_INC);
                }
            }

            return 1f / Mathf.Max(1f - rise, 0.01f);
        }

        private float GetMergeSpeed()
        {
            var rise = 1f;
            var mode = ModeManager.Instance.mode;
            if (mode != null)
            {
                var myUnit = mode.core.ally.myUnit;
                if (myUnit != null)
                {
                    rise = myUnit.core.stat.GetValue(eAbility.MERGE_NEXT_SPEED_INC);
                }
            }

            return 1f / Mathf.Max(1f - rise, 0.01f);
        }

        public override bool IsFixed()
        {
            return true;
        }

        private void Cmd_Merge()
        {
            AutoMerge();
        }

        private void Cmd_Create()
        {
            if (cellCount <= 0)
            {
                Main.Instance.ShowFloatingMessage("key_ex_merge_craete".L());
                return;
            }

            CreateItem();
        }

        public List<int> RefreshLocate(int mergeID)
        {
            if (!savedAllLocate.TryGetValue(mergeID, out var v))
            {
                savedAllLocate.Add(mergeID, v = new List<int>());
            }

            v.Clear();

            if (items.TryGetValue(mergeID, out var list))
            {
                foreach (var item in list)
                {
                    v.Add(item.locate);
                }
            }

            return v;
        }

        private static long GetMaxCell()
        {
            var mode = ModeManager.Instance.mode;
            if (mode == null)
            {
                return 0;
            }

            var myUnit = mode.core.ally.myUnit;
            if (myUnit == null)
            {
                return 0;
            }

            return myUnit.core.stat.GetLongValue(eAbility.MERGE_CHARGE_CELL);
        }

        private static float GetGageSpeed()
        {
            var rise = 1f;
            var mode = ModeManager.Instance.mode;
            if (mode != null)
            {
                var myUnit = mode.core.ally.myUnit;
                if (myUnit != null)
                {
                    rise = myUnit.core.stat.GetValue(eAbility.MERGE_CHARGE_SPEED_INC);
                }
            }

            return 1f / Mathf.Max(1f - rise, 0.01f);
        }
    }
}