using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProduceResourceCardBehaviour : MonoBehaviourCI, ICardBuilding, IResourceProduction
{
    private List<ItemLimit> ItemsToProcess;

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
    }

    private void Start()
    {
        StartCoroutine(TryToProduceOverXSeconds());
    }

    public TypeProcessing GetCurrentTypeProcessed() => produceCRBehaviour.CurrentTypesProcessed?.FirstOrDefault();
    public GameObject GetGameObject() => gameObject;
    public BuildingType GetBuildingType() => buildingBehaviour.BuildingType;

    public bool CanProduce(ItemProduceSetting itemProduceSetting)
    {
        if (ItemProdHelper.HasReachedRscProductionBuffer(itemProduceSetting.ItemsToProduce, handleProduceResourceOrderBehaviour))
            return false;

        if (!consumeRefillItemsBehaviour.CanConsumeRefillItems(itemProduceSetting.ItemsConsumedToProduce))
            return false;

        if (!produceCRBehaviour.IsReadyForNextProduction)
            return false;

        return true;
    }

    private IEnumerator TryToProduceOverXSeconds()
    {
        var itemToProduceSettings = this.GetItemToProduceSettings(buildingBehaviour);
        if (itemToProduceSettings == null)
        {
            yield return Wait4Seconds.Get(0.1f); // kan nog niet produceren, doe check opnieuw na x secondes
            StartCoroutine(TryToProduceOverXSeconds());
        }
        else
        {
            consumeRefillItemsBehaviour.TryConsumeRefillItems(itemToProduceSettings.ItemsConsumedToProduce);

            var itemToProduce = itemToProduceSettings.ItemsToProduce.First();
            produceCRBehaviour.ProduceOverTime(new ProduceSetup(
                itemToProduceSettings.ItemsToProduce,
                produceAction: () => {
                    handleProduceResourceOrderBehaviour.ProduceItem(itemToProduce);
                    ItemsToProcess.Single(x => x.ItemType == itemToProduce.ItemType).ItemAmountToProduce.DecreaseOneNoNegative();
                },
                waitAfterProduceAction: () => StartCoroutine(TryToProduceOverXSeconds())));
        }
    }    

    public int GetCount(Enum type) => ItemsToProcess.Single(x => x.ItemType == (ItemType)type).ItemAmountToProduce;

    public void AddType(Enum type)
    {
        ItemsToProcess.Single(x => x.ItemType == (ItemType)type).ItemAmountToProduce++;
    }

    public void DecreaseType(Enum type)
    {
        ItemsToProcess.Single(x => x.ItemType == (ItemType)type).ItemAmountToProduce.DecreaseOneNoNegative();
    }

    public bool CanProces(Enum type) => 
        CanProduce(buildingBehaviour.BuildingType.GetItemProduceSettings().Single(x => x.ItemsToProduce.Any(y => y.ItemType == (ItemType)type)));
}