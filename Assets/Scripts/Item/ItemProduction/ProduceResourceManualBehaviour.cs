using System;
using System.Collections.Generic;
using System.Linq;

public class ProduceResourceManualBehaviour : MonoBehaviourCI, IResourceProduction
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
        produceCRBehaviour.ProduceOverTime(new ProduceSetup(itemToProduce,
            produceAction: () => handleProduceResourceOrderBehaviour.ProduceItem(itemToProduce)));
        // geen waitlogica; onnodig complex (hoop ik)
    }

    public bool CanProduce(ItemProduceSetting itemProduceSetting)
    {
        if (ItemProdHelper.HasReachedRscProductionBuffer(itemProduceSetting.ItemsToProduce, handleProduceResourceOrderBehaviour))
            return false;

        if (!produceCRBehaviour.IsReadyForNextProduction)
            return false;

        return true;
    }

    public int ProducedPerRun => itemToProduce.ProducedPerProdCycle;
    public int MaxBuffer => itemToProduce.MaxBuffer;
}
