using Assets;
using Assets.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BarracksBehaviour : MonoBehaviour, ICardBuilding, IRefillItems
{
    public List<SerfRequest> IncomingOrders;
    public List<ItemAmount> StockpileOfItemsRequired;

    private ConsumeRefillItemsBehaviour consumeRefillItemsBehaviour;
    [HideInInspector] public RefillBehaviour RefillBehaviour;

    public void Awake()
    {
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour)});

        RefillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItemsBehaviour = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
    }    

    public void AddItem(Enum type)
    {
        var unit = (BarracksUnitSetting)GetCardDisplaySetting(type);
        var itemsNeeded = GetCardDisplaySetting(type).ItemsConsumedToProduce.ConvertAll(x => (ItemAmount)x).ToList();

        if(consumeRefillItemsBehaviour.TryConsumeRefillItems(itemsNeeded))
        {
            var unitGo = Instantiate(unit.ResourcePrefab);
            unitGo.GetComponent<OwnedByPlayerBehaviour>().Player = Player.PLAYER1;
            unitGo.transform.position = this.transform.gameObject.EntranceExit();
            SetUnitStats(unitGo, unit);
        }
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

    public void DecreaseItem(Enum type) {}

    public int GetCount(Enum type) => 0;

    public ProductionSetting GetCardDisplaySetting(Enum type) => BarrackUnitPrefabs.Get().First(x => x.Type == (BarracksUnitType)type);

    public bool CanProces(Enum type) 
    {
        var itemsNeeded = GetCardDisplaySetting(type).ItemsConsumedToProduce;
        return consumeRefillItemsBehaviour.CanConsumeRefillItems(itemsNeeded);
    } 
  
    public QueueForBuildingBehaviour GetQueueForBuildingBehaviour() => GetComponent<QueueForBuildingBehaviour>();

    public List<ItemAmount> GetItemsToRefill()
    {
        var result = new List<ItemAmount>();
        foreach(var setting in BarrackUnitPrefabs.Get())
        {
            foreach (var itemConsumed in setting.ItemsConsumedToProduce)
            {
                result.Add(itemConsumed);
            }
        }
        return result;
    }

    public bool AlwaysRefillItemsIgnoreBuffer() => true;

    public List<ItemProduceSetting> GetItemProduceSettings() =>
        BarrackUnitPrefabs.Get().ConvertAll(x => (ProductionSetting)x).ConvertToSingleProduceItem();

    public GameObject GetGameObject() => gameObject;

    public float GetProductionTime(Enum type) => 0; // instant

    public UIItemProcessing GetCurrentItemProcessed() => null;
}