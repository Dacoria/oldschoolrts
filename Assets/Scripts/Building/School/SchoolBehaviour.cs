using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SchoolBehaviour : MonoBehaviourCI, ICardBuilding, IRefillItems
{
    public List<VillagerUnitSetting> VillagerUnitSettings;

    [ComponentInject]
    private ConsumeRefillItemsBehaviour ConsumeRefillItemsBehaviour;

    public void AddItem(Enum type)
    {        
        var itemsForProduction = GetItemsToConsumeForProduction((VillagerUnitType)type);
        if (CanProces((VillagerUnitType)type) && ConsumeRefillItemsBehaviour.TryConsumeRefillItems(itemsForProduction))
        {
            Train((VillagerUnitType)type);
        }        
    }

    public void DecreaseItem(Enum type) { }

    public ProductionSetting GetCardDisplaySetting(Enum type) => VillagerUnitSettings.First(x => x.Type == (VillagerUnitType)type);


    public int GetCount(Enum type) => 0;

    public void Train(VillagerUnitType type)
    {
        var villagerUnitSetting = VillagerUnitSettings.Single(x => x.Type == type);
        if (GameManager.CurrentPopulation < GameManager.PopulationLimit)
        {
            var villagerGo = Instantiate(villagerUnitSetting.ResourcePrefab, gameObject.EntranceExit(), Quaternion.identity);
            if(villagerGo.GetComponent<WorkManager>() != null)
            {
                villagerGo.GetComponent<WorkManager>().VillagerUnitType = villagerUnitSetting.Type;
                villagerGo.AddComponent<WarningDisplayAboveHeadBehaviour>();
            }

            AE.VillagerUnitCreated?.Invoke(villagerUnitSetting.Type);
        }
    }

    private List<ItemAmountBuffer> GetItemsToConsumeForProduction(VillagerUnitType type)
    {
        return VillagerUnitSettings
            .Single(x => x.Type == type)
            .ItemsConsumedToProduce;
    }

    public bool CanProces(Enum type)
    {
        var itemsNeeded = GetItemsToConsumeForProduction((VillagerUnitType)type);
        var itemsToProduce = VillagerUnitSettings.Single(x => x.Type == (VillagerUnitType)type).ConvertToSingleProduceItem();
        return HasPopulationRoom && ConsumeRefillItemsBehaviour.CanConsumeRefillItems(itemsNeeded);
    }
    
    private bool HasPopulationRoom => GameManager.CurrentPopulation < GameManager.PopulationLimit;

    public QueueForBuildingBehaviour GetQueueForBuildingBehaviour() => GetComponent<QueueForBuildingBehaviour>();

    public bool AlwaysRefillItemsIgnoreBuffer() => false;

    public List<ItemProduceSetting> GetItemProduceSettings() =>
        VillagerUnitSettings.ConvertAll(x => (ProductionSetting)x).ConvertToSingleProduceItem();

    public GameObject GetGameObject() => gameObject;
}