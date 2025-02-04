using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BuilderBehaviour : BaseAEMonoCI, IHasStopped, IVillagerUnit
{    
    public BuilderRequest CurrentBuilderRequest { private set; get; }

    [ComponentInject] private NavMeshObstacle navMeshObstacle;
    [ComponentInject] private NavMeshAgent navMeshAgent;
    [ComponentInject] private Animator animator;
    [ComponentInject] private FoodConsumptionBehaviour foodConsumptionBehaviour;

    private bool isWorking;

    public VillagerUnitType GetVillagerUnitType() => VillagerUnitType.Builder;
    public GameObject GetGO() => this.gameObject;
    public bool IsVillagerWorker() => false;

    protected override void Awake()
    {
        // Geen CI -> components via start toegevoegd (event is te traag -> via awake unit)
        NewVillagerComponentsManager.NewVillagerUnit(this);
    }

    private void Start()
    {
        this.ComponentInject(); // nu CI; components zijn toegevoegd
        navMeshAgent.areaMask = 1 << 0;
        AE.NewVillagerUnit?.Invoke(this);
        AE.FreeBuilder?.Invoke(this);
    }

    protected override void OnBuilderRequestStatusChanged(BuilderRequest builderRequest, BuildStatus previousStatus)
    {
        if (builderRequest == CurrentBuilderRequest && builderRequest.Status != BuildStatus.NONE)
        {
            switch (builderRequest.Status)
            {
                case BuildStatus.COMPLETED_BUILDING:
                case BuildStatus.COMPLETED_PREPARING:
                    CurrentBuilderRequest = null;
                    if (!stopAsapWithOrders && !stoppedWithOrders)
                    {
                        AE.FreeBuilder?.Invoke(this);
                    }
                    break;
                case BuildStatus.CANCEL:
                    CurrentBuilderRequest = null;
                    if (!stopAsapWithOrders && !stoppedWithOrders)
                    {
                        AE.FreeBuilder?.Invoke(this);

                    }
                    break;
            }

            RecalculateNavMeshPriority();
        }
    }

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
                    AE.FreeBuilder?.Invoke(this);
                    break;
            }
        }
    }

    private bool stopAsapWithOrders;
    private bool stoppedWithOrders;

    private void UpdateStopOrders()
    {
        if (stopAsapWithOrders &&
            !stoppedWithOrders &&
            CurrentBuilderRequest == null)
        {
            GameManager.Instance.TryRemoveBuilderFromFreeBuilderList(this);
            stoppedWithOrders = true;
        }
    }

    public bool HasStoppedWithLogic() => stoppedWithOrders;

    private void Update()
    {
        UpdateStopOrders();
        if (!isWorking)
        {
            if (CurrentBuilderRequest != null && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.destination = GetClosestPointFromBuilderToGo(CurrentBuilderRequest.GameObject);
                var flatPosition = transform.position;
                flatPosition.y = 0;

                if (navMeshAgent.StoppedAtDestination())
                {
                    if (CurrentBuilderRequest.Status == BuildStatus.NEEDS_PREPARE)
                    {
                        var buildingBehaviourOfBuilding = CurrentBuilderRequest.GameObject.GetComponent<BuildingBehaviour>();
                        if (buildingBehaviourOfBuilding == null) { throw new System.Exception("Altijd een BuildingBehaviour verwacht bij preparen gebouw"); }
                        StartCoroutine(PrepareTheGround(buildingBehaviourOfBuilding.BuildingType.GetBuildDurationSettings().TimeToPrepareBuildingInSeconds));
                        CurrentBuilderRequest.Status = BuildStatus.PREPARING;
                    }
                    if (CurrentBuilderRequest.Status == BuildStatus.NEEDS_BUILDING)
                    {
                        var buildingBehaviourOfBuilding = CurrentBuilderRequest.GameObject.GetComponent<BuildingBehaviour>();
                        if(buildingBehaviourOfBuilding == null) { throw new System.Exception("Altijd een BuildingBehaviour verwacht bij afronden gebouw"); }
                        StartCoroutine(FinishTheBuilding(buildingBehaviourOfBuilding.BuildingType.GetBuildDurationSettings().TimeToBuildRealInSeconds));
                        CurrentBuilderRequest.Status = BuildStatus.BUILDING;
                    }
                }
            }
        }

        animator.SetBool(Constants.ANIM_BOOL_IS_WALKING, IsWalking());
        animator.SetBool(Constants.ANIM_BOOL_IS_WORKING, isWorking && !IsWalking());
        animator.SetBool(Constants.ANIM_BOOL_IS_IDLE, !IsWalking() && !isWorking);
    }

    private bool IsWalking()
    {
        return navMeshAgent.isActiveAndEnabled && 
            !(navMeshAgent.destination == null ||
            navMeshAgent.isStopped ||
            navMeshAgent.StoppedAtDestination());
    }

    private Vector3 GetClosestPointFromBuilderToGo(GameObject goToGoTo)
    {
        var collider = goToGoTo.GetComponent<Collider>();
        if(collider == null)
        {
            Debug.Log(goToGoTo.name + " heeft geen collider -> gebruik locatie van go");
            return goToGoTo.transform.position;
        }

        return collider.ClosestPoint(transform.position);
    }

    private IEnumerator PrepareTheGround(float timeToPrepareGround)
    {
        isWorking = true;
        navMeshAgent.enabled = false;
        navMeshObstacle.enabled = true;
        yield return Wait4Seconds.Get(timeToPrepareGround);
        if(this.CurrentBuilderRequest != null)
        {
            // kan dood gegaan zijn
            navMeshObstacle.enabled = false;
            navMeshAgent.enabled = true;
            this.CurrentBuilderRequest.Status = BuildStatus.COMPLETED_PREPARING;
            isWorking = false;
        }       
    }

    private IEnumerator FinishTheBuilding(float timeToBuildBuilding)
    {
        isWorking = true;
        navMeshAgent.enabled = false;
        navMeshObstacle.enabled = true;
        yield return Wait4Seconds.Get(timeToBuildBuilding);
        if (this.CurrentBuilderRequest != null)
        {
            // kan doodgegaan zijn
            navMeshObstacle.enabled = false;
            navMeshAgent.enabled = true;
            this.CurrentBuilderRequest.Status = BuildStatus.COMPLETED_BUILDING;
            isWorking = false;
        }
    }

    public void AssignBuilderRequest(BuilderRequest builderRequest)
    {
        CurrentBuilderRequest = builderRequest;
        navMeshAgent.isStopped = false; // foodbehaviour kan dit uitzetten (maakt zelf gebruik van enabled true/false ipv stopped)
        RecalculateNavMeshPriority();
    }

    private void RecalculateNavMeshPriority()
    {
        this.navMeshAgent.avoidancePriority =
            CurrentBuilderRequest?.Priority ??
            99; // being idle has absolutely no priority
    }   

    public void OnDestroy()
    {
        GameManager.Instance.TryRemoveBuilderFromFreeBuilderList(this);
        if (this.CurrentBuilderRequest != null)
        {
            this.CurrentBuilderRequest.Status = BuildStatus.CANCEL;
        }
    }
}