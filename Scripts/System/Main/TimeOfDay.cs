using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay
{
    private static readonly IReadOnlyDictionary<Packet, GameEventType> loadPackets
        = new Dictionary<Packet, GameEventType>()
        {
            [Packet.TIME_OF_DAY] = GameEventType.TIME_OF_DAY,
            [Packet.GET_USER_INFO] = GameEventType.GET_USER_INFO,
            [Packet.GET_MISSION_DATAS] = GameEventType.GET_MISSION_DATAS,
            [Packet.GET_STORE_DATAS] = GameEventType.GET_STORE_DATAS,
            [Packet.GET_ATTENDANCE_DATAS] = GameEventType.GET_ATTENDANCE_DATAS,
        };

    private float tickTimer = 0f;

    public void UpdateDt(float dt)
    {
        tickTimer -= dt;
        if (tickTimer > 0f)
        {
            return;
        }

        tickTimer += 5f;

        if (Main.Instance.time.now.Date > User.Instance.info.lastLink.Date)
        {
            if (_DEBUG)
            {
                Debug.Log("## Call Time Of Day");
            }

            Main.Instance.StartCoroutine(LoadAllData());
        }
    }

    private IEnumerator LoadAllData()
    {
        foreach (var kv in loadPackets)
        {
            yield return LoadData(kv.Key, kv.Value);
        }
    }

    private IEnumerator LoadData(Packet packet, GameEventType eventType)
    {
        var loadData = true;
        VirtualServer.Send(packet, o => { loadData = false; GameEvent.Instance.AddEvent(eventType, o); });

        while (loadData)
        {
            yield return null;
        }
    }

#if USE_DEBUG
    protected const bool _DEBUG = true;
#else
    protected const bool _DEBUG = false;
#endif
}
