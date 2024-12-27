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
        return prefabs.ToDictionary(x => x.name, y => y);
    }

    public static Dictionary<string, Sprite> CreateSpriteDict(List<string> rscPathList)
    {
        var prefabs = CreatePrefabsFromRscList<Sprite>(rscPathList);
        return prefabs.ToDictionary(x => x.name, y => y);
    }
}