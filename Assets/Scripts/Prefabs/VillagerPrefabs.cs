using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VillagerPrefabs
{
    private static List<VillagerUnitSetting> cache;
    public static List<VillagerUnitSetting> Get()
    {
        if (cache == null)
        {
            cache = GenerateVillagerUnitSetting();
        }
        return cache;
    }

    private static List<VillagerUnitSetting> GenerateVillagerUnitSetting()
    {
        var result = new List<VillagerUnitSetting>();

        foreach (VillagerUnitType villagerUnitType in Enum.GetValues(typeof(VillagerUnitType)).Cast<VillagerUnitType>().ToList())
        {
            if (Load.GoMap.TryGetValue($"{villagerUnitType.ToString()}Prefab", out GameObject villagerPrefab))
            {
                if (Load.SpriteMap.TryGetValue($"{villagerUnitType.ToString()}Image", out Sprite villagerUnitSprite))
                {
                    var item = new VillagerUnitSetting
                    {
                        Type = villagerUnitType,
                        ItemsConsumedToProduce = GetItemsConsumedToProduce(villagerUnitType),
                        Icon = villagerUnitSprite,
                        VillagerBehaviour = villagerPrefab.GetComponent<IVillagerUnit>()
                    };
                    result.Add(item);
                }
                else
                {
                    throw new Exception($"VillagerUnitType {villagerUnitType} heeft geen sprite image :O");
                }
            }
            else
            {
                throw new Exception($"VillagerUnitType {villagerUnitType} heeft geen prefab GO :O");
            }
        }

        return result;
    }

    private static List<ItemAmountBuffer> GetItemsConsumedToProduce(VillagerUnitType type)
    {
        switch (type)
        {
            case VillagerUnitType.Builder:
                return new List<ItemAmountBuffer> { new ItemAmountBuffer { Amount = 2, ItemType = ItemType.GOLDBAR, MaxBuffer = 5 } };
            default:
                return new List<ItemAmountBuffer> { new ItemAmountBuffer { Amount = 1, ItemType = ItemType.GOLDBAR, MaxBuffer = 5 } };
        }
    }
}