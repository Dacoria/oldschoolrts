using System;
using System.Collections.Generic;
using System.Linq;

public class ProduceResourceManualBehaviour : ProduceResourceAbstract
{
    private HandleProduceResourceOrderBehaviour handleProduceResourceOrderBehaviour;

    public ItemOutput ItemToProduce; // 1 item manueel te maken per keer; consumeert nooit iets
    public override List<ItemProduceSetting> GetResourcesToProduce() =>
        new List<ItemProduceSetting> { new ItemProduceSetting { ItemsToProduce = new List<ItemOutput> { ItemToProduce } } };

    public new void Start()
    {
        base.Start();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> {  typeof(HandleProduceResourceOrderBehaviour) });

        handleProduceResourceOrderBehaviour = gameObject.AddComponent<HandleProduceResourceOrderBehaviour>();
    }   

    public void ProduceResource()
    {
        handleProduceResourceOrderBehaviour.ProduceItems(GetResourcesToProduce().First());
    }

    public int ProducedPerRun => ItemToProduce.ProducedPerProdCycle;
    public int MaxBuffer => ItemToProduce.MaxBuffer;
}
