using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BarracksBehaviour : MonoBehaviourCI, ICardSelectProdBuilding, IProduce
{
    [ComponentInject] private BuildingBehaviour buildingBehaviour;

    private ConsumeRefillItemsBehaviour consumeRefillItemsBehaviour;
    private RefillBehaviour refillBehaviour;
    private QueueForBuildingBehaviour queueForBuildingBehaviour;
    private ProduceCRBehaviour produceCRBehaviour;

    public new void Awake()
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
        var barracksUnitType = (BarracksUnitType)type;
        if (BattleManager.ToggleInstaFreeUnits_Active)
        {
            produceCRBehaviour.ProduceInstant(new ProduceSetup(barracksUnitType, this));
        }
        else
        {
            var itemsForProduction = buildingBehaviour.BuildingType.GetProductionSettings().Single(x => (BarracksUnitType)x.GetType() == (BarracksUnitType)type).ItemsConsumedToProduce;
            if (CanProces(barracksUnitType))
            {
                consumeRefillItemsBehaviour.TryConsumeRefillItems(itemsForProduction);
                produceCRBehaviour.ProduceOverTime(new ProduceSetup(barracksUnitType, this));
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
            var unit = BarrackUnitPrefabs.Get().Single(x => x.Type == (BarracksUnitType)type);
            var unitGo = Instantiate(unit.ResourcePrefab);
            unitGo.GetComponent<OwnedByPlayerBehaviour>().Player = Player.PLAYER1;
            unitGo.transform.position = this.transform.gameObject.EntranceExit();
        }
    }   

    public bool CanProces(Enum type) 
    {
        if (!produceCRBehaviour.IsReadyForNextProduction())
        {
            return false;
        }

        var itemsNeeded = buildingBehaviour.BuildingType.GetProductionSettings().First(x => (BarracksUnitType)x.GetType() == (BarracksUnitType)type).ItemsConsumedToProduce;
        if (!consumeRefillItemsBehaviour.CanConsumeRefillItems(itemsNeeded))
            return false;        

        return true;
    }

    public GameObject GetGameObject() => gameObject;
    public TypeProcessing GetCurrentTypeProcessed() => produceCRBehaviour?.CurrentTypesProcessed?.FirstOrDefault();
    public BuildingType GetBuildingType() => buildingBehaviour.BuildingType;    
}