using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class WorkManager : BaseAEMonoCI, IHasStopped, IVillagerUnit
{
    public List<BuildingType> BuildingTypeToBringResourceBackTo;
    public float TimeToWaitForRetryIfNoNewAction = 1;

    // wordt geset bij initieren van unit in school
    public VillagerUnitType VillagerUnitType;
    public VillagerUnitType GetVillagerUnitType() => VillagerUnitType;
    public GameObject GetGO() => this.gameObject;
    public bool IsVillagerWorker() => true;

    [ComponentInject] private Animator animator;
    [ComponentInject] private NavMeshAgent navMeshAgent;
    [ComponentInject] private GoingToWorkStation goingToWorkStation;
    private List<IVillagerWorkAction> villagerWorkActions;

    private GameObject objectToBringResourceBackTo;
    private FoodConsumptionBehaviour foodConsumptionBehaviour;

    private bool stopAsapWithOrders;
    private bool stoppedWithOrders;

    private bool isIdle;
    private bool workerIsActive;

    protected override void Awake()
    {
        if (VillagerUnitType == VillagerUnitType.Serf || VillagerUnitType == VillagerUnitType.Builder)
            throw new System.Exception($"Worker found unsupported type: {VillagerUnitType}");

        // Geen CI -> components via start toegevoegd (event is te traag -> via awake unit)
        NewVillagerComponentsManager.NewVillagerUnit(this);
    }

    public void Start()
    {        
        this.ComponentInject(); // nu CI; components zijn toegevoegd
        villagerWorkActions = GetComponents<IVillagerWorkAction>().ToList(); // via CI krijg je soms dubbele :o

        AE.NewVillagerUnit?.Invoke(this);
        navMeshAgent.isStopped = true;
        StartCoroutine(FindRelevantBuildingAndContinueAfterwards());
    }

    private IEnumerator FindRelevantBuildingAndContinueAfterwards()
    {
        while (true)
        {
            var buildingMatchForVillager = BuildingForVillagerManager.Instance.TryCoupleBuildingWithVillager(this, BuildingTypeToBringResourceBackTo);
            if (buildingMatchForVillager != null)
            {
                objectToBringResourceBackTo = buildingMatchForVillager.gameObject;
                villagerWorkActions.ForEach(x => x.SetReturnTargetForAction(objectToBringResourceBackTo));
                break;
            }

            // probeer elke seconde
            AE.NoWorkerAction?.Invoke(this);
            yield return Wait4Seconds.Get(1f);
        }

        workerIsActive = true;
    }

    public bool HasStoppedWithLogic() => stoppedWithOrders;

    protected override void OnFoodStatusHasChanged(FoodConsumption foodConsumption, FoodConsumptionStatus previousStatus)
    {
        if (foodConsumption == foodConsumptionBehaviour.FoodConsumption)
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
                    goingToWorkStation.ActionIsAvailable = true;// prio 1 -> deze wordt als 1e weer gepakt                    
                    break;
            }
        }
    }

    void Update()
    {   
        if(workerIsActive && !stoppedWithOrders && !isIdle)
        {
            var activeWorkerAction = GetActiveWorkAction();
            if (activeWorkerAction == null)
            {
                DoNextAction();
            }
        }
        UpdateAnimation();
    }

    private void DoNextAction()
    {
        if (stopAsapWithOrders || stoppedWithOrders)
        {
            stoppedWithOrders = true;
            return;
        }

        var nextWorkerAction = DetermineNextWorkerAction();
        if (nextWorkerAction != null)
        {
            nextWorkerAction.Init();
            AE.StartNewWorkerAction?.Invoke(this);
        }
        else
        {
            StartCoroutine(IdleForRetryNewAction());
        }
    }

    private IVillagerWorkAction DetermineNextWorkerAction()
    {
        if (GetActiveWorkAction() != null)
        {
            throw new System.Exception("DetermineNextWorkerAction? zou niet moeten bij actieve acties");
        }

        var availableActions = villagerWorkActions.Where(x => x.CanDoAction()).ToList();
        if(!availableActions.Any())
        {
            return null; 
        }

        var highestPrioWorkActions = availableActions
            .Where(y => y.GetPrio() == availableActions.Min(z => z.GetPrio()))
            .ToList();

        return highestPrioWorkActions[Random.Range(0, highestPrioWorkActions.Count)];
    }   

    public IVillagerWorkAction GetActiveWorkAction() => villagerWorkActions.FirstOrDefault(x => x.IsActive());

    private IEnumerator IdleForRetryNewAction()
    {
        isIdle = true;
        yield return Wait4Seconds.Get(TimeToWaitForRetryIfNoNewAction);
        isIdle = false; // via update nieuwe poging
    }

    private void UpdateAnimation()
    {
        var activeWorkAction = GetActiveWorkAction();
        if (!workerIsActive || activeWorkAction == null)
        {
            animator.SetBool(Constants.ANIM_BOOL_IS_WALKING, !navMeshAgent.isStopped);
            animator.SetBool(Constants.ANIM_BOOL_IS_WORKING, false);
            animator.SetBool(Constants.ANIM_BOOL_IS_WORKING_2, false);
            animator.SetBool(Constants.ANIM_BOOL_IS_IDLE, navMeshAgent.isStopped);
        }
        else
        {
            var animationStatus = activeWorkAction.GetAnimationStatus();
            animator.SetBool(Constants.ANIM_BOOL_IS_WALKING, animationStatus.IsWalking);
            animator.SetBool(Constants.ANIM_BOOL_IS_WORKING, animationStatus.IsWorking);
            animator.SetBool(Constants.ANIM_BOOL_IS_WORKING_2, animationStatus.IsWorking2);
            animator.SetBool(Constants.ANIM_BOOL_IS_IDLE, animationStatus.IsIdle);
        }
    }
}