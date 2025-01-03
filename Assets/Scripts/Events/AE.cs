using System;
using System.Collections.Generic;
using UnityEngine;

public static class AE
{
    // FREE SERFS/BUILDERS
    public static Action<BuilderBehaviour> FreeBuilder;
    public static Action<SerfBehaviour> FreeSerf;

    // ORDER CHANGES
    public static Action<SerfOrder, Status> OrderStatusChanged;
    public static Action<BuilderRequest, BuildStatus> BuilderRequestStatusChanged;

    // ORDER REQUESTS
    public static Action<BuilderRequest> BuilderRequest;
    public static Action<SerfRequest> SerfRequest;

    // SERF ORDER CHANGE IN PROGRESS
    public static Action<SerfOrder> StartCompletingSerfRequest;

    // KEYACTION
    public static Action<KeyCodeAction> KeyCodeAction;

    // MOUSE ACTION
    public static Action<GameObject> LeftClickOnGo;

    // FOOD
    public static Action<FoodConsumption, FoodConsumptionStatus> FoodStatusHasChanged;
    public static Action<TavernBehaviour, FoodConsumptionBehaviour> ReachedFoodRefillingPoint;
    public static Action<FoodConsumption> NoFoodToConsume;

    // WORKMANAGER ACTION
    public static Action<WorkManager> NoWorkerAction;
    public static Action<WorkManager> StartNewWorkerAction;

    // VILLAGERS
    public static Action<IVillagerUnit> NewVillagerUnit;

    // BATTLE UNITS
    public static Action<ArmyUnitBehaviour> NewBattleUnit;

    // BUILDINGS
    public static Action<BuildingBehaviour> NewBuilding;
    public static Action<WorkerBuildingBehaviour> BuildingNeedsWorker;

    // PRODUCE EVENTS
    public static Action<BuildingBehaviour, List<Enum>> StartedProducingAction; // evt aanhaken op deze events
    public static Action<BuildingBehaviour, List<Enum>> FinishedProducingAction; // evt aanhaken op deze events
    public static Action<BuildingBehaviour> FinishedWaitingAfterProducingAction; // evt aanhaken op deze events    
}