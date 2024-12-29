using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProduceResourceOverTimeBehaviour : MonoBehaviour, IResourcesToProduce, IRefillItems, IProduceResourceOverTimeDurations
{
    public List<ItemProduceSetting> ResourcesToProduce;

    private ConsumeRefillItemsBehaviour consumeRefillItems;
    private HandleAutoProduceResourceOrderOverTimeBehaviour handleAutoProduceResourceOrderOverTimeBehaviour;

    public float ProduceTimeInSeconds;
    public float TimeToProduceResourceInSeconds => ProduceTimeInSeconds;
    public float WaitAfterProduceTimeInSeconds = 2;
    public float TimeToWaitAfterProducingInSeconds => WaitAfterProduceTimeInSeconds;

    public void Start()
    {
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour), typeof(HandleAutoProduceResourceOrderOverTimeBehaviour) });

        gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItems = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
        handleAutoProduceResourceOrderOverTimeBehaviour = gameObject.AddComponent<HandleAutoProduceResourceOrderOverTimeBehaviour>();
    }

    public List<ItemProduceSetting> GetItemProduceSettings() => ResourcesToProduce;
    public bool CanProduceResource() => GetItemToProduce() != null;

    public ItemProduceSetting GetItemToProduce() => ResourcesToProduce.FirstOrDefault(x => CanProduceResource(x));    

    private bool CanProduceResource(ItemProduceSetting itemProduceSetting)
    {
        if(HasReachedProductionBuffer(itemProduceSetting))
        {
            return false;
        }
        if(ResourcesToProduce.Any(x => !consumeRefillItems.CanConsumeRefillItems(x.ItemsConsumedToProduce)))
        {
            return false;
        }
        if (consumeRefillItems != null)
        {
            return consumeRefillItems.CanConsumeRefillItems(itemProduceSetting.ItemsConsumedToProduce);
        }
        return true;
    }    

    private bool HasReachedProductionBuffer(ItemProduceSetting itemProduceSetting)
    {
        foreach (var itemToProduce in itemProduceSetting.ItemsToProduce)
        {
            var outputOrders = handleAutoProduceResourceOrderOverTimeBehaviour.HandleProduceResourceOrderOverTimeBehaviour.ProduceResourceOrderBehaviour.OutputOrders;
            var itemsOutstandingOrders = outputOrders.Where(x => x.ItemType == itemToProduce.ItemType);
            if (itemsOutstandingOrders.Count() + itemToProduce.ProducedPerProdCycle > itemToProduce.MaxBuffer)
            {
                return true;
            }
        }

        return false;
    }

    public bool ConsumeRequiredResources(ItemProduceSetting itemProduceSetting)
    {
        if (consumeRefillItems != null)
        {
            return consumeRefillItems.TryConsumeRefillItems(itemProduceSetting.ItemsConsumedToProduce.ConvertAll(x => (ItemAmount)x));
        }
        return true;
    }

    public bool AlwaysRefillItemsIgnoreBuffer() => false;

    public List<ItemOutput> GetAvailableItemsToProduce() => ResourcesToProduce.SelectMany(x => x.ItemsToProduce).ToList();

    public ItemOutput GetSingleProducingItemWithoutConsuming()
    {
        if( ResourcesToProduce.Count == 1 &&
            !ResourcesToProduce.First().ItemsConsumedToProduce.Any() &&
            ResourcesToProduce.First().ItemsToProduce.Count == 1
        )
        {
            return ResourcesToProduce.First().ItemsToProduce.First();
        }

        return null;
    }

    public bool IsSingleProducingItemWithoutConsuming() => GetSingleProducingItemWithoutConsuming() != null;
}
