using ModeComponent;
using MyPlayerComponent;
using System;

public class GameEvent : Singleton<GameEvent>
{
    private readonly EventDispatcher<GameEventType> eventDispatcher = new EventDispatcher<GameEventType>();

    public GameEvent()
    {
        eventDispatcher.SetSort((a, b) => a.priority.CompareTo(b.priority));
    }

    public void DoReset()
    {
        eventDispatcher.DoReset();
    }

    public void UpdateDt(float dt)
    {
        eventDispatcher.UpdateDt(dt);
    }

    public void RemoveHandler(EventDispatcher<GameEventType>.Handler handler)
    {
        eventDispatcher.RemoveHandler(handler);
    }

    public void AddEvent(GameEventType type, params object[] args)
    {
        eventDispatcher.AddEvent(type, args);
    }

    public void AddEvent(GameEventType type)
    {
        eventDispatcher.AddEvent(type, null);
    }

    public EventDispatcher<GameEventType>.Handler CreateHandler(object o, Func<bool> onCallable)
    {
        var handler = EventDispatcher<GameEventType>.Handler.Of(onCallable, GetPriority(o));
        eventDispatcher.AddHandler(handler);

        return handler;
    }

    private static int GetPriority(object o)
    {
        int i = 0;
        ++i;
        if (o is MyPlayerBaseComponent)
        {
            return i;
        }

        ++i;
        if (o is Unit)
        {
            return i;
        }

        ++i;
        if (o is ModeBaseComponent)
        {
            return i;
        }

        ++i;
        if (o is SkillBase)
        {
            return i;
        }

        return i;
    }

    public static T GetSafe<T>(object[] args, int index) where T : class
    {
        if (args == null)
        {
            return null;
        }

        if (index < 0 || index >= args.Length)
        {
            return null;
        }

        return args[index] as T;
    }

    public static T? GetSafeS<T>(object[] args, int index) where T : struct
    {
        if (args == null)
        {
            return null;
        }

        if (index < 0 || index >= args.Length)
        {
            return null;
        }

        return (T)args[index];
    }
}

public enum GameEventType
{
    LOGIN,
    TIME_OF_DAY,
    CREATE_NICKNAME,

    GET_USER_INFO,
    GET_MODE_DATAS,
    GET_ITEM_DATAS,
    GET_MISSION_DATAS,
    GET_ATTENDANCE_DATAS,
    GET_LAB_DATAS,
    GET_COLLECTION_DATAS,
    GET_STORE_DATAS,
    GET_ADVERTISEMENT_DATAS,
    GET_INVENTORY_INFO,
    GET_CARD_DATAS,
    GET_MERGE_DATAS,

    JOIN_FIELD,
    JOIN_MY_UNIT,
    DEAD_UNIT,
    REMOVE_UNIT,

    MODE_ENTER,
    MODE_START,
    MODE_PLAY,
    MODE_SUCCESS,
    MODE_FAIL,

    GET_MODE_REWARD,
    SUCCESS_DUNBREAK,

    REFRESH_SCORE,
    REFRESH_EXP,

    UPDATE_MODE,
    UPDATE_AD,
    UPDATE_ITEM,
    UPDATE_ITEM_ALL,
    UPDATE_ITEM_AMOUNT,
    UPDATE_ITEM_NEW,
    UPDATE_CARD,

    UPDATE_LAB,
    UPDATE_LAB_VIRTUAL,
    UPDATE_MISSION,
    UPDATE_ATTENDANCE,
    UPDATE_INVENTORY,
    UPDATE_COLLECTION,
    UPDATE_MERGE,

    ENEMY_ATTACKED,
    
    CHANGE_MYUNIT_HP,
    CHANGE_MYUNIT_MP,

    EQUIP_ITEM,
    CHANGE_UNIT_COUNT,
    PURCHASE_STORE_ITEM,
    SHOW_GET_INFO,
    RESPONSE_RANK,

    CONNECT_PLATFORM,
    MY_UNIT_STAT_UPDATED,

    CREATE_MERGE_ITEM,
}