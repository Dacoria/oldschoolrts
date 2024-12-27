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

    public static List<T> CreatePrefabsFromRscList<T>(List<string> rscPathList)
    {
        var result = new List<T>();
        foreach (var rscPath in rscPathList)
        {
            var prefabs = TypeList<T>(rscPath);
            result.AddRange(prefabs);
        }

        return result;
    }

    public static Dictionary<string, T> ConvertDictToCaseIgnoreKey<T>(Dictionary<string, T> dict)
    {
        var dictResult = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var kv in dict)
        {
            dictResult.Add(kv.Key, kv.Value);
        }

        return dictResult;
    }
}