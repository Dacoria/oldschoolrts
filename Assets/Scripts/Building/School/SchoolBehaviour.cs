using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SchoolBehaviour : MonoBehaviourCI, ICardBuilding
{
    [ComponentInject] private BuildingBehaviour buildingBehaviour;

    private RefillBehaviour refillBehaviour;
    private ConsumeRefillItemsBehaviour consumeRefillItemsBehaviour;
    private QueueForBuildingBehaviour queueForBuildingBehaviour;
    private ProduceCRBehaviour produceCRBehaviour;

    private new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour), typeof(QueueForBuildingBehaviour), typeof(ProduceCRBehaviour) });

        refillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItemsBehaviour = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
        queueForBuildingBehaviour = gameObject.AddComponent<QueueForBuildingBehaviour>();
        produceCRBehaviour = gameObject.AddComponent<ProduceCRBehaviour>();
    }

    public void AddType(Enum type)
    {
        var villagerType = (VillagerUnitType)type;
        var itemsForProduction = GetItemsToConsumeForProduction(villagerType);
        if (CanProces((VillagerUnitType)type))
        {
            produceCRBehaviour.ProduceOverTime(new ProduceSetup(villagerType,
                produceAction: () => CreateUnit(villagerType)));
        }
        else
        {
            throw new Exception("Zou al gecheckt moeten zijn"); //queue
        }
    }

    public void CreateUnit(VillagerUnitType type)
    {
        var villagerUnitSetting = VillagerPrefabs.Get().Single(x => x.Type == type);
        var villagerVillagerBehaviour = villagerUnitSetting.VillagerBehaviour;            

        if (villagerVillagerBehaviour.IsVillagerWorker())
        {
            ((WorkManager)villagerVillagerBehaviour).VillagerUnitType = villagerUnitSetting.Type;
        }

        var villagerGo = Instantiate(villagerUnitSetting.VillagerBehaviour.GetGO(), gameObject.EntranceExit(), Quaternion.identity);
    }

    private List<ItemAmountBuffer> GetItemsToConsumeForProduction(VillagerUnitType type) => 
        VillagerPrefabs.Get().Single(x => x.Type == type).ItemsConsumedToProduce;
    

    public bool CanProces(Enum type)
    {
        if (!PopulationManager.HasPopulationRoom)
            return false;

        if(!produceCRBehaviour.IsReadyForNextProduction)
            return false;

        var itemsNeeded = GetItemsToConsumeForProduction((VillagerUnitType)type);
        if (!consumeRefillItemsBehaviour.CanConsumeRefillItems(itemsNeeded))
            return false;

        return true;
    }

    public GameObject GetGameObject() => gameObject;
    public TypeProcessing GetCurrentTypeProcessed() => produceCRBehaviour.CurrentTypesProcessed.First();
    public BuildingType GetBuildingType() => buildingBehaviour.BuildingType;
}