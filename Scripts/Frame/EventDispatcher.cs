using System.Collections.Generic;
using System;

public class EventDispatcher<T> where T : Enum
{
    // InvalidOperationException 방지 변수
    private List<Handler> addHandlers = new List<Handler>();
    private List<Handler> removeHandlers = new List<Handler>();

    private List<Handler> handlers = new List<Handler>();
    private readonly Dictionary<T, List<object[]>> events = new Dictionary<T, List<object[]>>();
    private readonly List<CallParam> callPrams = new List<CallParam>();
    private Comparison<Handler> sort = null;

    public void DoReset()
    {
        foreach (var pair in events)
        {
            pair.Value.Clear();
        }

        callPrams.Clear();
    }

    public void SetSort(Comparison<Handler> sort)
    {
        this.sort = sort;
    }

    public void UpdateDt(float dt)
    {
        RemoveHandlers();
        AddHandlers();
        UpdateCall();
    }

    private void RemoveHandlers()
    {
        if (removeHandlers.Count == 0)
        {
            return;
        }

        foreach (var o in removeHandlers)
        {
            handlers.Remove(o);
        }

        removeHandlers.Clear();
    }

    private void AddHandlers()
    {
        if (addHandlers.Count == 0)
        {
            return;
        }

        handlers.AddRange(addHandlers);

        if (sort != null)
        {
            handlers.Sort(sort);
        }

        addHandlers.Clear();
    }

    private void UpdateCall()
    {
        foreach (var pair in events)
        {
            var type = pair.Key;
            var list = pair.Value;

            if (list.Count == 0)
            {
                continue;
            }

            foreach (var arg in list)
            {
                callPrams.Add(CallParam.Of(type, arg));
            }

            list.Clear();
        }

        if (callPrams.Count == 0)
        {
            return;
        }

        foreach (var param in callPrams)
        {
            Call(param.type, param.args);
        }

        callPrams.Clear();
    }

    private void Call(T type, object[] args)
    {
        foreach (var handler in handlers)
        {
            if (!handler.TryGetValue(type, out var calls))
            {
                continue;
            }

            if (calls == null)
            {
                continue;
            }

            foreach (var call in calls)
            {
                call(args);
            }
        }
    }

    public void AddHandler(Handler handler)
    {
        if (handler == null)
        {
            return;
        }

        if (handlers.Contains(handler))
        {
            return;
        }

        if (addHandlers.Contains(handler))
        {
            return;
        }

        addHandlers.Add(handler);
    }

    public void RemoveHandler(Handler handler)
    {
        if (handler == null)
        {
            return;
        }

        if (!handlers.Contains(handler))
        {
            return;
        }

        if (removeHandlers.Contains(handler))
        {
            return;
        }

        removeHandlers.Add(handler);
    }

    public void AddEvent(T type, object[] args)
    {
        if (!events.TryGetValue(type, out var list))
        {
            events.Add(type, list = new List<object[]>());
        }

        list.Add(args);
    }

    public class Handler
    {
        private readonly Dictionary<T, List<Action<object[]>>> typeActions = null;
        private readonly Func<bool> onCallable = null;
        
        public readonly int priority = 10000;

        public static Handler Of(Func<bool> onCallable, int priority)
        {
            return new Handler(onCallable, priority);
        }

        private Handler(Func<bool> onCallable, int priority)
        {
            this.typeActions = new Dictionary<T, List<Action<object[]>>>();
            this.onCallable = onCallable;
            this.priority = priority;
        }

        public Handler Add(T type, Action<object[]> on)
        {
            if (!typeActions.TryGetValue(type, out var list))
            {
                typeActions.Add(type, list = new List<Action<object[]>>());
            }

            list.Add(on);

            return this;
        }

        public bool TryGetValue(T type, out List<Action<object[]>> calls)
        {
            calls = null;
            if (onCallable != null && !onCallable.Invoke())
            {
                return false;
            }

            return typeActions.TryGetValue(type, out calls);
        }
    }

    private struct CallParam
    {
        public readonly T type;
        public readonly object[] args;

        public static CallParam Of(T type, object[] args)
        {
            return new CallParam(type, args);
        }

        private CallParam(T type, object[] args)
        {
            this.type = type;
            this.args = args;
        }
    }
}