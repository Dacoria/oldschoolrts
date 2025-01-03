using System;
using System.Collections.Generic;

public class ProduceSetup
{
    public ProduceSetup(ItemOutput produceType, IProduce produceAction, Action produceCallback = null, Action waitAfterProduceCallback = null)
    {
        ProduceTypes = new List<Enum> ();
        for (int i = 0; i < produceType.ProducedPerProdCycle; i ++)
        {
            ProduceTypes.Add(produceType.ItemType);
        }

        ProduceAction = produceAction;
        ProduceCallback = produceCallback;
        WaitAfterProduceCallback = waitAfterProduceCallback;
    }

    public ProduceSetup(List<ItemOutput> produceTypes, IProduce produceAction, Action produceCallback = null, Action waitAfterProduceCallback = null)
    {
        ProduceTypes = new List<Enum>();
        foreach (var produceType in produceTypes)
        {
            for (int i = 0; i < produceType.ProducedPerProdCycle; i++)
            {
                ProduceTypes.Add(produceType.ItemType);
            }
        }

        ProduceAction = produceAction;
        ProduceCallback = produceCallback;
        WaitAfterProduceCallback = waitAfterProduceCallback;
    }

    public ProduceSetup(Enum produceType, IProduce produceAction, Action produceCallback = null, Action waitAfterProduceCallback = null)
    {
        ProduceTypes = new List<Enum> { produceType };
        ProduceAction = produceAction;
        ProduceCallback = produceCallback;
        WaitAfterProduceCallback = waitAfterProduceCallback;
    }

    public List<Enum> ProduceTypes;
    public IProduce ProduceAction;
    public Action ProduceCallback;
    public Action WaitAfterProduceCallback;
}