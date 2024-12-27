using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class Load
{
    private static List<string> spriteRscList = new List<string>
    {
        Constants.LOAD_PATH_SPRITE,
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

    public static Sprite Get(this Dictionary<string, Sprite> dict, string key)
    {
        var sprite = dict[key];
        if(sprite == null)
        {
            throw new System.Exception("No sprite for key: " + key);
        }

        return sprite;
    }
}