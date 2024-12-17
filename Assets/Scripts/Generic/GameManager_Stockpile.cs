using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameManager : BaseAEMono
{
    public GameObject UnknownGameObjectPrefab;
    public Sprite UnknownSprite;

    private List<StockpileBehaviour> stockpiles = new List<StockpileBehaviour>();
    public List<ResourcePrefabItem> ResourcePrefabItems;

    private void InitResourcesStockpile()
    {
        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
        {
            var countItemTypeInDict = ResourcePrefabItems.Count(x => x.ItemType == itemType);
            if (countItemTypeInDict != 1)
            {
                throw new Exception($"ItemType {itemType} komt {countItemTypeInDict} keer voor ipv 1 -- > Zie Grass -> ResourcePrefabDictionary");
            }

            var resourceItem = ResourcePrefabItems.Single(x => x.ItemType == itemType);
            if (resourceItem.Icon == null)
            {
                resourceItem.Icon = UnknownSprite;
            }
            if (resourceItem.ResourcePrefab == null)
            {
                resourceItem.ResourcePrefab = UnknownGameObjectPrefab;
            }
        }
    }

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