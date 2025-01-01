using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class BaseAEMonoCI : MonoBehaviourCI
{
    protected void OnEnable()
    {
        if (IsOverwritten("OnBuilderRequestStatusChanged")) AE.BuilderRequestStatusChanged += OnBuilderRequestStatusChanged;
        if (IsOverwritten("OnBuilderRequest")) AE.BuilderRequest += OnBuilderRequest;
        if (IsOverwritten("OnBuildingNeedsWorker")) AE.BuildingNeedsWorker += OnBuildingNeedsWorker;
        if (IsOverwritten("OnFinishedProducingAction")) AE.FinishedProducingAction += OnFinishedProducingAction;
        if (IsOverwritten("OnFinishedWaitingAfterProducingAction")) AE.FinishedWaitingAfterProducingAction += OnFinishedWaitingAfterProducingAction;
        if (IsOverwritten("OnFoodStatusHasChanged")) AE.FoodStatusHasChanged += OnFoodStatusHasChanged;
        if (IsOverwritten("OnFreeBuilder")) AE.FreeBuilder += OnFreeBuilder;
        if (IsOverwritten("OnFreeSerf")) AE.FreeSerf += OnFreeSerf;
        if (IsOverwritten("OnKeyCodeAction")) AE.KeyCodeAction += OnKeyCodeAction;
        if (IsOverwritten("OnLeftClickOnGo")) AE.LeftClickOnGo += OnLeftClickOnGo;
        if (IsOverwritten("OnNewVillagerUnit")) AE.NewVillagerUnit += OnNewVillagerUnit;
        if (IsOverwritten("OnNoFoodToConsume")) AE.NoFoodToConsume += OnNoFoodToConsume;
        if (IsOverwritten("OnNoWorkerAction")) AE.NoWorkerAction += OnNoWorkerAction;
        if (IsOverwritten("OnOrderStatusChanged")) AE.OrderStatusChanged += OnOrderStatusChanged;
        if (IsOverwritten("OnReachedFoodRefillingPoint")) AE.ReachedFoodRefillingPoint += OnReachedFoodRefillingPoint;
        if (IsOverwritten("OnSerfRequest")) AE.SerfRequest += OnSerfRequest;
        if (IsOverwritten("OnStartCompletingSerfRequest")) AE.StartCompletingSerfRequest += OnStartCompletingSerfRequest;
        if (IsOverwritten("OnStartedProducingAction")) AE.StartedProducingAction += OnStartedProducingAction;
        if (IsOverwritten("OnStartNewWorkerAction")) AE.StartNewWorkerAction += OnStartNewWorkerAction;
    }

    protected void OnDisable()
    {
        if (IsOverwritten("OnBuilderRequestStatusChanged")) AE.BuilderRequestStatusChanged -= OnBuilderRequestStatusChanged;
        if (IsOverwritten("OnBuilderRequest")) AE.BuilderRequest -= OnBuilderRequest;
        if (IsOverwritten("OnBuildingNeedsWorker")) AE.BuildingNeedsWorker -= OnBuildingNeedsWorker;
        if (IsOverwritten("OnFinishedProducingAction")) AE.FinishedProducingAction += OnFinishedProducingAction;
        if (IsOverwritten("OnFinishedWaitingAfterProducingAction")) AE.FinishedWaitingAfterProducingAction += OnFinishedWaitingAfterProducingAction;
        if (IsOverwritten("OnFoodStatusHasChanged")) AE.FoodStatusHasChanged -= OnFoodStatusHasChanged;
        if (IsOverwritten("OnFreeBuilder")) AE.FreeBuilder -= OnFreeBuilder;
        if (IsOverwritten("OnFreeSerf")) AE.FreeSerf -= OnFreeSerf;
        if (IsOverwritten("OnKeyCodeAction")) AE.KeyCodeAction -= OnKeyCodeAction;
        if (IsOverwritten("OnLeftClickOnGo")) AE.LeftClickOnGo -= OnLeftClickOnGo;
        if (IsOverwritten("OnNewVillagerUnit")) AE.NewVillagerUnit -= OnNewVillagerUnit;
        if (IsOverwritten("OnNoFoodToConsume")) AE.NoFoodToConsume -= OnNoFoodToConsume;
        if (IsOverwritten("OnNoWorkerAction")) AE.NoWorkerAction -= OnNoWorkerAction;
        if (IsOverwritten("OnOrderStatusChanged")) AE.OrderStatusChanged -= OnOrderStatusChanged;
        if (IsOverwritten("OnReachedFoodRefillingPoint")) AE.ReachedFoodRefillingPoint -= OnReachedFoodRefillingPoint;
        if (IsOverwritten("OnSerfRequest")) AE.SerfRequest -= OnSerfRequest;
        if (IsOverwritten("OnStartCompletingSerfRequest")) AE.StartCompletingSerfRequest -= OnStartCompletingSerfRequest;
        if (IsOverwritten("OnStartedProducingAction")) AE.StartedProducingAction -= OnStartedProducingAction;
        if (IsOverwritten("OnStartNewWorkerAction")) AE.StartNewWorkerAction -= OnStartNewWorkerAction;
    }

    protected virtual void OnBuilderRequestStatusChanged(BuilderRequest request, BuildStatus status) { }
    protected virtual void OnBuilderRequest(BuilderRequest request) { }
    protected virtual void OnBuildingNeedsWorker(WorkerBuildingBehaviour behaviour) { }
    protected virtual void OnFinishedProducingAction(BuildingBehaviour building, List<ItemOutput> items) { }
    protected virtual void OnFinishedWaitingAfterProducingAction(BuildingBehaviour building) { }
    protected virtual void OnFoodStatusHasChanged(FoodConsumption consumption, FoodConsumptionStatus status) { }
    protected virtual void OnFreeBuilder(BuilderBehaviour behaviour) { }
    protected virtual void OnFreeSerf(SerfBehaviour behaviour) { }
    protected virtual void OnKeyCodeAction(KeyCodeAction action) { }
    protected virtual void OnLeftClickOnGo(GameObject go) { }
    protected virtual void OnNewVillagerUnit(IVillagerUnit newVillagerUnit) { }
    protected virtual void OnNoFoodToConsume(FoodConsumption consumption) { }
    protected virtual void OnNoWorkerAction(WorkManager manager) { }
    protected virtual void OnReachedFoodRefillingPoint(TavernBehaviour behaviour1, FoodConsumptionBehaviour behaviour) { }
    protected virtual void OnOrderStatusChanged(SerfOrder order, Status prevStatus) { }
    protected virtual void OnSerfRequest(SerfRequest request) { }
    protected virtual void OnStartCompletingSerfRequest(SerfOrder order) { }
    protected virtual void OnStartedProducingAction(BuildingBehaviour building, List<ItemOutput> items) { }
    protected virtual void OnStartNewWorkerAction(WorkManager manager) { }

    // GEEN ABSTRACTE CLASSES!
    private bool IsOverwritten(string functionName)
    {
        var type = GetType();
        var method = type.GetMember(functionName, BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);
        return method.Length > 0;
    }
}