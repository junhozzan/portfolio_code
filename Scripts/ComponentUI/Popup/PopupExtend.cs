using System.Collections.Generic;
using System.Text;
using UIPopup;
using System;

public class PopupExtend : Singleton<PopupExtend>
{
    private readonly StringBuilder sb = new StringBuilder();

    public void Initialize()
    {
        CraeteHandler();
    }

    private void CraeteHandler()
    {
        GameEvent.Instance.CreateHandler(this, null)
            .Add(GameEventType.SHOW_GET_INFO, Handle_SHOW_GET_INFO)
            ;
    }

    private void Handle_SHOW_GET_INFO(object[] args)
    {
        var getInfos = GameEvent.GetSafe<IList<GetInfo>>(args, 0);
        if (getInfos == null)
        {
            return;
        }

        var callback = GameEvent.GetSafe<Action>(args, 1);

        ShowGetItemsPopup(getInfos, callback);
    }

    private CpUI_PopupFrame_GetItems ShowGetItemsPopup(IList<GetInfo> getInfos, Action callback)
    {
        if (getInfos.Count == 0)
        {
            return null;
        }

        var popup = CpUI_Popup.Instance.Show<CpUI_PopupFrame_GetItems>();
        popup.SetContents(getInfos);
        popup.SetButton().SetTextKey("key_ok")
            .SetOnActive(() =>
            {
                callback?.Invoke();
            });

        return popup;
    }

