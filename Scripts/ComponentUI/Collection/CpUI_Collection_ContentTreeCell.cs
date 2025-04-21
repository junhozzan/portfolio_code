using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System;

namespace UICollection
{
    public class CpUI_Collection_ContentTreeCell : MyOSAHierarchyItem
    {
        [SerializeField] CpUI_Collection_ContentTreeCell_Group group = null;
        [SerializeField] CpUI_Collection_ContentTreeCell_Content content = null;

        private ReadOnlyDictionary<Type, CpUI_Collection_ContentTreeCell_Base> cells = null;
        private CpUI_Collection_ContentTreeCell_Base currentCell = null;

        public readonly CellInfo cellInfo = new CellInfo();

        public virtual void Init()
        {
            this.cells = new Dictionary<Type, CpUI_Collection_ContentTreeCell_Base>
            {
                [typeof(GroupOsaItem)] = group,
                [typeof(ContentOsaItem)] = content,
            }
            .ReadOnly();

            foreach (var cell in cells.Values)
            {
                cell.Init(this);
            }

            Cmd.Add(gameObject, eCmdTrigger.OnClick, Cmd_SelectCell);
        }

        private void SetDefault()
        {
            foreach (var cell in cells.Values)
            {
                cell.gameObject.SetActive(false);
            }

            currentCell = null;
        }

        public override void DoReset()
        {
            SetDefault();
        }

        public override void Refresh()
        {
            if (cellInfo != null)
            {
                cellInfo.Refresh();
            }

            if (currentCell != null)
            {
                currentCell.Refresh();
            }
        }

        public override void UpdateViews(MyOSAHierarchy.IOsaItem iOsaItem)
        {
            if (!cells.TryGetValue(iOsaItem.GetType(), out currentCell))
            {
                return;
            }

            cellInfo.SetOsaItem(iOsaItem);
            Refresh();
        }

        public override float GetSize()
        {
            if (currentCell == null)
            {
                return 0f;
            }

            return currentCell.GetSize();
        }

        private void Cmd_SelectCell()
        {
            if (currentCell == null)
            {
                return;
            }

            if (!currentCell.IsSelectable())
            {
                return;
            }

            UIBase.ClickSound();
            ToggleDirectory();
        }
    }
}