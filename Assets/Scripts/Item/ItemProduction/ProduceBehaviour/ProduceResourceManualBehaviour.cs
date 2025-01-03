using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProduceResourceManualBehaviour : MonoBehaviourCI, ICardOneProdBuilding
{
    private ItemOutput itemToProduce => buildingBehaviour.BuildingType.GetItemProduceSettings().First().ItemsToProduce.First();

    [ComponentInject] private BuildingBehaviour buildingBehaviour;
    private RefillBehaviour refillBehaviour;
    private ConsumeRefillItemsBehaviour consumeRefillItemsBehaviour;
    private ProduceCRBehaviour produceCRBehaviour;
    private HandleProduceResourceOrderBehaviour handleProduceResourceOrderBehaviour;

    private new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour), typeof(ProduceCRBehaviour), typeof(HandleProduceResourceOrderBehaviour) });

        refillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItemsBehaviour = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
        produceCRBehaviour = gameObject.AddComponent<ProduceCRBehaviour>();
        handleProduceResourceOrderBehaviour = gameObject.AddComponent<HandleProduceResourceOrderBehaviour>();
    }

    public void ProduceResource()
    {
        produceCRBehaviour.ProduceInstant(new ProduceSetup(itemToProduce, handleProduceResourceOrderBehaviour));
    }

    // gebruiken in manuele gebouwen? voor nu niet
    public bool CanProduce(ItemProduceSetting itemProduceSetting)
    {
        if (ItemProdHelper.HasReachedRscProductionBuffer(itemProduceSetting.ItemsToProduce, handleProduceResourceOrderBehaviour))
            return false;

        if (!produceCRBehaviour.IsReadyForNextProduction())
            return false;

        return true;
    }

    public int ProducedPerRun => itemToProduce.ProducedPerProdCycle;
    public int MaxBuffer => itemToProduce.MaxBuffer;
    public GameObject GetGameObject() => gameObject;
    public List<TypeProcessing> GetCurrentTypesProcessed() => produceCRBehaviour.CurrentTypesProcessed;
}
