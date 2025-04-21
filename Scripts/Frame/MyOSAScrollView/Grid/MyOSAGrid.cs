using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;

public class MyOSAGrid : GridAdapter<MyGridParams, MyOSAGrid.ListVH>
{
    private readonly List<IOsaItem> items = new List<IOsaItem>();
    private readonly List<MyOSAGridItem> listObjects = new List<MyOSAGridItem>();
    private Action<MyOSAGridItem> onCreateItem = null;

    protected override void Awake()
    {
        if (IsInitialized)
        {
            return;
        }

        base.Init();

        // 원본 프리펩 숨기기
        _Params.Grid.CellPrefab.gameObject.SetActive(false);
        // 리스트 사이즈가 0일경우 Destroy 방지
        _Params.optimization.KeepItemsPoolOnEmptyList = true;
        _Params.effects.MaxSpeed = 2000;
        _Params._myParams.UpdateScrollSize(GetViewportSize(), GetContentSize());

        CheckLayoutElement(_Params.Grid.CellPrefab);
    }

    protected override void Start() { }

    public void Init(Action<MyOSAGridItem> onCreateItem)
    {
        this.onCreateItem = onCreateItem;
    }

    public void SetItems(ICollection<IOsaItem> _items)
    {
        items.Clear();
        items.AddRange(_items);

        ResetItems(items.Count);
    }

    public void Scroll(int idx, float viewStart = 0f, float Pivot = 0f)
    {
        if (idx < 0 || idx >= items.Count)
        { 
            return;
        }

        if (idx == 0)
        {
            ResetItems(0);
            ResetItems(items.Count);
        }
        else
        {
            ScrollTo(idx, viewStart, Pivot);
            //ScrollTo(idx);
        }
    }

    public void Refresh()
    {
        for (int i = 0, cnt = listObjects.Count; i < cnt; ++i)
        {
            var obj = listObjects[i];
            if (!obj.gameObject.activeSelf || obj.IsEmpty())
            {
                continue;
            }

            obj.Refresh();
        }
    }

    protected override CellGroupViewsHolder<ListVH> CreateViewsHolder(int itemIndex)
    {
        var item = base.CreateViewsHolder(itemIndex);
        var cells = item.ContainingCellViewsHolders;

        for (int i = 0, len = cells.Length; i < len; ++i)
        {
            var cell = cells[i].item;

            if (!listObjects.Contains(cell))
            {
#if UNITY_EDITOR
                cell.name = $"{cell.name}_{listObjects.Count}";
#endif
                listObjects.Add(cell);
            }

            if (onCreateItem != null)
            {
                onCreateItem(cell);
            }
        }

        return item;
    }

    protected override void UpdateCellViewsHolder(ListVH viewsHolder)
    {
        var index = viewsHolder.ItemIndex;
        if (index < 0 || index >= items.Count)
        {
            return;
        }

        var item = items[index];
        viewsHolder.UpdateViews(item);
    }


    public void CheckLayoutElement(RectTransform rt)
    {
        if (rt.GetComponent<LayoutElement>() != null)
        {
            return;
        }

        if (_DEBUG)
        {
            Debug.Log("## LayoutElement is null and create");
        }

        var le = rt.gameObject.AddComponent<LayoutElement>();
        le.preferredWidth = rt.rect.width;
        le.preferredHeight = rt.rect.height;
    }


    public class ListVH : CellViewsHolder
    {
        public MyOSAGridItem item { get; private set; } = null;

        public override void CollectViews()
        {
            base.CollectViews();

            if (root.TryGetComponent(out MyOSAGridItem _item))
            {
                item = _item;
            }
            else
            {
                if (_DEBUG)
                {
                    Debug.Log("## MyOSAGridItem component is null!");
                }
            }
        }

        public void UpdateViews(IOsaItem tOsaItem)
        {
            if (item == null)
            {
                return;
            }

            item.DoReset();
            item.UpdateViews(tOsaItem);
        }
    }

    public interface IOsaItem
    {
        void DoReset();
        bool IsEmpty();
        int SortCompare(IOsaItem other);
    }

    public class OsaPool<T> where T : IOsaItem, new()
    {
        public List<T> pool { get; private set; } = new List<T>();

        public void DoReset()
        {
            for (int i = 0, cnt = pool.Count; i < cnt; ++i)
            {
                pool[i].DoReset();
            }
        }

        public T Pop(int idx = -1)
        {
            if (idx != -1)
            {
                int require = idx - pool.Count;
                if (require >= 0)
                {
                    for (int i = 0; i <= require; ++i)
                    {
                        pool.Add(new T());
                    }
                }

                return pool[idx];
            }
            else
            {
                T t = pool.Find(x => x.IsEmpty());
                if (t == null)
                {
                    pool.Add(t = new T());
                }

                return t;
            }
        }
    }

#if USE_DEBUG
    private const bool _DEBUG = true;
#else
    private const bool _DEBUG = false;
#endif
}
