using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardItemsProduceBehaviour : MonoBehaviourCI, ICardBuilding, IResourcesToProduceSettings, IRefillItems
{
    [ComponentInject] private BuildingBehaviour buildingBehaviour;
    private RefillBehaviour refillBehaviour;
    private ConsumeRefillItemsBehaviour consumeRefillItems;
    private HandleProduceResourceOrderOverTimeBehaviour handleProduceResourceOrderOverTimeBehaviour;

    private List<ItemProductionSetting> ItemProductionSettings => buildingBehaviour.BuildingType.GetItemProductionSettings();
    private List<ItemLimit> ItemsToProcess;

    new void Awake()
    {
        base.Awake();
        ItemsToProcess = ItemProductionSettings.Select(x => new ItemLimit { ItemType = x.Type }).ToList(); // initieel zetten; dan weet je welke items je kan maken
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour), typeof(HandleProduceResourceOrderOverTimeBehaviour) });

        refillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItems = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
        handleProduceResourceOrderOverTimeBehaviour = gameObject.AddComponent<HandleProduceResourceOrderOverTimeBehaviour>();

        handleProduceResourceOrderOverTimeBehaviour.StartedProducingAction += OnStartedProducingAction;
    }

    private void OnDestroy()
    {
        handleProduceResourceOrderOverTimeBehaviour.StartedProducingAction -= OnStartedProducingAction;
    }

    private void OnStartedProducingAction(List<ItemOutput> itemsProducing) 
    {
        var itemProcessed = ItemsToProcess.Single(x => x.ItemType == itemsProducing.First().ItemType);
        itemProcessed.ItemsToProduce--;
    }

    public List<ItemProduceSetting> GetItemProduceSettings() =>
        ItemProductionSettings.ConvertAll(x => (ProductionSetting)x).ConvertToSingleProduceItem();

    private bool CanProduceResource(ItemProductionSetting itemProductionSetting)
    {
        if (HasReachedProductionBuffer(itemProductionSetting))
        {
            return false;
        }
        if (!consumeRefillItems.CanConsumeRefillItems(itemProductionSetting.ItemsConsumedToProduce))
        {
            return false;
        }

        return true;
    }

    // bepaalt welk item eerst --> Logica nu: meeste aantal eerst (daarna lijstvolgorde)
    public ItemProduceSetting GetItemToProduceSettings()
    {
        var itemsThatCanBeProduced = ItemProductionSettings.Where(x => CanProduceResource(x) && !HasReachedProductionBuffer(x));

        foreach (var itemToProcess in ItemsToProcess
            .Where(x => x.ItemsToProduce >= 1)
            .OrderByDescending(x => x.ItemsToProduce))
        {
            var itemToProduce = itemsThatCanBeProduced.FirstOrDefault(x => x.Type == itemToProcess.ItemType);
            if (itemToProduce != null)
            {
                return itemToProduce.ConvertToSingleProduceItem();
            }
        }

        return null;
    }

    private bool HasReachedProductionBuffer(ItemProductionSetting itemProductionSetting)
    {
        foreach (var itemToProduce in itemProductionSetting.ItemsConsumedToProduce)
        {
            var outputOrders = handleProduceResourceOrderOverTimeBehaviour.ProduceResourceOrderBehaviour.OutputOrders;
            var outstandingOrders = outputOrders.Where(x => x.ItemType == itemToProduce.ItemType);
            if (outstandingOrders.Count() + itemToProduce.Amount > itemToProduce.MaxBuffer)
            {
                return true;
            }
        }

        return false;
    }    

    public bool ConsumeRequiredResources(ItemProduceSetting itemProduceSetting) =>
        consumeRefillItems.TryConsumeRefillItems(itemProduceSetting.ItemsConsumedToProduce.ConvertAll(x => (ItemAmount)x));

    public bool AlwaysRefillItemsIgnoreBuffer() => false;

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
        return consumeRefillItems.CanConsumeRefillItems(itemsToConsume);
    }

    public GameObject GetGameObject() => gameObject;
    public float GetProductionTime(Enum type)
    {        
        return buildingBehaviour.BuildingType.GetProductionDurationSettings().TimeToProduceResourceInSeconds; // momenteel geen verschil per type
    }

    public List<ItemProduceSetting> GetResourcesToProduceSettings() => new List<ItemProduceSetting>(); // alleen voor display buiten kaarten

    public UIItemProcessing GetCurrentItemProcessed()
    {
        if(!handleProduceResourceOrderOverTimeBehaviour.IsProducingResourcesRightNow)
        {
            return null;
        }
        var itemtype = handleProduceResourceOrderOverTimeBehaviour.ItemsBeingProduced.First().ItemType;
        return new UIItemProcessing
        {
             StartTimeBeingBuild = handleProduceResourceOrderOverTimeBehaviour.StartTimeProducing,
             Type = itemtype
        };
    }
}