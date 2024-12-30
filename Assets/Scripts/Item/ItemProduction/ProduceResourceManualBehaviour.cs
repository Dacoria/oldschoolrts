using System;
using System.Collections.Generic;
using System.Linq;

public class ProduceResourceManualBehaviour : ProduceResourceAbstract
{
    private HandleProduceResourceOrderBehaviour handleProduceResourceOrderBehaviour;

    public ItemOutput ItemToProduce; // 1 item manueel te maken per keer; consumeert nooit iets

    new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> {  typeof(HandleProduceResourceOrderBehaviour) });

        handleProduceResourceOrderBehaviour = gameObject.AddComponent<HandleProduceResourceOrderBehaviour>();
    }

    public void ProduceResource()
    {
        handleProduceResourceOrderBehaviour.ProduceItemsNoConsumption(GetResourcesToProduceSettings().First().ItemsToProduce);
    }

    protected override List<ItemProduceSetting> GetConcreteResourcesToProduce() =>
        new List<ItemProduceSetting> { new ItemProduceSetting { ItemsToProduce = new List<ItemOutput> { ItemToProduce } } };

    public int ProducedPerRun => ItemToProduce.ProducedPerProdCycle;
    public int MaxBuffer => ItemToProduce.MaxBuffer;
}
