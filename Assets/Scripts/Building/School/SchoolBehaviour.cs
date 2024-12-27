using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SchoolBehaviour : MonoBehaviourCI, ICardBuilding, IRefillItems
{

    [ComponentInject] private ConsumeRefillItemsBehaviour ConsumeRefillItemsBehaviour;

    public void AddItem(Enum type)
    {        
        var itemsForProduction = GetItemsToConsumeForProduction((VillagerUnitType)type);
        if (CanProces((VillagerUnitType)type) && ConsumeRefillItemsBehaviour.TryConsumeRefillItems(itemsForProduction))
        {
            Train((VillagerUnitType)type);
        }        
    }

    public void DecreaseItem(Enum type) { }

    public ProductionSetting GetCardDisplaySetting(Enum type) => VillagerPrefabs.Get().First(x => x.Type == (VillagerUnitType)type);


    public int GetCount(Enum type) => 0;

    public void Train(VillagerUnitType type)
    {
        var villagerUnitSetting = VillagerPrefabs.Get().Single(x => x.Type == type);
        if (GameManager.CurrentPopulation < GameManager.PopulationLimit)
        {
            var villagerVillagerBehaviour = villagerUnitSetting.VillagerBehaviour;
            if (villagerVillagerBehaviour.IsVillagerWorker())
            {
                ((WorkManager)villagerVillagerBehaviour).VillagerUnitType = villagerUnitSetting.Type;
            }

            var villagerGo = Instantiate(villagerUnitSetting.VillagerBehaviour.GetGO(), gameObject.EntranceExit(), Quaternion.identity);
        }
    }

    private List<ItemAmountBuffer> GetItemsToConsumeForProduction(VillagerUnitType type)
    {
        return VillagerPrefabs.Get()
            .Single(x => x.Type == type)
            .ItemsConsumedToProduce;
    }

    public bool CanProces(Enum type)
    {
        var itemsNeeded = GetItemsToConsumeForProduction((VillagerUnitType)type);
        var itemsToProduce = VillagerPrefabs.Get().Single(x => x.Type == (VillagerUnitType)type).ConvertToSingleProduceItem();
        return HasPopulationRoom && ConsumeRefillItemsBehaviour.CanConsumeRefillItems(itemsNeeded);
    }
    
    private bool HasPopulationRoom => GameManager.CurrentPopulation < GameManager.PopulationLimit;

    public QueueForBuildingBehaviour GetQueueForBuildingBehaviour() => GetComponent<QueueForBuildingBehaviour>();

    public bool AlwaysRefillItemsIgnoreBuffer() => false;

    public List<ItemProduceSetting> GetItemProduceSettings() =>
        VillagerPrefabs.Get().ConvertAll(x => (ProductionSetting)x).ConvertToSingleProduceItem();

    public GameObject GetGameObject() => gameObject;
}