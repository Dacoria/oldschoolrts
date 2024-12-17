using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeRefillItemsBehaviour : MonoBehaviour
{
    [ComponentInject]
    private RefillBehaviour RefillBehaviour;

    public void Awake()
    {
        this.ComponentInject();
    }  
    public bool CanConsumeRefillItems(List<ItemAmountBuffer> itemsConsumedToProduce) =>
        CanConsumeRefillItems(itemsConsumedToProduce.ConvertAll(x => (ItemAmount)x));

    private bool CanConsumeRefillItems(List<ItemAmount> itemsConsumedToProduce)
    {
        return itemsConsumedToProduce.All(x =>
             RefillBehaviour.StockpileOfItemsRequired.Single(y => y.ItemType == x.ItemType).Amount >= x.Amount
        );
    }

    public bool TryConsumeRefillItems(List<ItemAmountBuffer> itemsConsumedToProduce)
    {
        return TryConsumeRefillItems(itemsConsumedToProduce.ConvertAll(x => (ItemAmount)x).ToList());
    }

    public bool TryConsumeRefillItems(List<ItemAmount> itemsConsumedToProduce)
    {
        if(CanConsumeRefillItems(itemsConsumedToProduce))
        {
            foreach (var itemToConsume in itemsConsumedToProduce)
            {
                var stockpileOfItem = RefillBehaviour.StockpileOfItemsRequired.Single(x => x.ItemType == itemToConsume.ItemType);
                stockpileOfItem.Amount = stockpileOfItem.Amount - itemToConsume.Amount;
                RefillBehaviour.AddSerfRequestTillBuffer(itemToConsume.ItemType);
            }

            return true;
        }

        return false;
    }
}