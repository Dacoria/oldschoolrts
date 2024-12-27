using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class Load
{
    private static List<string> spriteRscList = new List<string>
    {
        Constants.LOAD_PATH_SPRITE_BUILDINGS,
        Constants.LOAD_PATH_SPRITE_RESOURCES
    };

    private static Dictionary<string, Sprite> __spriteMap;
    public static Dictionary<string, Sprite> SpriteMap
    {
        get
        {
            if (__spriteMap == null || !Application.isPlaying)            
                __spriteMap = LoadHelper.CreateSpriteDict(spriteRscList);            

            return __spriteMap;
        }
    }
}