using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public static partial class Load
{
    private static List<string> spriteRscList = new List<string>
    {
        Constants.LOAD_PATH_SPRITE_BUILDINGS,
        Constants.LOAD_PATH_SPRITE_RESOURCES,
        Constants.LOAD_PATH_SPRITE_UNITS
    };

    private static Dictionary<string, Sprite> __spriteMap;
    public static Dictionary<string, Sprite> SpriteMap
    {
        get
        {
            if (__spriteMap == null || !Application.isPlaying)            
                __spriteMap = CreateSpriteDict(spriteRscList);            

            return __spriteMap;
        }
    }

    public static Dictionary<string, Sprite> CreateSpriteDict(List<string> rscPathList)
    {
        var prefabs = LoadHelper.CreatePrefabsFromRscList<Sprite>(rscPathList);
        var prefabDict = prefabs.ToDictionary(x => x.name, y => y);
        return LoadHelper.ConvertDictToCaseIgnoreKey(prefabDict);
    }
}