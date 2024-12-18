using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardItemsProduceBehaviour : MonoBehaviour, ICardBuilding, IResourcesToProduce, IProduceResourceOverTime, IRefillItems
{
    public List<ItemProductionSetting> ItemProductionSettings;

    private ConsumeRefillItemsBehaviour ConsumeRefillItems;
    private ProduceResourceOrderBehaviour ProduceResourceOrderBehaviour;

    private List<ItemLimit> ItemsToProcess;

    public void Awake()
    {
        ItemsToProcess = ItemProductionSettings
            .Select(x =>
                new ItemLimit
                {
                    ItemType = x.Type
                }
            ).ToList();

        gameObject.AddComponent<RefillBehaviour>();
        ConsumeRefillItems = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();

        if (GetComponent<ProduceResourceOrderBehaviour>() == null)
        {
            ProduceResourceOrderBehaviour = gameObject.AddComponent<ProduceResourceOrderBehaviour>();
        }
    }

    public List<ItemProduceSetting> GetItemProduceSettings() =>
        ItemProductionSettings.ConvertAll(x => (ProductionSetting)x).ConvertToSingleProduceItem();
    public bool CanProduceResource() => GetItemToProduce() != null;

    // bepaalt welk item eerst --> Logica nu: meeste aantal eerst (daarna lijstvolgorde)
    public ItemProduceSetting GetItemToProduce()
    {
        var itemsThatCanBeProduced = ItemProductionSettings.Where(x => CanProduceResource(x) && !HasReachedProductionBuffer(x));

        foreach (var itemToProcess in ItemsToProcess
            .Where(x => x.ItemsToProduce >= 1)
            .OrderByDescending(x => x.ItemsToProduce))
        {
            if (itemsThatCanBeProduced.Any(x => x.Type == itemToProcess.ItemType))
            {
                return itemsThatCanBeProduced.Single(x => x.Type == itemToProcess.ItemType).ConvertToSingleProduceItem();
            }
        }

        return null;
    }

    private bool HasReachedProductionBuffer(ItemProductionSetting itemProductionSetting)
    {
        foreach (var itemToProduce in itemProductionSetting.ItemsConsumedToProduce)
        {
            var outstandingOrders = ProduceResourceOrderBehaviour.OutputOrders.Where(x => x.ItemType == itemToProduce.ItemType);
            if (outstandingOrders.Count() + itemToProduce.Amount > itemToProduce.MaxBuffer)
            {
                return true;
            }
        }

        return false;
    }

    private bool CanProduceResource(ItemProductionSetting itemProductionSetting)
    {
        return ConsumeRefillItems.CanConsumeRefillItems(itemProductionSetting.ItemsConsumedToProduce);
    }

    public bool ConsumeRequiredResources(ItemProduceSetting itemProduceSetting) =>
        ConsumeRefillItems.TryConsumeRefillItems(itemProduceSetting.ItemsConsumedToProduce.ConvertAll(x => (ItemAmount)x));

    public bool AlwaysRefillItemsIgnoreBuffer() => false;

    public List<ItemOutput> GetAvailableItemsToProduce() => GetItemProduceSettings().SelectMany(x => x.ItemsToProduce).ToList();

    public int GetCount(Enum type) => ItemsToProcess.Single(x => x.ItemType == (ItemType)type).ItemsToProduce;


    public void AddItem(Enum type)
    {
        GetItem((ItemType)type).ItemsToProduce++;
    }

    public void DecreaseItem(Enum type)
    {
        var item = GetItem((ItemType)type);
        item.ItemsToProduce = Math.Max(item.ItemsToProduce - 1, 0);
    }

    private ItemLimit GetItem(ItemType itemType) => ItemsToProcess.Single(x => x.ItemType == itemType);

    public ProductionSetting GetCardDisplaySetting(Enum type) => ItemProductionSettings.Single(x => x.GetType() == type);

    private List<ItemAmountBuffer> GetItemsToConsume(ItemType type) =>
        ItemProductionSettings.Single(x => x.Type == type).ItemsConsumedToProduce;

    public bool CanProces(Enum type)
    {
        var itemsToConsume = GetItemsToConsume((ItemType)type);
        return ConsumeRefillItems.CanConsumeRefillItems(itemsToConsume);
    }

    public QueueForBuildingBehaviour GetQueueForBuildingBehaviour() => GetComponent<QueueForBuildingBehaviour>();

    public void StartProducing(ItemProduceSetting itemProduceSetting)
    {
        var itemProcessed = ItemsToProcess.Single(x => x.ItemType == itemProduceSetting.ItemsToProduce.First().ItemType);
        itemProcessed.ItemsToProduce--;
    }
    public void FinishProducing(ItemProduceSetting itemProduceSetting) { }

    public float TimeToProduceResourceInSeconds = 5;
    public float TimeToWaitAfterProducingInSeconds = 1.5f;

    public float GetTimeToProduceResourceInSeconds() => TimeToProduceResourceInSeconds;
    public float GetTimeToWaitAfterProducingInSeconds() => TimeToWaitAfterProducingInSeconds;

    public GameObject GetGameObject() => gameObject;
}