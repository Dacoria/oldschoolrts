using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ItemProdHelper
{
    public static bool HasReachedRscProductionBuffer(List<ItemOutput> ItemsToProduce, HandleProduceResourceOrderBehaviour produceResource)
	{
		foreach (var itemToProduce in ItemsToProduce)
		{
			var totalItems = produceResource.OutputOrders.Count(x => x.ItemType == itemToProduce.ItemType) + itemToProduce.ProducedPerProdCycle;
			if (totalItems > itemToProduce.MaxBuffer)
			{
				return true;
			}
		}

		return false;
	}

    public static ItemProduceSetting GetItemToProduceSettings(this IResourceProduction rscProduction, BuildingBehaviour buildingBehaviour) =>
        buildingBehaviour.BuildingType.GetItemProduceSettings()
        .Where(x => rscProduction.CanProduce(x))
        .OrderBy(x => x.ItemsToProduce.Count)
        .FirstOrDefault();
}