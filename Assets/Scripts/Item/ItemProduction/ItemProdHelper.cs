using System.Collections.Generic;
using System.Linq;

public static class ItemProdHelper
{
    public static bool HasReachedRscProductionBuffer(ItemProductionSetting itemProduction, HandleProduceResourceOrderBehaviour produceResource)
    {
        return HasReachedRscProductionBuffer(itemProduction.Type, 1, itemProduction.MaxBuffer, produceResource);
    }

    public static bool HasReachedRscProductionBuffer(List<ItemOutput> ItemsToProduce, HandleProduceResourceOrderBehaviour produceResource)
	{
		foreach (var itemToProduce in ItemsToProduce)
		{
            if(HasReachedRscProductionBuffer(itemToProduce.ItemType, itemToProduce.ProducedPerProdCycle, itemToProduce.MaxBuffer, produceResource))
            {
                return true;
            }
		}

		return false;
	}

    private static bool HasReachedRscProductionBuffer(ItemType itemType, int producedPerCycle, int maxBuffer, HandleProduceResourceOrderBehaviour produceResource)
    {
        var totalItems = produceResource.OutputOrders.Count(x => x.ItemType == itemType) + producedPerCycle;
        if (totalItems > maxBuffer)
        {
            return true;
        }        

        return false;
    }
}