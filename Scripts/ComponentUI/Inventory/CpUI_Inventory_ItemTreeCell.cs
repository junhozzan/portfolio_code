using UnityEngine;

namespace UIInventory
{
    public class CpUI_Inventory_ItemTreeCell : MyOSABasicItem
    {
        [SerializeField] CpUI_ItemFrame itemFrame = null;
        [SerializeField] UIText nameText = null;
        [SerializeField] UIText amountText = null;
        [SerializeField] UIText levelText = null;
        [SerializeField] UIImage bg = null;
        [SerializeField] UIImage expGage = null;
        [SerializeField] GameObject selected = null;
        [SerializeField] GameObject equip = null;
        [SerializeField] GameObject unowned = null;

        private IUInventory uiInventory = null;
        private ResourceItem resItem = null;

        public void Init(IUInventory uiInventory)
        {
            this.uiInventory = uiInventory;

            Cmd.Add(gameObject, eCmdTrigger.OnClick, Cmd_SelectCell);
        }

        public override void DoReset()
        {
            resItem = null;
            itemFrame.SetDefault();
        }

        public override void Refresh()
        {
            RefreshItemFrame();
            RefreshNameText();
            RefreshAmountText();
            RefreshLevelText();
            RefreshSelected();
            RefreshEquipMark();
            RefreshUnownedMark();
            RefresExpGage();
        }

        private void RefreshItemFrame()
        {
            itemFrame.Set(resItem, false);

            //if (!resItem.IsAvatar())
            //{
            //    itemFrame.SetLevelText($"{(MyPlayer.Instance.core.item.TryGetItem(resItem.id, out var item) ? item.GetLevel() : 0)}");
            //}
        }

        private void RefreshNameText()
        {
            nameText.SetText(resItem.GetName());
            nameText.SetTextColor(resItem.GetGradeColor());
        }

        private void RefreshAmountText()
        {
            var amount = MyPlayer.Instance.core.item.TryGetItem(resItem.id, out var item) ? item.GetAmount() : 0;
            amountText.SetText($"{"key_amount".L()}: {Util.ToComma(amount)}");
        }

        private void RefreshLevelText()
        {
            if (levelText == null)
            {
                return;
            }

            if (!MyPlayer.Instance.core.item.TryGetItem(resItem.id, out var item))
            {
                levelText.gameObject.SetActive(false);
                return;
            }

            levelText.gameObject.SetActive(true);
            levelText.SetText($"Lv.{item.GetLevel()}");
        }

        private void RefreshSelected()
        {
            selected.SetActive(uiInventory.GetInventory().IsSelectedItem(resItem.id));
        }

        private void RefreshEquipMark()
        {
            equip.SetActive(MyPlayer.Instance.core.inventory.IsEquipedItem(resItem.id));
        }

        private void RefreshUnownedMark()
        {
            unowned.SetActive(!MyPlayer.Instance.core.item.TryGetItem(resItem.id, out var _));
        }

        private void RefresExpGage()
        {
            if (expGage == null)
            {
                return;
            }

            if (!MyPlayer.Instance.core.item.TryGetItem(resItem.id, out var item))
            {
                expGage.gameObject.SetActive(false);
                return;
            }

            expGage.gameObject.SetActive(true);
            expGage.SetFillAmount(resItem.GetExpToRate(item.GetExp()));
        }

        public override void UpdateViews(MyOSABasic.IOsaItem tOsaItem)
        {
            if (!(tOsaItem is CpUI_Inventory_ItemTree.ItemOsaItem osaItem))
            {
                return;
            }

            resItem = osaItem.resItem;
            Refresh();
        }

        private void Cmd_SelectCell()
        {
            SoundManager.Instance.PlaySfx(GameData.SOUND.SFX_ITEM_SELECT);
            uiInventory.GetInventory().SetSelectItem(resItem.id);
            uiInventory.RefreshExternal();
        }
    }
}