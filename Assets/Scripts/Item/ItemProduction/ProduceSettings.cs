using System;
using System.Collections.Generic;

public class ProduceSetup
{
    public ProduceSetup(ItemOutput produceType, Action produceAction, Action waitAfterProduceAction = null)
    {
        ProduceTypes = new List<Enum> { produceType.ItemType };
        ProduceAction = produceAction;
        WaitAfterProduceAction = waitAfterProduceAction;
    }

    public ProduceSetup(List<ItemOutput> produceTypes, Action produceAction, Action waitAfterProduceAction = null)
    {
        ProduceTypes = produceTypes.ConvertAll<Enum>(x => x.ItemType);
        ProduceAction = produceAction;
        WaitAfterProduceAction = waitAfterProduceAction;
    }

    public ProduceSetup(Enum produceType, Action produceAction, Action waitAfterProduceAction = null)
    {
        ProduceTypes = new List<Enum> { produceType };
        ProduceAction = produceAction;
        WaitAfterProduceAction = waitAfterProduceAction;
    }

    public List<Enum> ProduceTypes;
    public Action ProduceAction;
    public Action WaitAfterProduceAction;
}