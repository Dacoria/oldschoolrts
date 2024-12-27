using System.Collections.Generic;
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
                __goMap = LoadHelper.CreateGoDict(goList);                
            }

            return __goMap;
        }
    }
}