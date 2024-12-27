using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ResourcePrefabs
{
    private static List<ResourcePrefabItem> _prefabs;
    public static List<ResourcePrefabItem> Get()
    {
        if (_prefabs == null)
        {
            _prefabs = GenerateResourcePrefabItems();
        }
        return _prefabs;        
    }

    private static List<ResourcePrefabItem> GenerateResourcePrefabItems()
    {
        var result = new List<ResourcePrefabItem>();
        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)).Cast<ItemType>().OrderBy(x => x.ToString()))
        {
            if (Load.SpriteMap.TryGetValue($"{itemType.ToString()}Image", out Sprite itemSprite))
            {
                var prefabBuilding = new ResourcePrefabItem
                {
                    Icon = itemSprite,
                    ItemType = itemType,
                    ResourcePrefab = Load.GoMap["CubeUnknownBeingCarried"]
                };
/*
                if (resourceCarriedGoName.TryGetValue(itemType, out var name))
                {
                    prefabBuilding.ResourcePrefab = Load.GoMap[name];
                }
                else
                {
                    prefabBuilding.ResourcePrefab = Load.GoMap["CubeUnknownBeingCarried"];
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
