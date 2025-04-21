using UnityEngine;
using System;

public class GameTime
{
    private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private DateTime cache_date = DateTime.Now;
    private float cache_sinceStartup = 0f;

    public DateTime now
    {
        get
        {
            return DateTime.Now;
            //var ts = TimeSpan.FromSeconds(realtimeSinceStartup - cache_sinceStartup);
            //return cache_date.Add(ts);
        }
    }

    public float realtimeSinceStartup
    {
        get
        {
            return Time.realtimeSinceStartup;
        }
    }

    public long nowToEpochSecond()
    {
        return now.ToEpochSecond();
        //return cache_date.ToEpochSecond() + (long)(realtimeSinceStartup - cache_sinceStartup);
    }

    public void SetNowTime(long timestamp)
    {
        var ts = TimeSpan.FromMilliseconds(timestamp);
        var now = epoch.Add(ts);

        cache_date = now;
        cache_sinceStartup = Time.realtimeSinceStartup;
    }
}
