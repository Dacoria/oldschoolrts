using System.Reflection;
using UnityEngine;

public abstract class BaseAEMonoCI : MonoBehaviourCI
{
    protected void OnEnable()
    {
        if (IsOverwritten("OnFreeBuilder")) AE.FreeBuilder += OnFreeBuilder;
        if (IsOverwritten("OnFreeSerf")) AE.FreeSerf += OnFreeSerf;
        if (IsOverwritten("OnOrderStatusChanged")) AE.OrderStatusChanged += OnOrderStatusChanged;
        if (IsOverwritten("OnBuilderRequestStatusChanged")) AE.BuilderRequestStatusChanged += OnBuilderRequestStatusChanged;
        if (IsOverwritten("OnBuilderRequest")) AE.BuilderRequest += OnBuilderRequest;
        if (IsOverwritten("OnSerfRequest")) AE.SerfRequest += OnSerfRequest;
        if (IsOverwritten("OnKeyCodeAction")) AE.KeyCodeAction += OnKeyCodeAction;
        if (IsOverwritten("OnLeftClickOnGo")) AE.LeftClickOnGo += OnLeftClickOnGo;
        if (IsOverwritten("OnFoodStatusHasChanged")) AE.FoodStatusHasChanged += OnFoodStatusHasChanged;
        if (IsOverwritten("OnReachedFoodRefillingPoint")) AE.ReachedFoodRefillingPoint += OnReachedFoodRefillingPoint;
        if (IsOverwritten("OnNoFoodToConsume")) AE.NoFoodToConsume += OnNoFoodToConsume;
        if (IsOverwritten("OnNoWorkerAction")) AE.NoWorkerAction += OnNoWorkerAction;
        if (IsOverwritten("OnStartNewWorkerAction")) AE.StartNewWorkerAction += OnStartNewWorkerAction;       
        if (IsOverwritten("OnVillagerUnitCreated")) AE.VillagerUnitCreated += OnVillagerUnitCreated;
    }

    protected void OnDisable()
    {
        if (IsOverwritten("OnFreeBuilder")) AE.FreeBuilder -= OnFreeBuilder;
        if (IsOverwritten("OnFreeSerf")) AE.FreeSerf -= OnFreeSerf;
        if (IsOverwritten("OnOrderStatusChanged")) AE.OrderStatusChanged -= OnOrderStatusChanged;
        if (IsOverwritten("OnBuilderRequestStatusChanged")) AE.BuilderRequestStatusChanged -= OnBuilderRequestStatusChanged;
        if (IsOverwritten("OnBuilderRequest")) AE.BuilderRequest -= OnBuilderRequest;
        if (IsOverwritten("OnSerfRequest")) AE.SerfRequest -= OnSerfRequest;
        if (IsOverwritten("OnKeyCodeAction")) AE.KeyCodeAction -= OnKeyCodeAction;
        if (IsOverwritten("OnLeftClickOnGo")) AE.LeftClickOnGo -= OnLeftClickOnGo;
        if (IsOverwritten("OnFoodStatusHasChanged")) AE.FoodStatusHasChanged -= OnFoodStatusHasChanged;
        if (IsOverwritten("OnReachedFoodRefillingPoint")) AE.ReachedFoodRefillingPoint -= OnReachedFoodRefillingPoint;
        if (IsOverwritten("OnNoFoodToConsume")) AE.NoFoodToConsume -= OnNoFoodToConsume;
        if (IsOverwritten("OnNoWorkerAction")) AE.NoWorkerAction -= OnNoWorkerAction;
        if (IsOverwritten("OnStartNewWorkerAction")) AE.StartNewWorkerAction -= OnStartNewWorkerAction;
        if (IsOverwritten("OnVillagerUnitCreated")) AE.VillagerUnitCreated -= OnVillagerUnitCreated;
    }

    protected virtual void OnFreeBuilder(BuilderBehaviour behaviour) { }
    protected virtual void OnFreeSerf(SerfBehaviour behaviour) { }
    protected virtual void OnOrderStatusChanged(SerfOrder order) { }
    protected virtual void OnBuilderRequestStatusChanged(BuilderRequest request, BuildStatus status) { }
    protected virtual void OnBuilderRequest(BuilderRequest request) { }
    protected virtual void OnSerfRequest(SerfRequest request) { }
    protected virtual void OnKeyCodeAction(KeyCodeAction action) { }
    protected virtual void OnLeftClickOnGo(GameObject go) { }
    protected virtual void OnFoodStatusHasChanged(FoodConsumption consumption, FoodConsumptionStatus status) { }
    protected virtual void OnReachedFoodRefillingPoint(TavernBehaviour behaviour1, FoodConsumptionBehaviour behaviour2) { }
    protected virtual void OnNoFoodToConsume(FoodConsumption consumption) { }
    protected virtual void OnNoWorkerAction(WorkManager manager) { }
    protected virtual void OnStartNewWorkerAction(WorkManager manager) { }
    protected virtual void OnVillagerUnitCreated(VillagerUnitType villagerUnitType) { }


    // GEEN ABSTRACTE CLASSES!
    private bool IsOverwritten(string functionName)
    {
        var type = GetType();
        var method = type.GetMember(functionName, BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);
        return method.Length > 0;
    }
}