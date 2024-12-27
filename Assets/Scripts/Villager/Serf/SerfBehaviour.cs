using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SerfBehaviour : BaseAEMonoCI, IHasStopped, IVillagerUnit
{
    [ComponentInject] private NavMeshAgent NavMeshAgent;
    public SerfOrder _currentSerfOrder { get; private set; }
    [ComponentInject] private Animator Animator;
    [ComponentInject(Required.OPTIONAL)] private AudioSource AudioSource;
    [ComponentInject] private List<Renderer> Renderers;

    [ComponentInject] private SerfResourceCarryingBehaviour SerfResourceCarryingBehaviour;
    [ComponentInject] private FoodConsumptionBehaviour FoodConsumptionBehaviour;

    private float WaitTimeInSecCompletingSerfFromRequest = 2;
    private float WaitTimeInSecCompletingSerfToRequest = 1;

    public VillagerUnitType GetVillagerUnitType() => VillagerUnitType.Serf;
    public GameObject GetGO() => this.gameObject;

    protected override void Awake()
    {
        // Geen CI -> components worden door onderstaand event pas toegevoegd
        AE.NewVillagerUnit?.Invoke(this);
    }

    void Start()
    {
        this.ComponentInject(); // nu CI; components zijn toegevoegd
        AE.FreeSerf?.Invoke(this);
    }

    public bool HasStoppedWithLogic() => stoppedWithOrders;

    protected override void OnFoodStatusHasChanged(FoodConsumption foodConsumption, FoodConsumptionStatus previousStatus)
    {
        if(foodConsumption == FoodConsumptionBehaviour.FoodConsumption)
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

    protected override void OnOrderStatusChanged(SerfOrder serfOrder)
    {
        if (serfOrder == _currentSerfOrder)
        {
            if(SerfResourceCarryingBehaviour != null)
            {
                SerfResourceCarryingBehaviour.UpdateSerfOrder(serfOrder);
            }

            switch (serfOrder.Status)
            {
                case Status.SUCCESS:
                case Status.FAILED:
                    {
                        _currentSerfOrder = null;
                        if (NavMeshAgent.isActiveAndEnabled)
                        {
                            NavMeshAgent.isStopped = true;
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
            _currentSerfOrder == null)
        {
            GameManager.Instance.TryRemoveSerfFromFreeSerfList(this);
            stoppedWithOrders = true;                       
        }
    }

    private void UpdateCurrentOrder()
    {
        if (_currentSerfOrder != null && NavMeshAgent.isActiveAndEnabled)
        {
            UpdateNavMesh();

            if (NavMeshAgent.StoppedAtDestination() && !isHandlingStatusSwitch() && canCompleteOrder(_currentSerfOrder))
            {
                switch (_currentSerfOrder.Status)
                {
                    case Status.IN_PROGRESS_FROM:
                        {
                            // indien niet kan (bv warehouse is leeg), dan daarna naar failed setten PER actie (in stockpilebehaviour in dit vb). Met delay.
                            StartCoroutine(CompletedFromRequest(_currentSerfOrder));
                            break;
                        }
                    case Status.IN_PROGRESS_TO:
                        {
                            StartCoroutine(CompletedToRequest(_currentSerfOrder));
                            break;
                        }
                }
            }
        }
    }    

    private void UpdateNavMesh()
    {
        NavMeshAgent.isStopped = false;
        var destination = _currentSerfOrder.Location;

        var path = new NavMeshPath();
        if (!NavMesh.CalculatePath(
            this.transform.position,
            destination,
            _currentSerfOrder.Purpose.ToAreaMask(),
            path))
        {
            // This may be a terribly expensive operation. May be better to remember the currentOrder.From and if it changed, reevaluate the areamask?
            //Debug.LogWarning("Overwriting the navmesh-areaMask for the serf because there is no path.");
            NavMeshAgent.areaMask = 1 << 0; // can happen, so lets just make an exception here
        }
        else
        {
            NavMeshAgent.areaMask = _currentSerfOrder.Purpose.ToAreaMask();
        }


        switch (_currentSerfOrder.Status)
        {
            case Status.IN_PROGRESS_FROM:
            case Status.IN_PROGRESS_TO:
                NavMeshAgent.destination = destination;
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
        serfShowPackageHandling = gameObject.AddComponent<SerfShowPackageHandling>();
        AE.StartCompletingSerfRequest?.Invoke(_currentSerfOrder);
        yield return Wait4Seconds.Get(WaitTimeInSecCompletingSerfFromRequest);
        Destroy(serfShowPackageHandling);
        _currentSerfOrder.Status = Status.IN_PROGRESS_TO;
    }

    private IEnumerator CompletedToRequest(SerfOrder currentSerfOrder)
    {
        serfShowPackageHandling = gameObject.AddComponent<SerfShowPackageHandling>();
        AE.StartCompletingSerfRequest?.Invoke(_currentSerfOrder);
        yield return Wait4Seconds.Get(WaitTimeInSecCompletingSerfToRequest);
        Destroy(serfShowPackageHandling);
        _currentSerfOrder.Status = Status.SUCCESS;
    }

    private void UpdateAnimation()
    {
        Animator.SetBool(Constants.ANIM_BOOL_IS_WALKING, IsWalking());
        Animator.SetBool(Constants.ANIM_BOOL_IS_IDLE, !IsWalking());
    }

    private bool IsWalking()
    {
        return NavMeshAgent.isActiveAndEnabled &&
            !(NavMeshAgent.destination == null || 
            NavMeshAgent.isStopped ||
            NavMeshAgent.StoppedAtDestination());
    }

    public void Assign(SerfOrder serfOrder)
    {
        this._currentSerfOrder = serfOrder;
        serfOrder.Assignee = this;
        RecalculateNavMeshPriority();

        if (SerfResourceCarryingBehaviour != null)
        {
            SerfResourceCarryingBehaviour.UpdateSerfOrder(serfOrder);
        }
    }

    private void RecalculateNavMeshPriority()
    {
        this.NavMeshAgent.avoidancePriority =
            50 + // being a serf
            _currentSerfOrder?.Priority ?? 
            99; // being idle has absolutely no priority
    }

    public void OnDestroy()
    {
        if (this._currentSerfOrder != null)
        {
            this._currentSerfOrder.Status = Status.FAILED;
        }
        GameManager.Instance.TryRemoveSerfFromFreeSerfList(this);
    }    
}