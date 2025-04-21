using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIAttendance
{
    public class CpUI_Attendance_Content_Cell : UIBase
    {
        [SerializeField] CpUI_ItemFrame itemFrame = null;
        [SerializeField] UIText dayText = null;
        [SerializeField] GameObject ableMark = null;
        [SerializeField] GameObject completeMark = null;
        [SerializeField] GameObject button = null;

        private Cmd cmdClickCell = null;
        private ResourceAttendance.Reward reward = null;

        public void Init()
        {
            cmdClickCell = Cmd.Add(button, eCmdTrigger.OnClick, Cmd_ClickCell);
        }

        public void Set(ResourceAttendance.Reward reward)
        {
            this.reward = reward;

            RefreshCell();
            RefreshDayText();
        }

        private void RefreshIcon(bool showTooltip)
        {
            var resPack = ResourceManager.Instance.pack.GetPack(reward.packID);
            if (resPack == null)
            {
                return;
            }

            itemFrame.SetDefault();
            itemFrame.Set(resPack, showTooltip);
        }

        private void RefreshDayText()
        {
            dayText.SetText(reward.day.ToString());
        }

        private void RefreshCell()
        {
            if (reward == null)
            {
                return;
            }

            var attendance = MyPlayer.Instance.core.attendance.GetAttendance(reward.attendanceID);
            if (attendance == null)
            {
                return;
            }

            if (reward.day <= attendance.GetCount())
            {
                var complete = (attendance.GetFlag() & reward.flag) == reward.flag;

                cmdClickCell.Use(!complete);
                ableMark.SetActive(!complete);
                completeMark.SetActive(complete);

            }
            else
            {
                cmdClickCell.Use(false);
                ableMark.SetActive(false);
                completeMark.SetActive(false);
            }

            RefreshIcon(!ableMark.activeSelf);
        }

        private void Cmd_ClickCell()
        {
            MyPlayer.Instance.core.attendance.ClearAttendance(reward.attendanceID, reward.day);
        }
    }
}