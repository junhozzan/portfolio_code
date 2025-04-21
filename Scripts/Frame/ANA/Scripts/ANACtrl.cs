using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANACtrl
{
    public Dictionary<string, int> fileIDs = new Dictionary<string, int>();

    public void Init()
    {
        AndroidNativeAudio.makePool();
    }

    public void Load(string path)
    {
        if (fileIDs.ContainsKey(path))
        {
            return;
        }

        fileIDs.Add(path, AndroidNativeAudio.load(path));
    }

    public void Play(string path, float fVol)
    {
        if (!fileIDs.TryGetValue(path, out int id))
        {
            return;
        }

        AndroidNativeAudio.play(id, fVol);
    }
}
