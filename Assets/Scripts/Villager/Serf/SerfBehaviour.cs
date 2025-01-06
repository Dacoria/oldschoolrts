using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SerfBehaviour : BaseAEMonoCI, IHasStopped, IVillagerUnit
{
    [ComponentInject] private NavMeshAgent navMeshAgent;
    public SerfOrder CurrentSerfOrder { get; private set; }
    [ComponentInject] private Animator animator;
    [ComponentInject(Required.OPTIONAL)] private AudioSource audioSource;
    [ComponentInject] private List<Renderer> renderers;

    [ComponentInject] private SerfResourceCarryingBehaviour serfResourceCarryingBehaviour;
    [ComponentInject] private FoodConsumptionBehaviour foodConsumptionBehaviour;

    private float waitTimeInSecCompletingSerfFromRequest = 2;
    private float waitTimeInSecCompletingSerfToRequest = 1;

    public VillagerUnitType GetVillagerUnitType() => VillagerUnitType.Serf;
    public GameObject GetGO() => this.gameObject;
    public bool IsVillagerWorker() => false;

    protected override void Awake()
    {
        // Geen CI -> components via start toegevoegd (event is te traag -> via awake unit)
        NewVillagerComponentsManager.NewVillagerUnit(this);
    }

    void Start()
    {        
        this.ComponentInject(); // nu CI; components zijn toegevoegd
        AE.NewVillagerUnit?.Invoke(this);
        AE.FreeSerf?.Invoke(this);
    }

    public bool HasStoppedWithLogic() => stoppedWithOrders;

    protected override void OnFoodStatusHasChanged(FoodConsumption foodConsumption, FoodConsumptionStatus previousStatus)
    {
        if(foodConsumption == foodConsumptionBehaviour.FoodConsumption)
        {
            switch(foodConsumption.FoodConsumptionStatus)
            {
                case FoodConsumptionStatus.NEEDS_REFILL:
                    stopAsapWithOrders = true;
                    break;
                case FoodConsumptionStatus.REFILL_SUCCESS:
                case FoodConsumptionStatus.REFILL_FAILED:
                    stopAsapWithOrders = false;
                    stoppedWithOrders = false;
                    AE.FreeSerf?.Invoke(this);
                    break;
            }
        }
    }    

    protected override void OnOrderStatusChanged(SerfOrder serfOrder, Status prevStatus)
    {
        if (serfOrder == CurrentSerfOrder)
        {
            if(serfResourceCarryingBehaviour != null)
            {
                serfResourceCarryingBehaviour.UpdateSerfOrder(serfOrder);
            }

            switch (serfOrder.Status)
            {
                case Status.SUCCESS:
                case Status.FAILED:
                    {
                        CurrentSerfOrder = null;
                        if (navMeshAgent.isActiveAndEnabled)
                        {
                            navMeshAgent.isStopped = true;
                        }
                        if(!stopAsapWithOrders && !stoppedWithOrders)
                        {
                            AE.FreeSerf?.Invoke(this);
                        }
                        break;
                    }
            }

            RecalculateNavMeshPriority();
        }
    }

    private bool stopAsapWithOrders;
    private bool stoppedWithOrders;

    void Update()
    {
        UpdateFoodRefilling();
        UpdateCurrentOrder();
        UpdateAnimation();
    }
    
    private void UpdateFoodRefilling()
    {
        if(stopAsapWithOrders &&
            !stoppedWithOrders && 
            CurrentSerfOrder == null)
        {
            GameManager.Instance.TryRemoveSerfFromFreeSerfList(this);
            stoppedWithOrders = true;                       
        }
    }

    private void UpdateCurrentOrder()
    {
        if (CurrentSerfOrder != null && navMeshAgent.isActiveAndEnabled)
        {
            UpdateNavMesh();

            if (navMeshAgent.StoppedAtDestination() && !isHandlingStatusSwitch() && canCompleteOrder(CurrentSerfOrder))
            {
                switch (CurrentSerfOrder.Status)
                {
                    case Status.IN_PROGRESS_FROM:
                        {
                            StartCoroutine(CompletedFromRequest(CurrentSerfOrder));
                            break;
                        }
                    case Status.IN_PROGRESS_TO:
                        {
                            StartCoroutine(CompletedToRequest(CurrentSerfOrder));
                            break;
                        }
                }
            }
        }
    }    

    private void UpdateNavMesh()
    {
        navMeshAgent.isStopped = false;
        var destination = CurrentSerfOrder.Location;

        var path = new NavMeshPath();
        if (!NavMesh.CalculatePath(
            this.transform.position,
            destination,
            CurrentSerfOrder.Purpose.ToAreaMask(),
            path))
        {
            // This may be a terribly expensive operation. May be better to remember the currentOrder.From and if it changed, reevaluate the areamask?
            //Debug.LogWarning("Overwriting the navmesh-areaMask for the serf because there is no path.");
            navMeshAgent.areaMask = 1 << 0; // can happen, so lets just make an exception here
        }
        else
        {
            navMeshAgent.areaMask = CurrentSerfOrder.Purpose.ToAreaMask();
        }


        switch (CurrentSerfOrder.Status)
        {
            case Status.IN_PROGRESS_FROM:
            case Status.IN_PROGRESS_TO:
                navMeshAgent.destination = destination;
                break;
        }
    }

    private bool canCompleteOrder(SerfOrder currentSerfOrder)
    {
        var relevantGo = currentSerfOrder.To.GameObject;
        if (currentSerfOrder.Status == Status.IN_PROGRESS_FROM)
        {
            relevantGo = currentSerfOrder.From.GameObject;
        }

        return !GameManager.Instance.SerfOrderIsBeingCompletedForGo(relevantGo);
    }

    private bool isHandlingStatusSwitch() => serfShowPackageHandling != null;

    private SerfShowPackageHandling serfShowPackageHandling;    

    private IEnumerator CompletedFromRequest(SerfOrder currentSerfOrder)
    {
        AE.StartCompletingSerfRequest?.Invoke(CurrentSerfOrder); // voorkomt dat gebouw andere orders afhandelt
        yield return Wait4Seconds.Get(waitTimeInSecCompletingSerfFromRequest);
        if (currentSerfOrder.From.OrderDestination.CanProcessOrder(currentSerfOrder))
        {
            CurrentSerfOrder.Status = Status.IN_PROGRESS_TO;
        }
        else
        {
            CurrentSerfOrder.Status = Status.FAILED;
        }
    }

    private IEnumerator CompletedToRequest(SerfOrder currentSerfOrder)
    {
        AE.StartCompletingSerfRequest?.Invoke(CurrentSerfOrder); // voorkomt dat gebouw andere orders afhandelta
        yield return Wait4Seconds.Get(waitTimeInSecCompletingSerfToRequest);
        if(currentSerfOrder.To.OrderDestination.CanProcessOrder(currentSerfOrder))
        {
            CurrentSerfOrder.Status = Status.SUCCESS;
        }
        else
        {
            CurrentSerfOrder.Status = Status.FAILED;
        }
        
    }

    private void UpdateAnimation()
    {
        animator.SetBool(Constants.ANIM_BOOL_IS_WALKING, IsWalking());
        animator.SetBool(Constants.ANIM_BOOL_IS_IDLE, !IsWalking());
    }

    private bool IsWalking()
    {
        return navMeshAgent.isActiveAndEnabled &&
            !(navMeshAgent.destination == null || 
            navMeshAgent.isStopped ||
            navMeshAgent.StoppedAtDestination());
    }

    public void Assign(SerfOrder serfOrder)
    {
        this.CurrentSerfOrder = serfOrder;
        serfOrder.Assignee = this;
        RecalculateNavMeshPriority();

        if (serfResourceCarryingBehaviour != null)
        {
            serfResourceCarryingBehaviour.UpdateSerfOrder(serfOrder);
        }
    }

    private void RecalculateNavMeshPriority()
    {
        this.navMeshAgent.avoidancePriority =
            50 + // being a serf
            CurrentSerfOrder?.Priority ?? 
            99; // being idle has absolutely no priority
    }

    public void OnDestroy()
    {
        if (this.CurrentSerfOrder != null)
        {
            this.CurrentSerfOrder.Status = Status.FAILED;
        }
        GameManager.Instance.TryRemoveSerfFromFreeSerfList(this);
    }    
}