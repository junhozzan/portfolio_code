using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool<T> where T : Component
{
    private readonly List<T> pool = null;
    private readonly T origin = null;
    private readonly GameObject parent = null;
    private readonly Action<T> onCreateInit = null;
    private readonly Action<T> onClear = null;
    private readonly bool canDestroy = false;

    // 이펙트 양 조절
    private readonly int maximum = -1;

    public static ObjectPool<T> Of(T origin, GameObject parent, int maximum, bool canDestroy = false, Action<T> onCreateInit = null, Action<T> onClear = null, bool originVisible = false)
    {
        return new ObjectPool<T>(origin, parent, canDestroy, onCreateInit, onClear, originVisible, maximum);
    }

    public static ObjectPool<T> Of(T origin, GameObject parent, bool canDestroy = false, Action<T> onCreateInit = null, Action<T> onClear = null, bool originVisible = false)
    {
        return new ObjectPool<T>(origin, parent, canDestroy, onCreateInit, onClear, originVisible, -1);
    }

    public static ObjectPool<T> Of(T origin, Transform parent, bool canDestroy = false, Action<T> onCreateInit = null, Action<T> onClear = null, bool originVisible = false)
    {
        return new ObjectPool<T>(origin, parent.gameObject, canDestroy, onCreateInit, onClear, originVisible, -1);
    }

    private ObjectPool(T origin, GameObject parent, bool canDestroy, Action<T> onCreateInit, Action<T> onClear, bool originVisible, int maximum)
    {
        origin.gameObject.SetActive(originVisible);

        this.pool = new List<T>(32);
        this.origin = origin;
        this.parent = parent;
        this.canDestroy = canDestroy;
        this.onCreateInit = onCreateInit;
        this.onClear = onClear;
        this.maximum = maximum;
    }

    public T Create()
    {
        var go = GameObject.Instantiate(origin.gameObject, Vector3.zero, Quaternion.identity, parent.transform);

#if UNITY_EDITOR
        go.name = origin.name;
#endif

        if (!go.TryGetComponent(out T obj))
        {
            obj = go.AddComponent<T>();

            if (_DEBUG)
            {
                Debug.LogFormat($"## pool object type is null !! : {origin.name}");
            }
        }

        if (_DEBUG)
        {
            Debug.LogFormat($"## pool list count : {origin.name} [{pool.Count + 1}]");
        }

        if (onCreateInit != null)
        {
            onCreateInit(obj);
        }

        return obj;
    }

    public T Pop(bool visible = true)
    {
        var obj = pool.Find(x => !x.gameObject.activeSelf);
        if (obj == null)
        {
            if (maximum != -1 && pool.Count > maximum)
            {
                return null;
            }

            pool.Add(obj = Create());
        }

        obj.gameObject.SetActive(visible);

        return obj;
    }

    public void Clear(bool forceDestroy = false)
    {
        if (pool == null || pool.Count == 0)
        {
            return;
        }

        if (canDestroy || forceDestroy) 
        {
            // 모든 객체 제거 (Destroy)
            foreach (var item in pool)
            {
                onClear?.Invoke(item);
                GameObject.Destroy(item.gameObject);
            }

            pool.Clear();
        }
        else 
        {
            // 모든 객체 제거 (Invisible)
            foreach (var item in pool)
            {
                onClear?.Invoke(item);
                item.gameObject.SetActive(false);
            }
        }
    }

    public List<T> GetPool()
    {
        return pool;
    }

#if USE_DEBUG && USE_COUNT_DEBUG
    private const bool _DEBUG = true;
#else
    private const bool _DEBUG = false;
#endif
}

public class SimplePool<T> where T : SimplePoolItem
{
    private readonly Func<T> create = null;
    private readonly Queue<T> pool = null;
    private readonly List<T> list = null;

    public List<T> _list { get { return list; } }

    public static SimplePool<T> Of(Func<T> create, int size)
    {
        return new SimplePool<T>(create, size);
    }

    private SimplePool(Func<T> create, int size)
    {
        this.create = create;
        this.pool = new Queue<T>(size);
        this.list = new List<T>(size);
    }

    public void Clear()
    {
        foreach (var item in list)
        {
            if (pool.Contains(item))
            {
                continue;
            }

            item.OnDisable();
        }
    }

    public T Pop()
    {
        if (pool.Count == 0)
        {
            var newItem = create();
            newItem.SetOnReturn(ReturnItem);
            newItem.Initialize();

            pool.Enqueue(newItem);
            list.Add(newItem);

            if (_DEBUG)
            {
                Debug.LogFormat("## totalCreateCount : {0} [{1}]", newItem.GetType().Name, list.Count);
            }
        }

        var item = pool.Dequeue();
        item.OnEnable();
        item.DoReset();

        return item;
    }

    private void ReturnItem(SimplePoolItem item)
    {
        var tItem = item as T;
        if (tItem == null)
        {
            return;
        }

        if (pool.Contains(tItem))
        {
            if (_DEBUG)
            {
                Debug.Log($"## pool return contains {tItem.GetType().Name}");
            }

            return;
        }

        pool.Enqueue(tItem);
    }

#if USE_DEBUG && USE_COUNT_DEBUG
    private const bool _DEBUG = true;
#else
    private const bool _DEBUG = false;
#endif
}

public class SimplePoolItem
{
    private bool used = false;
    private Action<SimplePoolItem> onReturn = null;

    public void SetOnReturn(Action<SimplePoolItem> onReturn)
    {
        this.onReturn = onReturn;
    }

    public virtual void Initialize()
    {

    }

    public virtual void DoReset()
    {

    }

    public virtual void OnEnable()
    {
        used = true;
    }

    public virtual void OnDisable()
    {
        used = false;

        if (onReturn != null)
        {
            onReturn(this);
        }
    }

    public bool IsUsed()
    {
        return used;
    }
}