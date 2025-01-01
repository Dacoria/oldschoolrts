using Assets;
using Assets.CrossCutting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BarracksBehaviour : MonoBehaviourCI, ICardBuilding, IRefillItems
{
    [ComponentInject] private BuildingBehaviour buildingBehaviour;
    public List<SerfRequest> IncomingOrders;
    public List<ItemAmount> StockpileOfItemsRequired;

    private ConsumeRefillItemsBehaviour consumeRefillItemsBehaviour;
    [HideInInspector] public RefillBehaviour RefillBehaviour;
    public QueueForBuildingBehaviour queueForBuildingBehaviour;

    public new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour), typeof(QueueForBuildingBehaviour) });

        RefillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItemsBehaviour = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
        queueForBuildingBehaviour = gameObject.AddComponent<QueueForBuildingBehaviour>();
    }

    public void AddType(Enum type)
    {
        var unit = (BarracksUnitSetting)GetCardDisplaySetting(type);
        var itemsForProduction = GetCardDisplaySetting(type).ItemsConsumedToProduce.ConvertAll(x => (ItemAmount)x).ToList();

        if (CanProces((BarracksUnitType)type) && consumeRefillItemsBehaviour.TryConsumeRefillItems(itemsForProduction))
        {
            StartCoroutine(StartUnitCreationProcess((BarracksUnitType)type));
        }
    }

    private UIItemProcessing currentItemProcessed;
    public UIItemProcessing GetCurrentItemProcessed() => currentItemProcessed;

    private IEnumerator StartUnitCreationProcess(BarracksUnitType type)
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

    public void CreateUnit(Enum type)
    {
        var unit = (BarracksUnitSetting)GetCardDisplaySetting(type);
        var itemsNeeded = GetCardDisplaySetting(type).ItemsConsumedToProduce.ConvertAll(x => (ItemAmount)x).ToList();

        var unitGo = Instantiate(unit.ResourcePrefab);
        unitGo.GetComponent<OwnedByPlayerBehaviour>().Player = Player.PLAYER1;
        unitGo.transform.position = this.transform.gameObject.EntranceExit();
        SetUnitStats(unitGo, unit);
    }

    private void SetUnitStats(GameObject unitGo, BarracksUnitSetting unitSettings)
    {
        var armyUnitBehaviour = unitGo.GetComponent<ArmyUnitBehaviour>();
        if (armyUnitBehaviour != null)
        {
            armyUnitBehaviour.Offence = unitSettings.UnitStats.Offence.DeepClone();
            armyUnitBehaviour.Defence = unitSettings.UnitStats.Defence.DeepClone();
            armyUnitBehaviour.EnemyAttractRadius = unitSettings.UnitStats.RangeToAttractEnemies;
            armyUnitBehaviour.Reach = unitSettings.UnitStats.RangeToAttack;
            armyUnitBehaviour.NavMeshAgent.stoppingDistance = unitSettings.UnitStats.RangeToAttack;
            armyUnitBehaviour.NavMeshAgent.speed = unitSettings.UnitStats.Speed;

            var healthBehaviour = unitGo.GetComponent<HealthBehaviour>();
            if (healthBehaviour != null)
            {
                healthBehaviour.InitialHeath = unitSettings.UnitStats.Health.DeepClone();
                healthBehaviour.CurrentHealth = unitSettings.UnitStats.Health.DeepClone();
            }
        }
        else
        {
            throw new Exception($"Unit '{unitSettings.Type} -> {unitGo.name}' vereist ArmyUnitBehaviour");
        }
    }

    public void DecreaseType(Enum type) {}

    public int GetCount(Enum type) => 0;

    public ProductionSetting GetCardDisplaySetting(Enum type) => BarrackUnitPrefabs.Get().Single(x => x.Type == (BarracksUnitType)type);

    public bool CanProces(Enum type) 
    {
        var itemsNeeded = GetCardDisplaySetting(type).ItemsConsumedToProduce;
        return currentItemProcessed == null && consumeRefillItemsBehaviour.CanConsumeRefillItems(itemsNeeded);
    }

    public bool AlwaysRefillItemsIgnoreBuffer() => true;

    public List<ItemProduceSetting> GetItemProduceSettings() =>
        BarrackUnitPrefabs.Get().ConvertAll(x => (ProductionSetting)x).ConvertToSingleProduceItem();

    public GameObject GetGameObject() => gameObject;

    public float GetProductionTime(Enum type) => buildingBehaviour.BuildingType.GetProductionDurationSettings().TimeToProduceResourceInSeconds; // 1 waarde voor alles
}