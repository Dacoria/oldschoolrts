using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LoadHelper
{
    private static List<T> TypeList<T>(string path) =>
        Resources
        .LoadAll(path, typeof(T))
        .Cast<T>()
        .ToList();

    private static List<T> CreatePrefabsFromRscList<T>(List<string> rscPathList)
    {
        var result = new List<T>();
        foreach (var rscPath in rscPathList)
        {
            var prefabs = TypeList<T>(rscPath);
            result.AddRange(prefabs);
        }

        return result;
    }

    public static Dictionary<string, GameObject> CreateGoDict(List<string> rscPathList)
    {
        var prefabs = CreatePrefabsFromRscList<GameObject>(rscPathList);
        var prefabDict = prefabs.ToDictionary(x => x.name, y => y);
        var dictResult = new Dictionary<string, GameObject>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var kv in prefabDict)
        {
            dictResult.Add(kv.Key, kv.Value);
        }

        return dictResult;
    }

    public static Dictionary<string, Sprite> CreateSpriteDict(List<string> rscPathList)
    {
        var prefabs = CreatePrefabsFromRscList<Sprite>(rscPathList);
        var prefabDict = prefabs.ToDictionary(x => x.name, y => y);
        var dictResult = new Dictionary<string, Sprite>(StringComparer.InvariantCultureIgnoreCase);
        foreach(var kv in prefabDict)
        {
            dictResult.Add(kv.Key, kv.Value);
        }

        return dictResult;
    }
}