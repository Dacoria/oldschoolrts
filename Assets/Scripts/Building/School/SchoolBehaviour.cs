using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SchoolBehaviour : MonoBehaviourCI, ICardSelectProdBuilding, IProduce
{
    [ComponentInject] private BuildingBehaviour buildingBehaviour;

    private RefillBehaviour refillBehaviour;
    private ConsumeRefillItemsBehaviour consumeRefillItemsBehaviour;
    private QueueForBuildingBehaviour queueForBuildingBehaviour;
    private ProduceCRBehaviour produceCRBehaviour;

    // via start -> zorgt dat bij real activeren, geen nieuwe comp. worden aangemaakt
    private void Start()
    {
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
        if (VillagerManager.ToggleInstaFreeVillagers_Active)
        {
            produceCRBehaviour.ProduceInstant(new ProduceSetup(villagerType, this));
        }
        else
        {
            var itemsForProduction = GetItemsToConsumeForProduction(villagerType);
            if (CanProces((VillagerUnitType)type))
            {
                consumeRefillItemsBehaviour.TryConsumeRefillItems(itemsForProduction);
                produceCRBehaviour.ProduceOverTime(new ProduceSetup(villagerType, this));
            }
            else
            {
                throw new Exception("Zou al gecheckt moeten zijn"); //queue
            }
        }
    }

    public void Produce(List<Enum> types)
    {
        foreach (var type in types)
        {
            var villagerUnitSetting = VillagerPrefabs.Get().Single(x => x.Type == (VillagerUnitType)type);
            var villagerVillagerBehaviour = villagerUnitSetting.VillagerBehaviour;

            if (villagerVillagerBehaviour.IsVillagerWorker())
            {
                ((WorkManager)villagerVillagerBehaviour).VillagerUnitType = villagerUnitSetting.Type;
            }

            var villagerGo = Instantiate(villagerUnitSetting.VillagerBehaviour.GetGO(), gameObject.EntranceExit(), Quaternion.identity);
        }
    }

    private List<ItemAmountBuffer> GetItemsToConsumeForProduction(VillagerUnitType type) => 
        VillagerPrefabs.Get().Single(x => x.Type == type).ItemsConsumedToProduce;
    

    public bool CanProces(Enum type)
    {
        if (!PopulationManager.HasPopulationRoom)
            return false;

        if(!produceCRBehaviour.IsReadyForNextProduction())
            return false;

        var itemsNeeded = GetItemsToConsumeForProduction((VillagerUnitType)type);
        if (!consumeRefillItemsBehaviour.CanConsumeRefillItems(itemsNeeded))
            return false;

        return true;
    }

    public GameObject GetGameObject() => gameObject;
    public TypeProcessing GetCurrentTypeProcessed() => produceCRBehaviour?.CurrentTypesProcessed?.FirstOrDefault();
    public BuildingType GetBuildingType() => buildingBehaviour.BuildingType;    
}