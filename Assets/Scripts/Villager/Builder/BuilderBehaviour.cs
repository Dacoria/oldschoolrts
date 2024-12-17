using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BuilderBehaviour : BaseAEMono, IHasStopped, IVillagerUnit
{    
    public BuilderRequest _currentBuilderRequest { private set; get; }

    [ComponentInject] private NavMeshAgent NavMeshAgent;
    [ComponentInject] public NavMeshObstacle myNavMeshObstacle;
    [ComponentInject] private Animator Animator;
    [ComponentInject] private FoodConsumptionBehaviour FoodConsumptionBehaviour;

    private bool IsWorking;

    private new void Awake()
    {
        base.Awake();
        this.ComponentInject();

        NavMeshAgent.areaMask = 1 << 0;
    }

    private void Start()
    {
        AE.FreeBuilder(this);
    }   

    public bool HasStoppedWithLogic()
    {
        return stoppedWithOrders;
    }

    protected override void OnBuilderRequestStatusChanged(BuilderRequest builderRequest, BuildStatus previousStatus)
    {
        if (builderRequest == _currentBuilderRequest)
        {
            switch (builderRequest.Status)
            {
                case BuildStatus.COMPLETED_BUILDING:
                case BuildStatus.COMPLETED_PREPARING:
                    _currentBuilderRequest = null;
                    if (!stopAsapWithOrders && !stoppedWithOrders)
                    {
                        AE.FreeBuilder(this);
                    }
                    break;
                case BuildStatus.CANCEL:
                    _currentBuilderRequest = null;
                    if (!stopAsapWithOrders && !stoppedWithOrders)
                    {
                        AE.FreeBuilder(this);

                    }
                    break;
            }

            RecalculateNavMeshPriority();
        }
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
                    Debug.Log("FoodConsumptionStatus.REFILL_SUCCESS");
                    stopAsapWithOrders = false;
                    stoppedWithOrders = false;
                    AE.FreeBuilder(this);
                    break;
                case FoodConsumptionStatus.REFILL_FAILED:
                    stopAsapWithOrders = false;
                    stoppedWithOrders = false;
                    AE.FreeBuilder(this);
                    Debug.Log("FoodConsumptionStatus.REFILL_FAILED");
                    break;
            }
        }
    }

    private bool stopAsapWithOrders;
    private bool stoppedWithOrders;

    private void UpdateFoodRefilling()
    {
        if (stopAsapWithOrders &&
            !stoppedWithOrders &&
            _currentBuilderRequest == null)
        {
            GameManager.Instance.TryRemoveBuilderFromFreeBuilderList(this);
            stoppedWithOrders = true;
        }
    }

    private void Update()
    {
        UpdateFoodRefilling();
        if (!IsWorking)
        {
            if (_currentBuilderRequest != null && NavMeshAgent.isOnNavMesh)
            {
                NavMeshAgent.destination = GetClosestPointFromBuilderToGo(_currentBuilderRequest.GameObject);
                var flatPosition = transform.position;
                flatPosition.y = 0;

                if (NavMeshAgent.StoppedAtDestination())
                {
                    if (_currentBuilderRequest.Status == BuildStatus.NEEDS_PREPARE)
                    {
                        var buildingBehaviourOfBuilding = _currentBuilderRequest.GameObject.GetComponent<BuildingBehaviour>();
                        if (buildingBehaviourOfBuilding == null) { throw new System.Exception("Altijd een BuildingBehaviour verwacht bij preparen gebouw"); }
                        StartCoroutine(PrepareTheGround(buildingBehaviourOfBuilding.TimeToPrepareBuildingInSeconds));
                        _currentBuilderRequest.Status = BuildStatus.PREPARING;
                    }
                    if (_currentBuilderRequest.Status == BuildStatus.NEEDS_BUILDING)
                    {
                        var buildingBehaviourOfBuilding = _currentBuilderRequest.GameObject.GetComponent<BuildingBehaviour>();
                        if(buildingBehaviourOfBuilding == null) { throw new System.Exception("Altijd een BuildingBehaviour verwacht bij afronden gebouw"); }
                        StartCoroutine(FinishTheBuilding(buildingBehaviourOfBuilding.TimeToBuildRealInSeconds));
                        _currentBuilderRequest.Status = BuildStatus.BUILDING;
                    }
                }
            }
        }

        Animator.SetBool(Constants.ANIM_BOOL_IS_WALKING, IsWalking());
        Animator.SetBool(Constants.ANIM_BOOL_IS_WORKING, IsWorking && !IsWalking());
        Animator.SetBool(Constants.ANIM_BOOL_IS_IDLE, !IsWalking() && !IsWorking);
    }

    private bool IsWalking()
    {
        return NavMeshAgent.isActiveAndEnabled && 
            !(NavMeshAgent.destination == null ||
            NavMeshAgent.isStopped ||
            NavMeshAgent.StoppedAtDestination());
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
        IsWorking = true;
        NavMeshAgent.enabled = false;
        myNavMeshObstacle.enabled = true;
        yield return MonoHelper.Instance.GetCachedWaitForSeconds(timeToPrepareGround);
        if(this._currentBuilderRequest != null)
        {
            // kan dood gegaan zijn
            myNavMeshObstacle.enabled = false;
            NavMeshAgent.enabled = true;
            this._currentBuilderRequest.Status = BuildStatus.COMPLETED_PREPARING;
            IsWorking = false;
        }       
    }

    private IEnumerator FinishTheBuilding(float timeToBuildBuilding)
    {
        IsWorking = true;
        NavMeshAgent.enabled = false;
        myNavMeshObstacle.enabled = true;
        yield return MonoHelper.Instance.GetCachedWaitForSeconds(timeToBuildBuilding);
        if (this._currentBuilderRequest != null)
        {
            // kan doodgegaan zijn
            myNavMeshObstacle.enabled = false;
            NavMeshAgent.enabled = true;
            this._currentBuilderRequest.Status = BuildStatus.COMPLETED_BUILDING;
            IsWorking = false;
        }
    }

    public void AssignBuilderRequest(BuilderRequest builderRequest)
    {
        _currentBuilderRequest = builderRequest;
        NavMeshAgent.isStopped = false; // foodbehaviour kan dit uitzetten (maakt zelf gebruik van enabled true/false ipv stopped)
        RecalculateNavMeshPriority();
    }

    private void RecalculateNavMeshPriority()
    {
        this.NavMeshAgent.avoidancePriority =
            _currentBuilderRequest?.Priority ??
            99; // being idle has absolutely no priority
    }   

    public void OnDestroy()
    {
        GameManager.Instance.TryRemoveBuilderFromFreeBuilderList(this);
        if (this._currentBuilderRequest != null)
        {
            this._currentBuilderRequest.Status = BuildStatus.CANCEL;
        }
    }

    public VillagerUnitType GetVillagerUnitType() => VillagerUnitType.Builder;
}