using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class WorkManager : BaseAEMonoCI, IHasStopped, IVillagerUnit
{
    public List<BuildingType> BuildingTypeToBringResourceBackTo;
    public float timeToWaitForRetryIfNoNewAction = 1;

    [HideInInspector] // wordt geset bij initieren van unit in school
    public VillagerUnitType VillagerUnitType;
    public VillagerUnitType GetVillagerUnitType() => VillagerUnitType;
    public GameObject GetGO() => this.gameObject;
    public bool IsVillagerWorker() => true;

    [ComponentInject] private Animator Animator;
    [ComponentInject] private NavMeshAgent NavMeshAgent;
    [ComponentInject] private GoingToWorkStation GoingToWorkStation;
    private GameObject ObjectToBringResourceBackTo;
    private FoodConsumptionBehaviour FoodConsumptionBehaviour;

    private bool stopAsapWithOrders;
    private bool stoppedWithOrders;

    private bool isIdle;
    private bool workerIsActive;

    protected override void Awake()
    {
        // Geen CI -> components worden door onderstaand event pas toegevoegd        
        NewVillagerComponentsManager.NewVillagerUnit(this); // event is te traag
    }

    public void Start()
    {        
        this.ComponentInject(); // nu CI; components zijn toegevoegd
        AE.NewVillagerUnit?.Invoke(this);
        NavMeshAgent.isStopped = true;
        StartCoroutine(FindRelevantBuildingAndContinueAfterwards());
    }

    private IEnumerator FindRelevantBuildingAndContinueAfterwards()
    {
        while (ObjectToBringResourceBackTo == null)
        {
            ObjectToBringResourceBackTo = TryFindFreeObjectToBringResourceBackTo();
            if (ObjectToBringResourceBackTo?.activeSelf == true)
            {
                ObjectToBringResourceBackTo.GetComponent<WorkerBuildingBehaviour>().Worker = this.gameObject;
            }
            AE.NoWorkerAction?.Invoke(this);
            yield return Wait4Seconds.Get(1f);
        }

        SetRelevantScripts();
        workerIsActive = true;
    }

    private void SetRelevantScripts()
    {
        //TODO jelle list injecten
        var workActions = this.GetComponents<IVillagerWorkAction>();
        foreach(var workActionScript in workActions)
        {
            workActionScript.SetReturnTargetForAction(ObjectToBringResourceBackTo);
        }        
    }

    public bool HasStoppedWithLogic()
    {
        return stoppedWithOrders;
    }

    protected override void OnFoodStatusHasChanged(FoodConsumption foodConsumption, FoodConsumptionStatus previousStatus)
    {
        if (foodConsumption == FoodConsumptionBehaviour.FoodConsumption)
        {
            switch (foodConsumption.FoodConsumptionStatus)
            {
                case FoodConsumptionStatus.NEEDS_REFILL:
                    stopAsapWithOrders = true;
                    break;
                case FoodConsumptionStatus.REFILL_SUCCESS:
                case FoodConsumptionStatus.REFILL_FAILED:
                    stopAsapWithOrders = false;
                    stoppedWithOrders = false;
                    GoingToWorkStation.actionIsAvailable = true;// prio 1 -> deze wordt als 1e weer gepakt                    
                    break;
            }
        }
    }

    private GameObject TryFindFreeObjectToBringResourceBackTo()
    {
        var gameObjects = GameObject.FindGameObjectsWithTag(Constants.TAG_BUILDING);
        var firstOrNullUninhabitedBuilding = gameObjects.FirstOrDefault(x =>
            x.activeSelf &&
            BuildingTypeToBringResourceBackTo.Any(y => y == x.GetComponent<BuildingBehaviour>()?.BuildingType) &&
            x.GetComponentInChildren<WorkerBuildingBehaviour>() != null &&
            x.GetComponentInChildren<WorkerBuildingBehaviour>().Worker == null &&
            x.GetComponent<BuildingBehaviour>()?.CurrentBuildStatus == BuildStatus.COMPLETED_BUILDING
            );

        return firstOrNullUninhabitedBuilding?.GetComponentInChildren<WorkerBuildingBehaviour>()?.gameObject;
    }

    void Update()
    {   
        if(workerIsActive && !isIdle)
        {
            var activeWorkerAction = GetActiveWorkAction();
            if(activeWorkerAction == null)
            {
                var success = DetermineNextWorkerAction();
                if (success)
                {
                    AE.StartNewWorkerAction?.Invoke(this);                    
                }
                else
                {
                    StartCoroutine(IdleForRetryNewAction());
                }
            }
        }
        UpdateAnimation();
    }

    private bool DetermineNextWorkerAction()
    {
        if(GetActiveWorkAction() == null)
        {
            if(stopAsapWithOrders || stoppedWithOrders)
            {
                stoppedWithOrders = true;
                return false;
            }

            var workActions = this.GetComponents<IVillagerWorkAction>();
            var availableActions = workActions.Where(x => x.CanDoAction()).ToList();
            var highestPrioWorkActions = availableActions
                .Where(y => y.GetPrio() == availableActions.Min(z => z.GetPrio()))
                .ToList();

            if (highestPrioWorkActions.Count() == 1)
            {                
                highestPrioWorkActions.First().Init();
                return true;
            }
            else if(highestPrioWorkActions.Count() > 1)
            {            
                var randomValue = Random.Range(1, highestPrioWorkActions.Count() * 50);
                var index = (int)randomValue / 50;
                highestPrioWorkActions.ToList()[index].Init();
                return true;
            }
        }

        return false; // geen workacties geactiveerd
    }

    private void UpdateAnimation()
    {
        var activeWorkAction = GetActiveWorkAction();
        if (!workerIsActive || activeWorkAction == null)
        {
            Animator.SetBool(Constants.ANIM_BOOL_IS_WALKING, !NavMeshAgent.isStopped);
            Animator.SetBool(Constants.ANIM_BOOL_IS_WORKING, false);
            Animator.SetBool(Constants.ANIM_BOOL_IS_WORKING_2, false);
            Animator.SetBool(Constants.ANIM_BOOL_IS_IDLE, NavMeshAgent.isStopped);
        }
        else
        {
            var animationStatus = activeWorkAction.GetAnimationStatus();
            Animator.SetBool(Constants.ANIM_BOOL_IS_WALKING, animationStatus.IsWalking);
            Animator.SetBool(Constants.ANIM_BOOL_IS_WORKING, animationStatus.IsWorking);
            Animator.SetBool(Constants.ANIM_BOOL_IS_WORKING_2, animationStatus.IsWorking2);
            Animator.SetBool(Constants.ANIM_BOOL_IS_IDLE, animationStatus.IsIdle);
        }       
    }

    public IVillagerWorkAction LatestVillagerWorkAction;
    private IVillagerWorkAction GetActiveWorkAction()
    {
        if(LatestVillagerWorkAction?.IsActive() == true)
        {
            // soort cache, zodat je niet elke update get components hoeft te doen
            return LatestVillagerWorkAction;
        }

        LatestVillagerWorkAction = null;

        var workActions = this.GetComponents<IVillagerWorkAction>();
        LatestVillagerWorkAction = workActions.FirstOrDefault(x => x.IsActive());
        return LatestVillagerWorkAction;
    }

    private IEnumerator IdleForRetryNewAction()
    {
        isIdle = true;
        NavMeshAgent.isStopped = true; // idle = stilstaan animatie + navmesh stoppen

        yield return Wait4Seconds.Get(timeToWaitForRetryIfNoNewAction);
        var success = DetermineNextWorkerAction();
        if(success || stoppedWithOrders)
        {
            isIdle = false;
        }
        else
        {
            StartCoroutine(IdleForRetryNewAction());
            AE.NoWorkerAction?.Invoke(this);
        }
    }
}