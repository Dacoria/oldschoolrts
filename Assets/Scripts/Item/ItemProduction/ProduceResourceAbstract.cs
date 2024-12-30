using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ProduceResourceAbstract : MonoBehaviour, IResourcesToProduceSettings, IRefillItems
{
    private RefillBehaviour refillBehaviour;
    private ConsumeRefillItemsBehaviour consumeRefillItems;
    [HideInInspector] public HandleProduceResourceOrderBehaviour ProduceResourceOrder; // wordt in concrete classen aangemaakt; daarom in start pas initiaten

    protected abstract List<ItemProduceSetting> GetConcreteResourcesToProduce();
    public List<ItemProduceSetting> GetResourcesToProduceSettings() => GetConcreteResourcesToProduce();

    public void Awake()
    {
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour)});

        refillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItems = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
    }

    private void Start()
    {
        ProduceResourceOrder = GetComponent<HandleProduceResourceOrderBehaviour>();
    }

    public List<ItemProduceSetting> GetItemProduceSettings() => GetResourcesToProduceSettings();
    public bool CanProduceResource() => GetItemToProduceSettings() != null;

    public ItemProduceSetting GetItemToProduceSettings() => GetResourcesToProduceSettings().FirstOrDefault(x => CanProduceResource(x));    

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

    public List<ItemOutput> GetAvailableItemsToProduce() => GetResourcesToProduceSettings().SelectMany(x => x.ItemsToProduce).ToList();
}
