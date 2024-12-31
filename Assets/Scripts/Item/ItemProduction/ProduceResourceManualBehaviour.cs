using System;
using System.Collections.Generic;
using System.Linq;

public class ProduceResourceManualBehaviour : ProduceResourceAbstract
{
    private HandleProduceResourceOrderBehaviour handleProduceResourceOrderBehaviour;

    private ItemOutput itemToProduce;

    new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<Type> {  typeof(HandleProduceResourceOrderBehaviour) });

        handleProduceResourceOrderBehaviour = gameObject.AddComponent<HandleProduceResourceOrderBehaviour>();
        itemToProduce = buildingBehaviour.BuildingType.GetItemProduceSettings().First().ItemsToProduce.Single();
    }

    public void ProduceResource()
    {
        handleProduceResourceOrderBehaviour.ProduceItemsNoConsumption(new List<ItemOutput> { itemToProduce });
    }    

    public int ProducedPerRun => itemToProduce.ProducedPerProdCycle;
    public int MaxBuffer => itemToProduce.MaxBuffer;
}
