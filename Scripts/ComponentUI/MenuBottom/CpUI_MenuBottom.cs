using UnityEngine;

namespace UIMenu_Bottom
{
    public class CpUI_MenuBottom : UIMonoBehaviour
    {
        private static CpUI_MenuBottom instance = null;
        public static CpUI_MenuBottom Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_MenuBottom>("pf_ui_menu_bottom");
                }

                return instance;
            }
        }

        [SerializeField] UIButton mergeButton = null;
        [SerializeField] UIButton invenButton = null;
        [SerializeField] UIButton labButton = null;
        [SerializeField] UIRadio radio = null;

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.BASE, true);

            mergeButton.Init(Cmd_OpenMerge);
            invenButton.Init(Cmd_OpenInven);
            labButton.Init(Cmd_OpenLab);
        }

        public void On()
        {
            if (!UIManager.Instance.Show(this))
            {
                return;
            }

            radio.Choice(mergeButton.gameObject);

            Cmd_OpenMerge();
        }

        public override bool IsFixed()
        {
            return true;
        }

        private void Cmd_OpenMerge()
        {
            UIMerge.CpUI_Merge.Instance.On();
            UIInventory.CpUI_Inventory.Instance.Off();
            UILab.CpUI_Lab.Instance.Off();
        }

        private void Cmd_OpenInven()
        {
            UIInventory.CpUI_Inventory.Instance.On();
            UILab.CpUI_Lab.Instance.Off();
        }

        private void Cmd_OpenLab()
        {
            UILab.CpUI_Lab.Instance.On();
            UIInventory.CpUI_Inventory.Instance.Off();
        }
    }
}