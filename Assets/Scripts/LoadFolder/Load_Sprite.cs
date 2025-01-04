using System.Collections.Generic;
using UnityEngine;

public static partial class Load
{
    private static Dictionary<string, Sprite> _spriteMapRsc;
    public static Dictionary<string, Sprite> SpriteMapRsc => LoadHelper.GetOrCreateCache(_spriteMapRsc, Constants.LOAD_PATH_SPRITE_RESOURCES);

    private static Dictionary<string, Sprite> _spriteMapBuildings;
    public static Dictionary<string, Sprite> SpriteMapBuildings => LoadHelper.GetOrCreateCache(_spriteMapBuildings, Constants.LOAD_PATH_SPRITE_BUILDINGS);

    private static Dictionary<string, Sprite> _spriteMapVillagers;
    public static Dictionary<string, Sprite> SpriteMapVillagers => LoadHelper.GetOrCreateCache(_spriteMapVillagers, Constants.LOAD_PATH_SPRITE_VILLAGERS);

    private static Dictionary<string, Sprite> _spriteMapMilitary;
    public static Dictionary<string, Sprite> SpriteMapMilitary => LoadHelper.GetOrCreateCache(_spriteMapMilitary, Constants.LOAD_PATH_SPRITE_MILITARY);    
}