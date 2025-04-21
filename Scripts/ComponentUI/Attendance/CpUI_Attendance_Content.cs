using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UIAttendance
{
    public class CpUI_Attendance_Content : UIBase
    {
        [SerializeField] CpUI_Attendance_Content_Cell cellOrigin = null;
        [SerializeField] GameObject grid = null;
        [SerializeField] GameObject closeButton = null;

        private ObjectPool<CpUI_Attendance_Content_Cell> poolCell = null;

        public void Init(Action onClose)
        {
            poolCell = ObjectPool<CpUI_Attendance_Content_Cell>.Of(cellOrigin, grid, onCreateInit: CreateCellInit);
            Cmd.Add(closeButton, eCmdTrigger.OnClick, onClose);
        }

        private void CreateCellInit(CpUI_Attendance_Content_Cell cell)
        {
            cell.Init();
        }

        public void Set(ResourceAttendance resAttendance)
        {
            poolCell.Clear();
            foreach (var reward in resAttendance.rewards.Values)
            {
                poolCell.Pop().Set(reward);
            }
        }

        public void SetPanelSize(float x, float y)
        {
            if (panel == null)
            {
                return;
            }

            panel.sizeDelta = new Vector2(x, y);
        }
    }
}