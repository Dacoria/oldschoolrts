using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ProduceResourceAbstract : MonoBehaviour, IResourcesToProduce, IRefillItems
{
    public abstract List<ItemProduceSetting> GetResourcesToProduce();

    private RefillBehaviour refillBehaviour;
    private ConsumeRefillItemsBehaviour consumeRefillItems;

    public void Start()
    {
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour)});

        refillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItems = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
    }

    public List<ItemProduceSetting> GetItemProduceSettings() => GetResourcesToProduce();
    public bool CanProduceResource() => GetItemToProduce() != null;

    public ItemProduceSetting GetItemToProduce() => GetResourcesToProduce().FirstOrDefault(x => CanProduceResource(x));    

    protected virtual bool CanProduceResource(ItemProduceSetting itemProduceSetting)
    {
        if(HasReachedProductionBuffer(itemProduceSetting))
        {
            return false;
        }
        if (consumeRefillItems != null)
        {
            return consumeRefillItems.CanConsumeRefillItems(itemProduceSetting.ItemsConsumedToProduce);
        }
        return true;
    }

    private HandleProduceResourceOrderBehaviour produceBehaviour;
    private bool HasReachedProductionBuffer(ItemProduceSetting itemProduceSetting)
    {
        if(produceBehaviour == null)
        {
            produceBehaviour = GetComponent<HandleProduceResourceOrderBehaviour>();
        }

        foreach (var itemToProduce in itemProduceSetting.ItemsToProduce)
        {
            var outputOrders = produceBehaviour.OutputOrders;
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

    public List<ItemOutput> GetAvailableItemsToProduce() => GetResourcesToProduce().SelectMany(x => x.ItemsToProduce).ToList();
}
