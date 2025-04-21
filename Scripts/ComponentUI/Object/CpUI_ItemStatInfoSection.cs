using System.Collections.Generic;
using UnityEngine;

public class CpUI_ItemStatInfoSection : UIBase
{
    [SerializeField] UIText textOrigin = null;

    private ObjectPool<UIText> textPool = null;
    private readonly List<TextParam> tempTextParams = new List<TextParam>(16);

    public void Init()
    {
        textPool = ObjectPool<UIText>.Of(textOrigin, textOrigin.transform.parent);
    }

    public void Refresh(TItem item)
    {
        textPool.Clear();

        ShowInfo(item);
        ShowEquipAbilty(item);
        ShowHoldAbility(item);
        ShowSpecialText(item);
        ShowSetOption(item);
    }

    private void ShowInfo(TItem item)
    {
        var resItem = item.resItem;
        if (resItem == null)
        {
            return;
        }

        var maxLevel = resItem.GetMaxLevel(item.GetAwaken());
        if (maxLevel > 0)
        {
            SetText("key_level".L(), GameData.COLOR.ITEM_INFO_MAIN);
            SetText($"Lv.{item.GetLevel()}/{resItem.GetMaxLevel(item.GetAwaken())} ({"key_awaken".L()} Lv.{item.GetAwaken()})", Color.white);
        }

        if (resItem.equipPoint > 0)
        {
            SetText(string.Empty, Color.white);
            SetText("key_need_equip_point".L(), GameData.COLOR.ITEM_INFO_MAIN);
            SetText(resItem.equipPoint.ToString(), Color.white);
        }
    }

    private void ShowEquipAbilty(TItem item)
    {
        var resItem = item.resItem;
        if (resItem == null)
        {
            return;
        }

        tempTextParams.Clear();

        var itemLevel = item.GetLevel();
        var itemAmount = item.GetAmount();
        var itemOptions = item.GetOptions();
        var riseParam = StatItem.Param.RiseParam.Of(itemLevel, itemAmount);
        foreach (var ability in resItem.equip.targetAbilities)
        {
            foreach (var statItem in ability.statItems)
            {
                var opt = statItem.GetOption(itemOptions);
                var statString = statItem.ToString(riseParam, opt);
                var s = opt >= 1f ? $"{statString} <color=#{GameData.COLOR.STAT_MAX.hex}>(Max)</color>" : statString;

                tempTextParams.Add(TextParam.Of(Color.white, s));
            }
        }

        if (tempTextParams.Count == 0)
        {
            return;
        }

        SetText(string.Empty, Color.white);
        SetText("key_equip_ability".L(), GameData.COLOR.ITEM_INFO_MAIN);
        foreach (var param in tempTextParams)
        {
            SetText(param.text, param.color);
        }
    }

    private void ShowHoldAbility(TItem item)
    {
        var resItem = item.resItem;
        if (resItem == null)
        {
            return;
        }

        tempTextParams.Clear();

        var itemLevel = item.GetLevel();
        var itemAmount = item.GetAmount();
        var itemOptions = item.GetOptions();
        var riseParam = StatItem.Param.RiseParam.Of(itemLevel, itemAmount);
        foreach (var ability in resItem.hold.targetAbilities)
        {
            foreach (var statItem in ability.statItems)
            {
                var opt = statItem.GetOption(itemOptions);
                var statString = statItem.ToString(riseParam, opt);
                var s = opt >= 1f ? $"{statString} <color=#{GameData.COLOR.STAT_MAX.hex}>(Max)</color>" : statString;

                tempTextParams.Add(TextParam.Of(Color.white, s));
            }
        }

        if (tempTextParams.Count == 0)
        {
            return;
        }

        SetText(string.Empty, Color.white);
        SetText("key_hold_ability".L(), GameData.COLOR.ITEM_INFO_MAIN);
        foreach (var param in tempTextParams)
        {
            SetText(param.text, param.color);
        }
    }

    private void ShowSpecialText(TItem item)
    {
        var resItem = item.resItem;
        if (resItem == null)
        {
            return;
        }

        if (resItem.appearance.specialTexts.Count == 0 && resItem.appearance.levelToSpecialTexts.Count == 0)
        {
            return;
        }

        SetText(string.Empty, Color.white);

        if (resItem.appearance.TryGetLevelToSpecialText(item.GetLevel(), out var v))
        {
            SetText(v.L(), GameData.COLOR.ITEM_SPECIAL_TEXT);
        }

        foreach (var s in resItem.appearance.specialTexts)
        {
            SetText(s.L(), GameData.COLOR.ITEM_SPECIAL_TEXT);
        }
    }

    private void ShowSetOption(TItem item)
    {
        var resItem = item.resItem;
        if (resItem == null)
        {
            return;
        }

        var resItemSet = ResourceManager.Instance.item.GetItemSet(resItem.itemSetID);
        if (resItemSet == null)
        {
            return;
        }

        tempTextParams.Clear();
        tempTextParams.Add(TextParam.Of(GameData.COLOR.ITEM_SET_NAME_TEXT, resItemSet.GetName()));

        var count = 0;
        foreach (var itemID in resItemSet.itemIDs)
        {
            var _resItem = ResourceManager.Instance.item.GetItem(itemID);
            var color = GameData.COLOR.ITEM_SET_INACTIVE_TEXT;
            if (MyPlayer.Instance.core.inventory.IsEquipedItem(_resItem.id))
            {
                ++count;
                color = GameData.COLOR.ITEM_SET_ACTIVE_TEXT;
            }

            tempTextParams.Add(TextParam.Of(color, _resItem.GetName()));
        }

        tempTextParams.Add(TextParam.empty);

        foreach (var option in resItemSet.options)
        {
            var color = option.count > count ? GameData.COLOR.ITEM_SET_INACTIVE_TEXT : GameData.COLOR.ITEM_SET_ACTIVE_TEXT;
            tempTextParams.Add(TextParam.Of(color, option.name.L()));
            foreach (var desc in option.descs)
            {
                tempTextParams.Add(TextParam.Of(color, desc.L()));
            }
        }

        if (tempTextParams.Count == 0)
        {
            return;
        }

        SetText(string.Empty, Color.white);

        foreach (var param in tempTextParams)
        {
            SetText(param.text, param.color);
        }
    }

    public void SetText(string str, Color color)
    {
        var text = textPool.Pop();
        text.gameObject.SetActive(true);
        text.transform.SetAsLastSibling();
        text.SetText(str);
        text.SetTextColor(color);
    }
}