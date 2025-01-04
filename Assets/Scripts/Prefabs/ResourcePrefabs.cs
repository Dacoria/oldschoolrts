using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ResourcePrefabs
{
    private static List<ResourcePrefabItem> cache;
    public static List<ResourcePrefabItem> Get()
    {
        if (cache == null)
        {
            cache = GenerateResourcePrefabItems();
        }
        return cache;        
    }

    private static List<ResourcePrefabItem> GenerateResourcePrefabItems()
    {
        var result = new List<ResourcePrefabItem>();
        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)).Cast<ItemType>().OrderBy(x => x.ToString()))
        {
            if (Load.SpriteMapRsc.TryGetValue($"{itemType.ToString()}Image", out Sprite itemSprite))
            {
                var prefabBuilding = new ResourcePrefabItem
                {
                    Icon = itemSprite,
                    ItemType = itemType,
                    ResourcePrefab = Load.GoMapRscToCarry[Constants.GO_PREFAB_RSC_TO_CARRY_CUBE_UNKNOWN]
                };
                /*
                if (resourceCarriedGoName.TryGetValue(itemType, out var name))
                {
                    prefabBuilding.ResourcePrefab = Load.GoMap[name];
                }
                else
                {
                    prefabBuilding.ResourcePrefab = Load.GoMap[Constants.GO_PREFAB_RSC_TO_CARRY_CUBE_UNKNOWN];
                }*/

                result.Add(prefabBuilding);
            }
            else
            {
                throw new Exception($"ResourceType {itemType} heeft geen Icon :O");
            }
        }

        return result;
    }

    // Voor nu: Ongebruikt
    private static Dictionary<ItemType, string> resourceCarriedGoName = new Dictionary<ItemType, string>
    {
        { ItemType.NONE, "CubeUnknownBeingCarried" },
        { ItemType.LUMBER, "LogPrefab" },
        { ItemType.WATER, "WaterBucket" },
        { ItemType.FISH, "Fish" },
    };
}
