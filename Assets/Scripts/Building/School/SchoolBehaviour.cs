using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SchoolBehaviour : MonoBehaviour, ICardBuilding, IRefillItems
{
    private ConsumeRefillItemsBehaviour consumeRefillItemsBehaviour;
    private QueueForBuildingBehaviour queueForBuildingBehaviour;

    public float BuildUnitDurationInSeconds;

    private void Awake()
    {
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<Type> { typeof(ConsumeRefillItemsBehaviour), typeof(QueueForBuildingBehaviour) });

        consumeRefillItemsBehaviour = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
        queueForBuildingBehaviour = gameObject.AddComponent<QueueForBuildingBehaviour>();
    }

    public void AddItem(Enum type)
    {        
        var itemsForProduction = GetItemsToConsumeForProduction((VillagerUnitType)type);
        if (CanProces((VillagerUnitType)type) && consumeRefillItemsBehaviour.TryConsumeRefillItems(itemsForProduction))
        {
            Train((VillagerUnitType)type);
        }        
    }

    public void DecreaseItem(Enum type) { }
    public float GetProductionTime(Enum type) => BuildUnitDurationInSeconds;

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
        return HasPopulationRoom && consumeRefillItemsBehaviour.CanConsumeRefillItems(itemsNeeded);
    }
    
    private bool HasPopulationRoom => GameManager.CurrentPopulation < GameManager.PopulationLimit;

    public QueueForBuildingBehaviour GetQueueForBuildingBehaviour() => queueForBuildingBehaviour;

    public bool AlwaysRefillItemsIgnoreBuffer() => false;

    public List<ItemProduceSetting> GetItemProduceSettings() =>
        VillagerPrefabs.Get().ConvertAll(x => (ProductionSetting)x).ConvertToSingleProduceItem();

    public GameObject GetGameObject() => gameObject;    
}