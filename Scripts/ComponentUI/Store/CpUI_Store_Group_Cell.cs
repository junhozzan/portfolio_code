using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UIStore
{
    public class CpUI_Store_Group_Cell : UIBase
    {
        [SerializeField] GameObject on = null;
        [SerializeField] GameObject off = null;
        [SerializeField] UIText[] nameTexts = null;

        private ResourceStoreGroup resGroup = null;
        private Action<ResourceStoreGroup> onClick = null;

        public void Init(ResourceStoreGroup resGroup, Action<ResourceStoreGroup> onClick)
        {
            this.resGroup = resGroup;
            this.onClick = onClick;

            RefreshNameTexts();

            Cmd.Add(gameObject, eCmdTrigger.OnClick, Cmd_OnClick);
        }

        private void RefreshNameTexts()
        {
            if (nameTexts == null)
            {
                return;
            }

            foreach (var text in nameTexts)
            {
                text.SetTextKey(resGroup.title);
            }
        }

        public void SetOnState(ResourceStoreGroup onresGroup)
        {
            var isOn = onresGroup == resGroup;
            on.SetActive(isOn);
            off.SetActive(!isOn);
        }

        private void Cmd_OnClick()
        {
            ClickSound();
            onClick?.Invoke(resGroup);
        }
    }
}