using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIAttendance
{
    public class CpUI_Attendance : UIMonoBehaviour
    {
        private static CpUI_Attendance instance = null;
        public static CpUI_Attendance Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_Attendance>("pf_ui_attendance");
                }

                return instance;
            }
        }

        [SerializeField] CpUI_Attendance_Content day28 = null;
        [SerializeField] UIText titleText = null;

        private ResourceAttendance resAttendance = null;
        private Queue<int> opendIDs = new Queue<int>();

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.CONTENTS, true);
            UsingBlind(false);
            UsingUpdate();

            day28.Init(ClearOrClose);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, IsActive)
                .Add(GameEventType.UPDATE_ATTENDANCE, Refresh);
        }

        private void On(int resID)
        {
            var res = ResourceManager.Instance.attendance.GetAttendance(resID);
            if (res == null)
            {
                return;
            }

            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            resAttendance = res;

            RefreshTitleText();
            RefreshPanelSize();
            RefreshContent();
        }

        public void On(IList<int> _opendIDs)
        {
            if (_opendIDs.Count == 0)
            {
                return;
            }

            opendIDs.Clear();
            foreach (var id in _opendIDs)
            {
                opendIDs.Enqueue(id);
            }

            AutoNextAttendance();
        }

        public override bool CanClose()
        {
            if (MyPlayer.Instance.core.attendance.IsExistDayReward(resAttendance.id))
            {
                AllClearAttendance();
                return false;
            }

            return true;
        }

        protected override void RefreshInternal()
        {
            RefreshContent();
        }

        private void RefreshTitleText()
        {
            if (titleText == null)
            {
                return;
            }

            titleText.SetText(resAttendance.GetName());
        }

        private void RefreshPanelSize()
        {
            day28.SetPanelSize(resAttendance.panelSizeX, resAttendance.panelSizeY);
        }

        private void RefreshContent()
        {
            var content = GetContent();
            if (content == null)
            {
                return;
            }

            content.Set(resAttendance);
        }

        private CpUI_Attendance_Content GetContent()
        {
            return day28;
        }

        private void AllClearAttendance()
        {
            MyPlayer.Instance.core.attendance.AllClearAttendance(resAttendance.id);
        }

        private void ClearOrClose()
        {
            ClickSound();

            if (MyPlayer.Instance.core.attendance.IsExistDayReward(resAttendance.id))
            {
                AllClearAttendance();
            }
            else
            {
                Off();
            }
        }

        public override void CloseEvent()
        {
            AutoNextAttendance();
        }

        private void AutoNextAttendance()
        {
            if (opendIDs.Count == 0)
            {
                return;
            }

            On(opendIDs.Dequeue());
        }
    }
}