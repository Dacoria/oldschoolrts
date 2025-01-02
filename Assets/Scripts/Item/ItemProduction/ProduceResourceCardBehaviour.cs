using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProduceResourceCardBehaviour : MonoBehaviourCI, ICardBuilding
{
    private List<ItemLimit> itemsToProcess;

    [ComponentInject] private BuildingBehaviour buildingBehaviour;
    private RefillBehaviour refillBehaviour;
    private ConsumeRefillItemsBehaviour consumeRefillItemsBehaviour;
    private ProduceCRBehaviour produceCRBehaviour;
    private HandleProduceResourceOrderBehaviour handleProduceResourceOrderBehaviour;

    private new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour), typeof(ProduceCRBehaviour), typeof(HandleProduceResourceOrderBehaviour) });

        refillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItemsBehaviour = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
        produceCRBehaviour = gameObject.AddComponent<ProduceCRBehaviour>();
        handleProduceResourceOrderBehaviour = gameObject.AddComponent<HandleProduceResourceOrderBehaviour>();

        itemsToProcess = buildingBehaviour.BuildingType.GetItemProductionSettings().Select(x => new ItemLimit { ItemType = x.Type }).ToList();
    }

    private void Start()
    {
        StartCoroutine(TryToProduceOverXSeconds());
    }

    public TypeProcessing GetCurrentTypeProcessed() => produceCRBehaviour.CurrentTypesProcessed?.FirstOrDefault();
    public GameObject GetGameObject() => gameObject;
    public BuildingType GetBuildingType() => buildingBehaviour.BuildingType;

    public bool CanProduce(ItemProductionSetting itemProduceSetting)
    {
        if (ItemProdHelper.HasReachedRscProductionBuffer(itemProduceSetting, handleProduceResourceOrderBehaviour))
            return false;

        if (!consumeRefillItemsBehaviour.CanConsumeRefillItems(itemProduceSetting.ItemsConsumedToProduce))
            return false;

        if (!produceCRBehaviour.IsReadyForNextProduction())
            return false;

        return true;
    }

    private IEnumerator TryToProduceOverXSeconds()
    {
        var itemToProduceSettings = GetItemToProduceSettings();
        if (itemToProduceSettings == null)
        {
            yield return Wait4Seconds.Get(0.1f); // kan nog niet produceren, doe check opnieuw na x secondes
            StartCoroutine(TryToProduceOverXSeconds());
        }
        else
        {
            var itemToProduce = (ItemType)itemToProduceSettings.GetType();
            consumeRefillItemsBehaviour.TryConsumeRefillItems(itemToProduceSettings.ItemsConsumedToProduce);
            produceCRBehaviour.ProduceOverTime(new ProduceSetup(
                itemToProduce,
                handleProduceResourceOrderBehaviour,
                produceCallback: () => {
                    DecreaseType(itemToProduce);
                    lastItemProduced = itemToProduce;
                },
                waitAfterProduceCallback: () => StartCoroutine(TryToProduceOverXSeconds())));
        }
    }

    private ItemType? lastItemProduced;
    private ItemProductionSetting GetItemToProduceSettings()
    {
        var allItemProductionSettings = buildingBehaviour.BuildingType.GetItemProductionSettings();

        if (!lastItemProduced.HasValue)
        {
            // geen eerdere waarde? pak hoogste qua aantal
            foreach (var itemToProcess in itemsToProcess.Where(x => x.ItemAmountToProduce >= 1).OrderByDescending(x => x.ItemAmountToProduce))
            {
                var itemProductionSetting = allItemProductionSettings.Single(x => x.Type == itemToProcess.ItemType);
                if (CanProduce(itemProductionSetting))
                {
                    return itemProductionSetting;
                }
            }
        }
        else
        {
            // wel eerder een waarde? ga 1 enumeratie verder
            var currEnum = lastItemProduced.Value.Next();
            while (currEnum != lastItemProduced.Value)
            {
                var itemProductionSetting = allItemProductionSettings.FirstOrDefault(x => x.Type == currEnum);
                if (itemProductionSetting != null)
                { 
                    if (itemsToProcess.Single(x => x.ItemType == currEnum).ItemAmountToProduce >= 1 && CanProduce(itemProductionSetting))
                    {
                        return itemProductionSetting;
                    }
                }
                
                currEnum = currEnum.Next();
            }
        }        

        return null;
    }

    public int GetCount(Enum type) => itemsToProcess.Single(x => x.ItemType == (ItemType)type).ItemAmountToProduce;

    public void AddType(Enum type)
    {
        itemsToProcess.Single(x => x.ItemType == (ItemType)type).ItemAmountToProduce++;
    }

    public void DecreaseType(Enum type)
    {
        var itemToProcess = itemsToProcess.Single(x => x.ItemType == (ItemType)type);
        if (itemToProcess.ItemAmountToProduce > 0)
            itemToProcess.ItemAmountToProduce --;
    }

    public bool CanProces(Enum type) =>
        CanProduce(buildingBehaviour.BuildingType.GetItemProductionSettings().Single(x => x.Type == (ItemType)type));
}