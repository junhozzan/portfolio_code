using MissionKey;
using UnityEngine;
namespace UIMission
{
    public class CpUI_Mission_Cell : MyOSABasicItem
    {
        [SerializeField] UIText remainTimeText = null;
        [SerializeField] UIText titleText = null;
        [SerializeField] UIText descText = null;
        [SerializeField] UIText progressText = null;
        [SerializeField] UIText clearButtonText = null;
        [SerializeField] UISlider progressSlider = null;
        [SerializeField] CpUI_ItemFrame rewardItemOrigin = null;
        [SerializeField] GameObject completeMark = null;
        [SerializeField] GameObject clearButton = null;

        private ObjectPool<CpUI_ItemFrame> rewardItemPool = null;
        private ResourceMission resMission = null;
        private Cmd cmdClear = null;

        public void Init()
        {
            rewardItemPool = ObjectPool<CpUI_ItemFrame>.Of(rewardItemOrigin, rewardItemOrigin.transform.parent);

            cmdClear = Cmd.Add(clearButton, eCmdTrigger.OnClick, Cmd_Clear)
                .SetOnDisable(Cmd_DisableClear);
        }

        public override void DoReset()
        {

        }

        public override void Refresh()
        {
            RefreshTitleText();
            RefreshDescText();
            RefreshProgress();
            RefreshRemainTimeText();
            RefreshState();
            RefreshRewardItems();
        }

        private void RefreshTitleText()
        {
            if (titleText == null)
            {
                return;
            }

            titleText.SetText($"[{resMission.GetName()}]");
        }

        private void RefreshDescText()
        {
            if (descText == null)
            {
                return;
            }

            var tmission = MyPlayer.Instance.core.mission.GetMission(resMission.id);
            var level = tmission != null ? tmission.GetLevel() : 0;
            var maxPoint = resMission.GetMaxPoint(level);
            descText.SetText(resMission.desc.L(maxPoint));
        }

        private void RefreshProgress()
        {
            if (progressText == null)
            {
                return;
            }

            var tmission = MyPlayer.Instance.core.mission.GetMission(resMission.id);
            if (tmission == null)
            {
                return;
            }

            var maxPoint = resMission.GetMaxPoint(tmission.GetLevel());
            var curPoint = MyPlayer.Instance.core.mission.IsCompleted(resMission.id) ? maxPoint : tmission.GetValue();

            progressText.SetText($"{CustomValueText(curPoint)}/{CustomValueText(maxPoint)}");
            progressSlider.SetFill(curPoint / (float)maxPoint);
        }

        private void RefreshRemainTimeText()
        {
            if (remainTimeText == null)
            {
                return;
            }

            remainTimeText.SetText("");
        }

        private void RefreshRewardItems()
        {
            rewardItemPool.Clear();
            
            if (MyPlayer.Instance.core.mission.IsCompleted(resMission.id))
            {
                return;
            }

            foreach (var packID in resMission.packIDs)
            {
                var resPack = ResourceManager.Instance.pack.GetPack(packID);
                if (resPack == null)
                {
                    continue;
                }

                var obj = rewardItemPool.Pop();
                obj.SetDefault();
                obj.Set(resPack, true);
            }
        }

        private void RefreshState()
        {
            completeMark.SetActive(false);
            clearButton.SetActive(false);
            cmdClear.Use(false);

            // 임무 완료
            if (MyPlayer.Instance.core.mission.IsCompleted(resMission.id))
            {
                completeMark.SetActive(true);
                return;
            }

            clearButton.SetActive(true);

            // 임무 클리어 가능
            if (MyPlayer.Instance.core.mission.IsClearable(resMission.id))
            {
                cmdClear.Use(true);
                clearButtonText.SetText("key_complete".L());
            }
            
            // 진행중
            else
            {
                clearButtonText.SetText("key_playing".L());
            }
        }

        public override void UpdateViews(MyOSABasic.IOsaItem tOsaItem)
        {
            if (!(tOsaItem is CpUI_Mission.MissionOsaItem osaItem))
            {
                return;
            }

            resMission = osaItem.resMission;

            Refresh();
        }

        private void Cmd_Clear()
        {
            UIBase.ClickSound();
            MyPlayer.Instance.core.mission.ClearMission(resMission.id);
        }

        private void Cmd_DisableClear()
        {

        }

        private string CustomValueText(long v)
        {
            return v.ToString();
        }
    }
}