using UnityEngine;
using SimpleJSON;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;

public class LocalSave
{
    private static readonly Dictionary<SaveType, JSONNode> datas = new Dictionary<SaveType, JSONNode>();
    private static readonly Dictionary<string, string> fileToBase64 = new Dictionary<string, string>();

    public static void Load()
    {
        datas.Clear();

        for (SaveType type = SaveType.AINFOFIRST; type < SaveType.CNT; ++type)
        {
            var filename = type.ToString().ToLower();
            var path = GetPath(filename);

            if (!File.Exists(path))
            {
                continue;
            }

            var loadData = SaveControl.LoadString(filename, File.ReadAllBytes(path));
            var data = (loadData != null) ? JSONNode.Parse(loadData) : new JSONObject();
            datas.Add(type, data);
        }
    }

    public static void Save(SaveType type, JSONNode node)
    {
        var fileName = type.ToString().ToLower();
        var path = GetPath(fileName);

        SaveControl.SaveToByteFile(path, fileName, node.ToString());
    }

    public static JSONNode Get(SaveType type)
    {
        if (!datas.TryGetValue(type, out var v))
        {
            datas.Add(type, v = new JSONObject());
        }

        return v;
    }

    private static string GetPath(string filename)
    {
        if (!fileToBase64.TryGetValue(filename, out var v))
        {
            var b = Encoding.UTF8.GetBytes(filename);
            var s = Convert.ToBase64String(b);
            var remove = s.Replace("=", "");
            fileToBase64.Add(filename, v = remove);
        }

        return string.Format("{0}/{1}", Application.persistentDataPath, v);
    }

    public static void DeleteAll()
    {
        var emptyString = new JSONObject().ToString();
        for (SaveType type = SaveType.AINFOFIRST; type < SaveType.CNT; ++type)
        {
            Save(type, emptyString);
        }
    }

    public enum SaveType
    {
        NONE = -1,

        AINFOFIRST, // 정보
        BITEMSECON, // 아이템
        CMODETHREE, // 모드
        DLABFOREFO, // 연구
        FMISSIONSI, // 임무
        GACHIEVESE, // 업적
        HATTENDANCE, // 출석
        ICOLLECTION, // 도감
        JSTORETENT, // 상점
        KINVENTORY, // 인벤
        LADVERTISE, // 광고
        MCARDEIGHT, // 카드
        OMERGENINE, // 머지

        CNT
    }
}


