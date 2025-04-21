using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;

public class MyOSABasic : OSA<MyBaseParamsWithPrefab, MyOSABasic.ListVH>
{
    private readonly List<IOsaItem> items = new List<IOsaItem>();
    private readonly List<MyOSABasicItem> listObjects = new List<MyOSABasicItem>();

    private Action<MyOSABasicItem> onCreateItem = null;

    protected override void Awake()
    {
        if (IsInitialized)
        {
            return;
        }

        base.Init();

        // 원본 프리펩 숨기기
        _Params.ItemPrefab.gameObject.SetActive(false);
        // 리스트 사이즈가 0일경우 Destroy 방지
        _Params.optimization.KeepItemsPoolOnEmptyList = true;
        _Params.effects.MaxSpeed = 2000;

        _Params._myParams.UpdateScrollSize(GetViewportSize(), GetContentSize());
    }

    protected override void Start() { }

    public void Init(Action<MyOSABasicItem> onCreateItem)
    {
        this.onCreateItem = onCreateItem;
    }

    public void SetItems(ICollection<IOsaItem> _items)
    {
        items.Clear();
        items.AddRange(_items);

        ResetItems(items.Count);
    }


    public void Scroll(int idx, float viewStart = 0f, float pivot = 0f)
    {
        idx = Mathf.Clamp(idx, 0, items.Count - 1);

        //if (idx == 0)
        //{
        //    ResetItems(0);
        //    ResetItems(items.Count);
        //}
        //else
        {
            ScrollTo(idx, viewStart, pivot);
        }
    }

    public void Refresh()
    {
        for (int i = 0, cnt = listObjects.Count; i < cnt; ++i)
        {
            var obj = listObjects[i];
            if (!obj.gameObject.activeSelf)
            {
                continue;
            }

            obj.Refresh();
        }
    }

    public IEnumerable<T> GetList<T>() where T : MyOSABasicItem
    {
        return listObjects.Cast<T>();
    }

    protected override ListVH CreateViewsHolder(int itemIndex)
    {
        var vh = new ListVH();
        vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

        if (!listObjects.Contains(vh.item))
        {
#if UNITY_EDITOR
            vh.item.name = $"{vh.item.name}_{listObjects.Count}";
#endif
            listObjects.Add(vh.item);
        }

        if (onCreateItem != null)
        {
            onCreateItem(vh.item);
        }

        return vh;
    }

    protected override void UpdateViewsHolder(ListVH newOrRecycled)
    {
        int index = newOrRecycled.ItemIndex;
        if (index < 0 || index >= items.Count)
        {
            return;
        }

        var item = items[index];
        newOrRecycled.UpdateViews(item);

        /// 호출시 리스트 사이즈를 설정 가능
        ScheduleComputeVisibilityTwinPass();
    }

    protected override float UpdateItemSizeOnTwinPass(ListVH viewsHolder)
    {
        if (_Params._myParams._autoListSize)
        {
            return base.UpdateItemSizeOnTwinPass(viewsHolder);
        }

        return viewsHolder.UpdateSize();
    }

    public class ListVH : BaseItemViewsHolder
    {
        public MyOSABasicItem item { get; private set; } = null;

        public override void CollectViews()
        {
            base.CollectViews();

            if (root.TryGetComponent(out MyOSABasicItem _item))
            {
                item = _item;
            }
            else
            {
                if (_DEBUG)
                {
                    Debug.Log("## MyOSABasicItem component is null!");
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

        public float UpdateSize()
        {
            if (item == null)
            {
                return 0f;
            }

            return item.GetSize();
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
                int iRequire = idx - pool.Count;
                if (iRequire >= 0)
                {
                    for (int i = 0; i <= iRequire; ++i)
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



