using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Assets;

public class SchoolBehaviour : MonoBehaviourCI, ICardBuilding, IRefillItems
{
    [ComponentInject] private BuildingBehaviour buildingBehaviour;

    private RefillBehaviour refillBehaviour;
    private ConsumeRefillItemsBehaviour consumeRefillItemsBehaviour;
    private QueueForBuildingBehaviour queueForBuildingBehaviour;

    private new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour), typeof(QueueForBuildingBehaviour) });

        refillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItemsBehaviour = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
        queueForBuildingBehaviour = gameObject.AddComponent<QueueForBuildingBehaviour>();
    }

    public void AddType(Enum type)
    {        
        var itemsForProduction = GetItemsToConsumeForProduction((VillagerUnitType)type);
        if (CanProces((VillagerUnitType)type) && consumeRefillItemsBehaviour.TryConsumeRefillItems(itemsForProduction))
        {
            StartCoroutine(StartUnitCreationProcess((VillagerUnitType)type));            
        }        
    }

    private UIItemProcessing currentItemProcessed;
    public UIItemProcessing GetCurrentItemProcessed() => currentItemProcessed;

    private IEnumerator StartUnitCreationProcess(VillagerUnitType type)
    {
        var produceDurations = buildingBehaviour.BuildingType.GetProductionDurationSettings();
        currentItemProcessed = new UIItemProcessing { StartTimeBeingBuild = DateTime.Now, Type = type };
        AE.StartedProducingAction?.Invoke(buildingBehaviour, null);
        yield return Wait4Seconds.Get(produceDurations.TimeToProduceResourceInSeconds);

        CreateUnit(type);
        AE.FinishedProducingAction?.Invoke(buildingBehaviour, null);

        yield return Wait4Seconds.Get(produceDurations.TimeToWaitAfterProducingInSeconds);

        AE.FinishedWaitingAfterProducingAction?.Invoke(buildingBehaviour);
        currentItemProcessed = null;
    }

    public void DecreaseType(Enum type) { }
    public float GetProductionTime(Enum type) => buildingBehaviour.BuildingType.GetProductionDurationSettings().TimeToProduceResourceInSeconds; // 1 waarde voor alles

    public ProductionSetting GetCardDisplaySetting(Enum type) => VillagerPrefabs.Get().First(x => x.Type == (VillagerUnitType)type);

    public int GetCount(Enum type) => 0;

    public void CreateUnit(VillagerUnitType type)
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

    private List<ItemAmountBuffer> GetItemsToConsumeForProduction(VillagerUnitType type) => 
        VillagerPrefabs.Get()
        .Single(x => x.Type == type)
        .ItemsConsumedToProduce;
    

    public bool CanProces(Enum type)
    {
        var itemsNeeded = GetItemsToConsumeForProduction((VillagerUnitType)type);
        var itemsToProduce = VillagerPrefabs.Get().Single(x => x.Type == (VillagerUnitType)type).ConvertToSingleProduceItem();
        return currentItemProcessed == null && HasPopulationRoom && consumeRefillItemsBehaviour.CanConsumeRefillItems(itemsNeeded);
    }
    
    private bool HasPopulationRoom => GameManager.CurrentPopulation < GameManager.PopulationLimit;

    public bool AlwaysRefillItemsIgnoreBuffer() => false;

    public List<ItemProduceSetting> GetItemProduceSettings() =>
        VillagerPrefabs.Get().ConvertAll(x => (ProductionSetting)x).ConvertToSingleProduceItem();

    public GameObject GetGameObject() => gameObject;
}