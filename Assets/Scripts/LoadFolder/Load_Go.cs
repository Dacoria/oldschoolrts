using System.Collections.Generic;
using UnityEngine;

public static partial class Load
{
    private static List<string> goList = new List<string>
    {
        Constants.LOAD_PATH_GO_UI_PREFAB,
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

    public static GameObject Get(this Dictionary<string, GameObject> dict, string key)
    {
        var go = dict[key];
        if (go == null)
        {
            throw new System.Exception("No Gameobject for key: " + key);
        }

        return go;
    }
}