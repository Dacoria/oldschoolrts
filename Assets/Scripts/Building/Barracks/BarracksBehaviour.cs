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
    public List<BarracksUnitSetting> BarracksUnitSettings;

    private ConsumeRefillItemsBehaviour ConsumeRefillItemsBehaviour;
    public void Awake()
    {
        gameObject.AddComponent<RefillBehaviour>();
        ConsumeRefillItemsBehaviour = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
    }    

    public void AddItem(Enum type)
    {
        var unit = (BarracksUnitSetting)GetCardDisplaySetting(type);
        var itemsNeeded = GetCardDisplaySetting(type).ItemsConsumedToProduce.ConvertAll(x => (ItemAmount)x).ToList();

        if(ConsumeRefillItemsBehaviour.TryConsumeRefillItems(itemsNeeded))
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
            armyUnitBehaviour.Offence = unitSettings.UnitStats.Offence;
            armyUnitBehaviour.Defence = unitSettings.UnitStats.Defence;
            armyUnitBehaviour.EnemyAttractRadius = unitSettings.UnitStats.RangeToAttractEnemies;
            armyUnitBehaviour.Reach = unitSettings.UnitStats.RangeToAttack;
            armyUnitBehaviour.NavMeshAgent.stoppingDistance = unitSettings.UnitStats.RangeToAttack;
            armyUnitBehaviour.NavMeshAgent.speed = unitSettings.UnitStats.Speed;

            var healthBehaviour = unitGo.GetComponent<HealthBehaviour>();
            if (healthBehaviour != null)
            {
                healthBehaviour.InitialHeath = unitSettings.UnitStats.Health;
                healthBehaviour.CurrentHealth = unitSettings.UnitStats.Health;
            }
        }
        else
        {
            throw new Exception("unit " + unitSettings.Type + " vereist ArmyUnitBehaviour");
        }
    }

    public void DecreaseItem(Enum type) {}

    public int GetCount(Enum type) => 0;

    public ProductionSetting GetCardDisplaySetting(Enum type) => BarracksUnitSettings.First(x => x.Type == (BarracksUnitType)type);

    public bool CanProces(Enum type) 
    {
        var itemsNeeded = GetCardDisplaySetting(type).ItemsConsumedToProduce;
        return ConsumeRefillItemsBehaviour.CanConsumeRefillItems(itemsNeeded);
    } 
  
    public QueueForBuildingBehaviour GetQueueForBuildingBehaviour() => GetComponent<QueueForBuildingBehaviour>();

    public List<ItemAmount> GetItemsToRefill()
    {
        var result = new List<ItemAmount>();
        foreach(var setting in BarracksUnitSettings)
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
        BarracksUnitSettings.ConvertAll(x => (ProductionSetting)x).ConvertToSingleProduceItem();

    public GameObject GetGameObject() => gameObject;
}