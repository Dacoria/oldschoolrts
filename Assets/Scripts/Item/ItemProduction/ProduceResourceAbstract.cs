using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ProduceResourceAbstract : BaseAEMonoCI, IResourcesToProduceSettings, IRefillItems
{
    private RefillBehaviour refillBehaviour;
    private ConsumeRefillItemsBehaviour consumeRefillItems;
    [HideInInspector] public HandleProduceResourceOrderBehaviour ProduceResourceOrder; // wordt in concrete classen aangemaakt; daarom in start pas initiaten
    [ComponentInject] protected BuildingBehaviour buildingBehaviour;

    public new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour)});

        refillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItems = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
    }

    private void Start()
    {
        ProduceResourceOrder = GetComponent<HandleProduceResourceOrderBehaviour>();
    }
    public List<ItemProduceSetting> GetItemProduceSettings() => buildingBehaviour.BuildingType.GetItemProduceSettings();

    public ItemProduceSetting GetItemToProduceSettings() => GetItemProduceSettings().FirstOrDefault(x => CanProduceResource(x));

    public bool CanProduceResource() => GetItemToProduceSettings() != null;

    protected virtual bool CanProduceResource(ItemProduceSetting itemProduceSetting)
    {
        if(HasReachedProductionBuffer(itemProduceSetting))
        {
            return false;
        }
        if(!consumeRefillItems.CanConsumeRefillItems(itemProduceSetting.ItemsConsumedToProduce))
        {
            return false;
        }

        return true;
    }

    private bool HasReachedProductionBuffer(ItemProduceSetting itemProduceSetting)
    {     
        if (ProduceResourceOrder == null)
        {
            return false;
        }

        foreach (var itemToProduce in itemProduceSetting.ItemsToProduce)
        {            
            if (ProduceResourceOrder.OutputOrders.Count(x => x.ItemType == itemToProduce.ItemType) + itemToProduce.ProducedPerProdCycle > itemToProduce.MaxBuffer)
            {
                return true;
            }
        }

        return false;
    }

    public bool ConsumeRequiredResources(ItemProduceSetting itemProduceSetting)
    {
        return consumeRefillItems.TryConsumeRefillItems(itemProduceSetting.ItemsConsumedToProduce.ConvertAll(x => (ItemAmount)x));
    }

    public List<ItemOutput> GetAvailableItemsToProduce() => buildingBehaviour.BuildingType.GetItemProduceSettings().SelectMany(x => x.ItemsToProduce).ToList();

    public bool AlwaysRefillItemsIgnoreBuffer() => false;

    public List<ItemProduceSetting> GetResourcesToProduceSettings() => buildingBehaviour.BuildingType.GetItemProduceSettings();
}
