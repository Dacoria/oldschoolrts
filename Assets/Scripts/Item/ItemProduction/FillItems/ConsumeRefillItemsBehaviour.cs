using System.Linq;
using System.Collections.Generic;

public class ConsumeRefillItemsBehaviour : MonoBehaviourCI
{
    [ComponentInject] private RefillBehaviour refillBehaviour;
     
    public bool CanConsumeRefillItems(List<ItemAmountBuffer> itemsConsumedToProduce) =>
        CanConsumeRefillItems(itemsConsumedToProduce.ConvertAll(x => (ItemAmount)x));

    private bool CanConsumeRefillItems(List<ItemAmount> itemsConsumedToProduce)
    {
        return itemsConsumedToProduce.All(x =>
             refillBehaviour.StockpileOfItemsRequired.Single(y => y.ItemType == x.ItemType).Amount >= x.Amount
        );
    }

    public bool TryConsumeRefillItems(List<ItemAmountBuffer> itemsConsumedToProduce)
    { 
        if (CanConsumeRefillItems(itemsConsumedToProduce))
        {
            foreach (var itemToConsume in itemsConsumedToProduce)
            {
                var stockpileOfItem = refillBehaviour.StockpileOfItemsRequired.Single(x => x.ItemType == itemToConsume.ItemType);
                stockpileOfItem.Amount = stockpileOfItem.Amount - itemToConsume.Amount;
                refillBehaviour.AddSerfRequestTillBuffer(itemToConsume.ItemType);
            }

            return true;
        }

        return false;
    }
}