    public CpUI_PopupFrame_BasicText ShowByPack(int packID, bool isAdReward)
    {
        sb.Clear();

        var popup = CpUI_Popup.Instance.Show<CpUI_PopupFrame_BasicText>();
        popup.SetTitle("key_item_appear_info".L());

        var resPack = ResourceManager.Instance.pack.GetPack(packID);
        if (resPack != null)
        {
            if (resPack.fixedMembers.Count > 0)
            {
                sb.Append($"<color=#{GameData.COLOR.GET_CHANCE_TITLE.hex}>[{"key_ex_fixed_appear_item".L()}]</color>");
                sb.Append("\n");

                foreach (var m in resPack.fixedMembers)
                {
                    IPackView packView = resPack.packItemType == PackItemType.ITEM ?
                        ResourceManager.Instance.item.GetItem(m.id)
                        : ResourceManager.Instance.pack.GetPack(m.id);

                    if (packView == null)
                    {
                        continue;
                    }

                    sb.Append($"\n<color=#{packView.GetGradeColor().hex}>{packView.GetName()}</color> {m.amount.ToString("#,###")}");
                }
            }

            if (resPack.randomMembers.Count > 0)
            {
                sb.Append($"<color=#{GameData.COLOR.GET_CHANCE_TITLE.hex}>[{"key_ex_chance_appear_item".L()}]</color>");
                sb.Append("\n");

                foreach (var m in resPack.randomMembers)
                {
                    IPackView packView = resPack.packItemType == PackItemType.ITEM ?
                        ResourceManager.Instance.item.GetItem(m.id)
                        : ResourceManager.Instance.pack.GetPack(m.id);
                    if (packView == null)
                    {
                        continue;
                    }

                    sb.Append($"\n<color=#{packView.GetGradeColor().hex}>{packView.GetName()}</color> <color=#{GameData.COLOR.GET_CHANCE.hex}>{(float)m.appear / resPack.maxRandomAppear * 100f:N2}%</color>");
                }
            }
        }

        if (isAdReward)
        {
            sb.Append($"\n\n<color=#{GameData.COLOR.ITEM_SPECIAL_TEXT.hex}>{"key_ex_ad_before_reward".L()}</color>");
        }

        popup.SetContent(sb.ToString());

        return popup;
    }

    public CpUI_PopupFrame_ItemUtil ShowItemUtil(int resItemID)
    {
        return CpUI_Popup.Instance.Show<CpUI_PopupFrame_ItemUtil>().On(resItemID);
    }

    public CpUI_PopupFrame_DismantleItem ShowDismantleItem(int resItemID)
    {
        var popup = CpUI_Popup.Instance.Show<CpUI_PopupFrame_DismantleItem>().On(resItemID);
        popup.SetButton().SetTextKey("key_cancel");
        popup.SetButton().SetTextKey("key_dismantle")
            .SetBackground(GameData.COLOR.OK_BUTTON, GameData.COLOR.OK_BUTTON_FRAME)
            .SetOnActive(() =>
            {
                MyPlayer.Instance.core.inventory.DismantleItem(resItemID, popup.GetAmount());
            });

        return popup;
    }

    public CpUI_PopupFrame_InputNickName ShowInputNickName()
    {
        var popup = CpUI_Popup.Instance.Show<CpUI_PopupFrame_InputNickName>();
        return popup;
    }

    public CpUI_PopupFrame_Dictionary ShowDictionary()
    {
        var popup = CpUI_Popup.Instance.Show<CpUI_PopupFrame_Dictionary>();
        popup.SetButton().SetTextKey("key_ok");

        return popup;
    }

    public CpUI_PopupFrame_BasicText ShowQuit(Action quitCallback = null)
    {
        var popup = CpUI_Popup.Instance.Show<CpUI_PopupFrame_BasicText>();
        popup.SetTitle("key_notice".L());
        popup.SetContent("key_ex_exit".L());
        popup.SetButton().SetTextKey("key_cancel");
        popup.SetButton().SetTextKey("key_ok")
            .SetBackground(GameData.COLOR.OK_BUTTON, GameData.COLOR.OK_BUTTON_FRAME)
            .SetOnActive(() =>
            {
                quitCallback?.Invoke();
                UnityEngine.Application.Quit();
            });

        return popup;
    }

    public CpUI_PopupFrame_BasicText ShowRankRegister(ResourceAdvertisement resAd)
    {
        var popup = CpUI_Popup.Instance.Show<CpUI_PopupFrame_BasicText>();
        popup.SetTitle("key_notice".L());
        var content = resAd.isShow ? 
            $"{"key_ex_rank_enroll_explain_ad".L()}\n\n<color=#{GameData.COLOR.ITEM_SPECIAL_TEXT.hex}>{"key_ex_ad_before_reward".L()}</color>"
            : "key_ex_rank_enroll_explain".L();

        popup.SetContent(content);
        popup.SetButton().SetTextKey("key_cancel");

        return popup;
    }

    public CpUI_PopupFrame_BasicText ShowItemRerollMaxCheckPopup()
    {
        var popup = CpUI_Popup.Instance.Show<CpUI_PopupFrame_BasicText>();
        popup.SetTitle("key_caution".L());
        popup.SetContent("key_ex_reroll".L());
        popup.SetButton().SetTextKey("key_cancel");

        return popup;
    }

    public CpUI_PopupFrame_BasicText ShowVersionChecker(string productVersion)
    {
        var content = $"{"key_ex_version_check".L()}\n\n<color=#{GameData.COLOR.WARNING.hex}>{"key_ex_id_caution".L()}</color>";

        var popup = CpUI_Popup.Instance.Show<CpUI_PopupFrame_BasicText>();
        popup.SetTitle("key_notice".L());
        popup.SetContent(content);
        popup.SetButton().SetTextKey("key_ok")
            .SetOnActive(() => 
            {
                Option.checkedVersion = productVersion;
                Option.Save();
            });
#if UNITY_ANDROID
        if (!string.IsNullOrEmpty(GameData.DEFAULT.GOOGLE_STORE_LINK))
        {
            popup.SetButton().SetTextKey("key_move")
                .SetBackground(GameData.COLOR.OK_BUTTON, GameData.COLOR.OK_BUTTON_FRAME)
                .SetOnActive(() =>
                {
                    // 스토어 링크
                    UnityEngine.Application.OpenURL(GameData.DEFAULT.GOOGLE_STORE_LINK);
                    Option.checkedVersion = productVersion;
                    Option.Save();
                });
        }
#endif

        return popup;
    }

    public CpUI_PopupFrame_Option ShowOption()
    {
        var popup = CpUI_Popup.Instance.Show<CpUI_PopupFrame_Option>();
        popup.SetButton().SetTextKey("key_close");

        return popup;
    }

    public CpUI_PopupFrame_Language ShowLanguage(Action closeEvent)
    {
        var popup = CpUI_Popup.Instance.Show<CpUI_PopupFrame_Language>().On(closeEvent);
        popup.SetButton().SetTextKey("key_close");

        return popup;
    }

    public CpUI_PopupFrame_ItemInfo ShowItemInfo(int itemID)
    {
        var popup = CpUI_Popup.Instance.Show<CpUI_PopupFrame_ItemInfo>().On(itemID);
        popup.SetTitle("key_item_info".L());
        popup.SetButton().SetTextKey("key_ok");

        return popup;
    }

    public CpUI_PopupFrame_PlatformSelect ShowPlatformSelectByLogin(Action<PlatformType> onSelect)
    {
        return CpUI_Popup.Instance.Show<CpUI_PopupFrame_PlatformSelect>()
            .OnByLogin()
            .SetOnSelect(onSelect);
    }

    public CpUI_PopupFrame_PlatformSelect ShowPlatformSelectByPlaying(Action<PlatformType> onSelect)
    {
        var popup = CpUI_Popup.Instance.Show<CpUI_PopupFrame_PlatformSelect>()
            .OnByPlaying()
            .SetOnSelect(onSelect);
        popup.SetButton().SetTextKey("key_cancel");

        return popup;
    }
}
