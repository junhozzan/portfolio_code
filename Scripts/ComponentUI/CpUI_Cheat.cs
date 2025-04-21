using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UICheat
{
    public class CpUI_Cheat : UIMonoBehaviour, IMenuItem
    {
        private static CpUI_Cheat instance = null;
        public static CpUI_Cheat Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_Cheat>("pf_ui_cheat");
                }

                return instance;
            }
        }

        [SerializeField] InputField inputField = null;

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.LAST, true);
            UsingBlind(false, true);
        }

        public void On()
        {
            UIManager.Instance.Show(this);
            enabled = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Command(inputField.text);
            }
        }

        public void OnCommand()
        {
            Command(inputField.text);
        }

        private void Command(string s)
        {
            var array = s.Split(' ');
            if (array.Length < 2)
            {
                return;
            }

            var cmd = array[0];

            switch (cmd)
            {
                case "모든장비아이템":
                    {
                        var amount = array[1].ToINT();
                        var itmes = ResourceManager.Instance.item.GetItems();
                        foreach (var item in itmes)
                        {
                            if (item.IsAvatar() || item.IsWealth() || item.itemType == ItemType.VIRTUAL)
                            {
                                continue;
                            }

                            var data = User.Instance.item.AddNewItem(item.id, amount);
                        }
                    }
                    break;

                case "item":
                case "아이템":
                    {
                        var id = array[1].ToINT();
                        var amount = array[2].ToINT();
                        var resItem = ResourceManager.Instance.item.GetItem(id);
                        if (resItem == null)
                        {
                            Fail();
                            return;
                        }

                        User.Instance.item.AddNewItem(resItem.id, amount);
                    }
                    break;

                case "스테이지":
                case "stage":

                    break;
            }

            Success();
        }

        private void Fail()
        {
            Main.Instance.ShowFloatingMessage("치트 사용 실패");
        }

        private void Success()
        {
            Main.Instance.ShowFloatingMessage("치트 사용 성공");

            inputField.text = string.Empty;
        }

        void IMenuItem.On(int value)
        {
            On();
        }

        bool IMenuItem.Notice()
        {
            return false;
        }
    }
}