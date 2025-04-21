using System;
using System.Collections.Generic;

public class XComponent
{
    // 부모 컴포넌트
    private XComponent parent = null;
    // 자식 컴포넌트
    private readonly List<XComponent> children = new List<XComponent>();
    // 최적화를 위해 Update는 Delegate로 실행
    private Action<float> onUpdate = null;

    private void SetParent(XComponent com)
    {
        parent = com;
    }

    private void AddChildren(XComponent com)
    {
        if (children.Contains(com))
        {
            return;
        }

        children.Add(com);
    }

    #region 최상위 컴포넌트에서 호출
    public virtual void Initialize()
    {
        children.ForEach(x => x.Initialize());
    }

    public virtual void DoReset()
    {
        children.ForEach(x => x.DoReset());
    }

    public virtual void OnEnable()
    {
        children.ForEach(x => x.OnEnable());
    }

    public virtual void OnDisable()
    {
        children.ForEach(x => x.OnDisable());
    }

    public virtual void UpdateDt(float dt)
    {
        children.ForEach(x => x.onUpdate?.Invoke(dt));
        children.ForEach(x => x.UpdateDt(dt));
    }
    #endregion

    public T AddComponent<T>(params object[] args) where T : XComponent
    {
        var com = Util.GetNewInstance(typeof(T), args) as T;
        com.SetParent(this);

        AddChildren(com);
        return com;
    }

    public T GetComponent<T>() where T : XComponent
    {
        var _ = this;
        while (_ != null)
        {
            var com = _.GetComponentInChildren<T>();
            if (com != null)
            {
                return com;
            }

            _ = _.parent;
        }

        return null;
    }

    private T GetComponentInChildren<T>() where T : XComponent
    {
        foreach (var child in children)
        {

            if (child is T match)
            {
                return match;
            }

            var nested = child.GetComponentInChildren<T>();
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }

#if USE_DEBUG
    protected const bool _DEBUG = true;
#else
    protected const bool _DEBUG = false;
#endif
}
