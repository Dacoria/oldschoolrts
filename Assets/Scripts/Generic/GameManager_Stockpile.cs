using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameManager : BaseAEMonoCI
{
    private List<StockpileBehaviour> stockpiles = new List<StockpileBehaviour>();
    [HideInInspector] public List<ResourcePrefabItem> ResourcePrefabItems = new List<ResourcePrefabItem>();

    private void InitResourcesStockpile()
    {
        ResourcePrefabItems = new List<ResourcePrefabItem>();
        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)).Cast<ItemType>().OrderBy(x => x.ToString()))
        {
            if (Load.SpriteMap.TryGetValue($"{itemType.ToString()}Image", out Sprite itemSprite))
            {
                var prefabBuilding = new ResourcePrefabItem
                {
                    Icon = itemSprite,
                    ItemType = itemType
                };

                if(resourceCarriedGoName.TryGetValue(itemType, out var name))
                {
                    prefabBuilding.ResourcePrefab = Load.GoMap[name];
                }
                else
                {
                    prefabBuilding.ResourcePrefab = Load.GoMap["CubeUnknownBeingCarried"];
                }

                ResourcePrefabItems.Add(prefabBuilding);
            }
            else
            {
                throw new Exception($"ResourceType {itemType} heeft geen Icon :O");
            }
        }
    }

    private Dictionary<ItemType, string> resourceCarriedGoName = new Dictionary<ItemType, string>
    {
        { ItemType.NONE, "CubeUnknownBeingCarried" },
        { ItemType.LUMBER, "LogPrefab" },
        { ItemType.WATER, "WaterBucket" },
        { ItemType.FISH, "Fish" },
    };

    public void RegisterStockpile(StockpileBehaviour stockpileBehaviour)
    {
        stockpiles.Add(stockpileBehaviour);
    }

    public void TryRemoveStockpile(StockpileBehaviour stockpileBehaviour)
    {
        var stockpile = stockpiles.FirstOrDefault(x => x == stockpileBehaviour);
        if (stockpile != null)
        {
            stockpiles.Remove(stockpile);
        }
    }
}