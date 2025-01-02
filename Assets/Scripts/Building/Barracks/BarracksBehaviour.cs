using Assets;
using Assets.CrossCutting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BarracksBehaviour : MonoBehaviourCI, ICardBuilding
{
    [ComponentInject] private BuildingBehaviour buildingBehaviour;
    public List<SerfRequest> IncomingOrders;
    public List<ItemAmount> StockpileOfItemsRequired;

    private ConsumeRefillItemsBehaviour consumeRefillItemsBehaviour;
    [HideInInspector] public RefillBehaviour RefillBehaviour;
    public QueueForBuildingBehaviour queueForBuildingBehaviour;
    private ProduceCRBehaviour produceCRBehaviour;

    public new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour), typeof(QueueForBuildingBehaviour), typeof(ProduceCRBehaviour) });

        RefillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItemsBehaviour = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
        queueForBuildingBehaviour = gameObject.AddComponent<QueueForBuildingBehaviour>();
        produceCRBehaviour = gameObject.AddComponent<ProduceCRBehaviour>();
    }

    public void AddType(Enum type)
    {
        var barracksUnitType = (BarracksUnitType)type;
        var itemsForProduction = buildingBehaviour.BuildingType.GetProductionSettings().Single(x => x.GetType() == type).ItemsConsumedToProduce;
        if (CanProces(barracksUnitType))
        {
            produceCRBehaviour.ProduceOverTime(new ProduceSetup(barracksUnitType,
                produceAction: () => CreateUnit(barracksUnitType)));
        }
        else
        {
            throw new Exception("Zou al gecheckt moeten zijn"); //queue
        }
    }

    private void CreateUnit(BarracksUnitType type)
    {
        var unit = BarrackUnitPrefabs.Get().Single(x => x.Type == type);
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

    public bool CanProces(Enum type) 
    {
        if (!produceCRBehaviour.IsReadyForNextProduction)
        {
            return false;
        }

        var itemsNeeded = buildingBehaviour.BuildingType.GetProductionSettings().First(x => x.GetType() == type).ItemsConsumedToProduce;
        if (!consumeRefillItemsBehaviour.CanConsumeRefillItems(itemsNeeded))
            return false;        

        return true;
    }

    public GameObject GetGameObject() => gameObject;
    public TypeProcessing GetCurrentTypeProcessed() => produceCRBehaviour.CurrentTypesProcessed.First();
    public BuildingType GetBuildingType() => buildingBehaviour.BuildingType;
}