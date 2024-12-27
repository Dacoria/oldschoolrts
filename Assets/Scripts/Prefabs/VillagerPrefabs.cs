using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VillagerPrefabs
{
    private static List<VillagerUnitSetting> _costs;
    public static List<VillagerUnitSetting> Get()
    {
        if (_costs == null)
        {
            _costs = GenerateVillagerUnitSetting();
        }
        return _costs;
    }

    private static List<VillagerUnitSetting> GenerateVillagerUnitSetting()
    {
        var result = new List<VillagerUnitSetting>();

        foreach (VillagerUnitType villagerUnitType in Enum.GetValues(typeof(VillagerUnitType)).Cast<VillagerUnitType>().OrderBy(x => x.ToString()))
        {
            if (Load.GoMap.TryGetValue($"{villagerUnitType.ToString()}Prefab", out GameObject villagerPrefab))
            {
                if (Load.SpriteMap.TryGetValue($"{villagerUnitType.ToString()}Image", out Sprite villagerUnitSprite))
                {
                    var item = new VillagerUnitSetting
                    {
                        Type = villagerUnitType,
                        ItemsConsumedToProduce = new List<ItemAmountBuffer> { new ItemAmountBuffer {Amount = 1, ItemType = ItemType.GOLDBAR, MaxBuffer = 5 } },
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
}