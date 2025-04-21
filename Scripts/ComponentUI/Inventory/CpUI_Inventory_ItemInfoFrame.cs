using UnityEngine;

namespace UIInventory
{
    public class CpUI_Inventory_ItemInfoFrame : UIBase
    {
        [SerializeField] CpUI_ItemFrame itemFrame = null;
        [SerializeField] UIText nameText = null;
        [SerializeField] CpUI_ItemStatInfoSection statSection = null;
        [SerializeField] GameObject equipMark = null;
        [SerializeField] GameObject unownedMark = null;

        [SerializeField] bool useButtons = true;
        [SerializeField] GameObject buttons = null;
        [SerializeField] GameObject equipButton = null;
        [SerializeField] GameObject releaseButton = null;
        [SerializeField] GameObject openItemUtilButton = null;
        [SerializeField] GameObject dismantleButton = null;

        private TItem item = null;

        public void Init()
        {
            statSection.Init();

            if (useButtons)
            {
                Cmd.Add(equipButton, eCmdTrigger.OnClick, Cmd_EquipItem);
                Cmd.Add(releaseButton, eCmdTrigger.OnClick, Cmd_ReleaseItem);
                Cmd.Add(openItemUtilButton, eCmdTrigger.OnClick, Cmd_OpenItemUtilPopup);
                Cmd.Add(dismantleButton, eCmdTrigger.OnClick, Cmd_Dismantle);
            }
        }

        public void SetDefault()
        {
            itemFrame.SetDefault();
            equipMark.SetActive(false);
            unownedMark.SetActive(false);
            buttons.SetActive(false);
            equipButton.SetActive(false);
            releaseButton.SetActive(false);
            openItemUtilButton.SetActive(false);
            dismantleButton.SetActive(false);
        }

        public void Refresh(TItem _item)
        {
            item = _item;

            SetDefault();
            RefreshItemSprite();
            RefreshNameText();
            RefreshUnownedMark();
            RefreshEquipMark();
            RefreshButtons();
            RefreshStatInfo();
        }

        private void RefreshStatInfo()
        {
            statSection.Refresh(item);
        }

        private void RefreshItemSprite()
        {
            var resItem = item.resItem;
            if (resItem == null)
            {
                return;
            }

            itemFrame.Set(item.resItem, false);
        }

        private void RefreshNameText()
        {
            var resItem = item.resItem;
            if (resItem == null)
            {
                return;
            }

            nameText.SetText(resItem.GetName());
            nameText.SetTextColor(resItem.GetGradeColor());
        }

        private void RefreshUnownedMark()
        {
            unownedMark.SetActive(!MyPlayer.Instance.core.item.TryGetItem(item.id, out var _));
        }

        private void RefreshEquipMark()
        {
            equipMark.SetActive(MyPlayer.Instance.core.inventory.IsEquipedItem(item.id));
        }

        public void RefreshButtons()
        {
            if (!useButtons)
            {
                return;
            }

            RefreshUtilButton();
            RefreshEquipButtons();
            //RefreshDismantleButton();

            if (openItemUtilButton.activeSelf
                || equipButton.activeSelf
                || releaseButton.activeSelf
                || dismantleButton.activeSelf)
            {
                buttons.SetActive(true);
            }
        }

        private void RefreshUtilButton()
        {
            var resItem = item.resItem;
            if (resItem == null)
            {
                return;
            }

            if (!resItem.enhance.use && !resItem.awaken.use && !resItem.reroll.use)
            {
                return;
            }

            openItemUtilButton.SetActive(true);
        }

        private void RefreshEquipButtons()
        {
            var resItem = item.resItem;
            if (resItem == null)
            {
                return;
            }

            if (!resItem.equip.use)
            {
                return;
            }

            var button = MyPlayer.Instance.core.inventory.IsEquipedItem(item.id) ? releaseButton : equipButton;
            button.SetActive(true);
        }

        private void RefreshDismantleButton()
        {
            var resItem = item.resItem;
            if (resItem == null)
            {
                return;
            }

            if (!resItem.dismantle.use)
            {
                return;
            }

            if (item.GetAmount() == 0)
            {
                return;
            }

            dismantleButton.SetActive(true);
        }

        private void Cmd_EquipItem()
        {
            ClickSound();

            MyPlayer.Instance.core.inventory.EquipItem(item.id);
        }

        private void Cmd_ReleaseItem()
        {
            ClickSound();
            
            MyPlayer.Instance.core.inventory.ReleaseItem(item.id);
        }

        private void Cmd_OpenItemUtilPopup()
        {
            ClickSound();

            PopupExtend.Instance.ShowItemUtil(item.id);
        }

        private void Cmd_Dismantle()
        {
            ClickSound();

            PopupExtend.Instance.ShowDismantleItem(item.id);
        }
    }
}