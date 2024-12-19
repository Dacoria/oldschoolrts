using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProduceResourceBehaviour : MonoBehaviour, IResourcesToProduce, IRefillItems
{
    public bool ProduceResourcesOverTimeStandard;
    public List<ItemProduceSetting> ResourcesToProduce;

    private ConsumeRefillItemsBehaviour ConsumeRefillItems;
    private ProduceResourceOrderBehaviour ProduceResourceOrderBehaviour;
    private IProduceResourceOverTime ProduceResourceOverTime;

    public void Start()
    {
        if(ResourcesToProduce.Any(x => x.ItemsConsumedToProduce.Any()))
        {
            gameObject.AddComponent<RefillBehaviour>();
            ConsumeRefillItems = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
        }
        if(GetComponent<ProduceResourceOverTimeBehaviour>() != null)
        {
            ProduceResourcesOverTimeStandard = true; // overschrijf setting als je deze behaviour hebt
        }

        if (ProduceResourcesOverTimeStandard && GetComponent<IProduceResourceOverTime>() == null)
        {
            gameObject.AddComponent<ProduceResourceOverTimeBehaviour>();
        }

        ProduceResourceOverTime = GetComponent<IProduceResourceOverTime>();

        if (GetComponent<ProduceResourceOrderBehaviour>() == null)
        {
            ProduceResourceOrderBehaviour = gameObject.AddComponent<ProduceResourceOrderBehaviour>();
        }
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
        if(ProduceResourceOverTime != null && !ProduceResourceOverTime.CanProduceResource())
        {
            return false;
        }

        if (ConsumeRefillItems != null)
        {
            return ConsumeRefillItems.CanConsumeRefillItems(itemProduceSetting.ItemsConsumedToProduce);
        }
        return true;
    }    

    private bool HasReachedProductionBuffer(ItemProduceSetting itemProduceSetting)
    {
        foreach (var itemToProduce in itemProduceSetting.ItemsToProduce)
        {
            var itemsOutstandingOrders = ProduceResourceOrderBehaviour.OutputOrders.Where(x => x.ItemType == itemToProduce.ItemType);
            if (itemsOutstandingOrders.Count() + itemToProduce.ProducedPerProdCycle > itemToProduce.MaxBuffer)
            {
                return true;
            }
        }

        return false;
    }

    public bool ConsumeRequiredResources(ItemProduceSetting itemProduceSetting)
    {
        if (ConsumeRefillItems != null)
        {
            return ConsumeRefillItems.TryConsumeRefillItems(itemProduceSetting.ItemsConsumedToProduce.ConvertAll(x => (ItemAmount)x));
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
