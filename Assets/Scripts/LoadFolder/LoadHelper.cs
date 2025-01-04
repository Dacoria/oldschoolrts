using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LoadHelper
{
    public static Dictionary<string, T> GetOrCreateCache<T>(this Dictionary<string, T> cache, string rscPath) where T : UnityEngine.Object
    {
        if (cache == null || !Application.isPlaying)
        {
            cache = CreateDict<T>(new List<string> { rscPath });
        }

        return cache;
    }

    private static Dictionary<string, T> CreateDict<T>(List<string> rscPathList) where T : UnityEngine.Object
    {
        var prefabs = CreatePrefabsFromRscList<T>(rscPathList);
        var prefabDict = prefabs.ToDictionary(x => x.name, y => y);
        return ConvertDictToCaseIgnoreKey(prefabDict);
    }

    private static List<T> CreatePrefabsFromRscList<T>(List<string> rscPathList)
    {
        var result = new List<T>();
        foreach (var rscPath in rscPathList)
        {
            var prefabs = Resources.LoadAll(rscPath, typeof(T)).Cast<T>().ToList();
            result.AddRange(prefabs);
        }

        return result;
    }            

    private static Dictionary<string, T> ConvertDictToCaseIgnoreKey<T>(Dictionary<string, T> dict)
    {
        var dictResult = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var kv in dict)
        {
            dictResult.Add(kv.Key, kv.Value);
        }

        return dictResult;
    }
}