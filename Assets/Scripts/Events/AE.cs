using System;
using UnityEngine;

public static class AE
{
    // FREE SERFS/BUILDERS
    public static Action<BuilderBehaviour> FreeBuilder;
    public static Action<SerfBehaviour> FreeSerf;

    // ORDER CHANGES
    public static Action<SerfOrder> OrderStatusChanged;
    public static Action<BuilderRequest, BuildStatus> BuilderRequestStatusChanged;

    // ORDER REQUESTS
    public static Action<BuilderRequest> BuilderRequest;
    public static Action<SerfRequest> SerfRequest;

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

    // VILLAGER CREATED
    public static Action<VillagerUnitType> VillagerUnitCreated;
}