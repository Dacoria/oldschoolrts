using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public static partial class Load
{
    private static List<string> goList = new List<string>
    {
        Constants.LOAD_PATH_GO_UI_PREFAB,
        Constants.LOAD_PATH_GO_BUILDINGS,
        Constants.LOAD_PATH_GO_RSC_TO_CARRY
    };

    private static Dictionary<string, GameObject> __goMap;
    public static Dictionary<string, GameObject> GoMap
    {
        get
        {
            if (__goMap == null || !Application.isPlaying)
            {
                __goMap = CreateGoDict(goList);                
            }

            return __goMap;
        }
    }

    public static Dictionary<string, GameObject> CreateGoDict(List<string> rscPathList)
    {
        var prefabs = LoadHelper.CreatePrefabsFromRscList<GameObject>(rscPathList);
        var prefabDict = prefabs.ToDictionary(x => x.name, y => y);
        return LoadHelper.ConvertDictToCaseIgnoreKey(prefabDict);
    }
}