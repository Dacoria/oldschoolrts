using System;
using System.Collections.Generic;


[Serializable]
public class ItemProduceSetting
{
    public List<ItemOutput> ItemsToProduce;
    
    public List<ItemAmountBuffer> ItemsConsumedToProduce;
}