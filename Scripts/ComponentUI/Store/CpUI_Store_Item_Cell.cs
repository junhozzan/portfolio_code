using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UIStore
{
    public class CpUI_Store_Item_Cell : MyOSAGridItem
    {
        [SerializeField] GameObject button = null;
        [SerializeField] UIText purchaseCount = null;
        [SerializeField] UIText nameText = null;
        [SerializeField] UIText amountText = null;
        [SerializeField] UIText costText = null;
        [SerializeField] UIImage costIcon = null;
        [SerializeField] GameObject disableBlind = null;

        private Action<ResourceStoreItem> onClick = null;
        private ResourceStoreItem resItem = null;

        public void Init(Action<ResourceStoreItem> onClick)
        {
            this.onClick = onClick;

            Cmd.Add(button, eCmdTrigger.OnClick, Cmd_OnClick);
        }

        public override void DoReset()
        {
            resItem = null;
            SetDefault();
        }

        private void SetDefault()
        {
            costIcon.gameObject.SetActive(false);
            purchaseCount.gameObject.SetActive(false);
        }

        private void Update()
        {
            RefreshCostText();
        }

        public override void Refresh()
        {
            RefreshNameText();
            RefreshAmountText();
            RefreshCostText();
            RefreshCostIcon();
            RefreshPurchaseCount();
            RefreshDisableBlind();
        }

        private void RefreshNameText()
        {
            if (resItem == null)
            {
                return;
            }

            nameText.SetText(resItem.GetName());
        }

        private void RefreshAmountText()
        {
            if (resItem == null)
            {
                return;
            }

            amountText.SetText($"{resItem.amountText}{resItem.amountTextKey.L()}" );
            amountText.SetTextColor(GameData.COLOR.STORE_GET_COUNT);
        }

        private void RefreshCostIcon()
        {
            if (resItem == null)
            {
                return;
            }

            var resWealthItem = ResourceManager.Instance.item.GetItem(resItem.costItemID);
            if (resWealthItem == null || !resWealthItem.appearance.use)
            {
                return;
            }

            costIcon.gameObject.SetActive(true);
            costIcon.SetSprite(resWealthItem.appearance.atlas, resWealthItem.appearance.sprite);
        }

        private void RefreshCostText()
        {
            if (resItem == null)
            {
                return;
            }

            if (resItem.IsAdReward())
            {
                var resAd = ResourceManager.Instance.ad.GetAd(resItem.adID);
                if (resAd != null)
                {
                    var ad = MyPlayer.Instance.core.ad.GetAd(resAd.id);
                    var current = Main.Instance.time.nowToEpochSecond();
                    if (ad.coolTime > current)
                    {
                        var sTime = Util.SecToTimer2(ad.coolTime - current);
                        costText.SetText(sTime);
                    }
                    else
                    {
                        costText.SetText("key_show_ad".L());
                    }

                    costText.SetTextColor(Color.white);
                }
            }
            else
            {
                if (resItem.costValue > 0)
                {
                    var amount = MyPlayer.Instance.core.item.GetAmount(resItem.costItemID);
                    costText.SetText(resItem.costValue.ToString("#,##0"));
                    costText.SetTextColor(amount >= resItem.costValue ? Color.white : GameData.COLOR.RED_FONT);
                }
                else
                {
                    costText.SetText("key_free".L());
                    costText.SetTextColor(Color.white);
                }
            }

            //costText.SetFitterHorizontal();
        }

        private void RefreshPurchaseCount()
        {
            if (resItem == null || resItem.IsLimitFree())
            {
                return;
            }

            var store = MyPlayer.Instance.core.store.GetStore(resItem.id);
            purchaseCount.gameObject.SetActive(true);
            purchaseCount.SetText($"{resItem.GetLimitString()} {resItem.limitCount - store.GetPurchaseCount()}/{resItem.limitCount}");
            purchaseCount.SetTextColor(GameData.COLOR.STORE_LIMIT_STRING);
        }

        private void RefreshDisableBlind()
        {
            if (resItem == null)
            {
                return;
            }

            if (resItem.IsLimitFree())
            {
                disableBlind.SetActive(false);
                return;
            }

            var store = MyPlayer.Instance.core.store.GetStore(resItem.id);
            disableBlind.SetActive(store.GetPurchaseCount() >= resItem.limitCount);
        }

        public override void UpdateViews(MyOSAGrid.IOsaItem tOsaItem)
        {
            var osaItem = tOsaItem as CpUI_Store.ItemCellOSAItem;
            if (osaItem == null)
            {
                return;
            }

            resItem = osaItem.resStoreItem;
            Refresh();
        }

        public override bool IsEmpty()
        {
            return resItem == null;
        }

        private void Cmd_OnClick()
        {
            UIBase.ClickSound();

            if (disableBlind.activeSelf)
            {
                Main.Instance.ShowFloatingMessage("key_ex_over_purchase".L());
                return;
            }

            if (resItem == null)
            {
                return;
            }

            onClick?.Invoke(resItem);
        }
    }
}