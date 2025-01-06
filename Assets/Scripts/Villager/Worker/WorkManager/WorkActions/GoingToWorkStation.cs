using UnityEngine;
using UnityEngine.AI;

public class GoingToWorkStation : MonoBehaviourCI, IVillagerWorkAction
{
    [HideInInspector] public bool ActionIsAvailable;
    [ComponentInject] private NavMeshAgent navMeshAgent;
    private GameObject objectToBringResourceBackTo;

    private bool isActive;

    public void Start()
    {
        ActionIsAvailable = true;
    }

    public void Update()
    {
        if (isActive && navMeshAgent.StoppedAtDestination() && !navMeshAgent.isStopped)
        {
            UpdateDestNavAgentReached();
        }
    }    

    public int GetPrio() => 1;
    public bool IsActive() => isActive;

    public bool CanDoAction()
    {
        // MEER?
        return ActionIsAvailable;
    }

    public void Init()
    {
        isActive = true;
        GoBackToGatherPoint();
    }

    public void Finished()
    {
        ActionIsAvailable = false; // standaard maar 1x -> kan overschreven worden (public)
        isActive = false;
        navMeshAgent.areaMask = 1 << NavMesh.GetAreaFromName(Constants.AREA_MASK_WALKABLE);
    }

    public AnimationStatus GetAnimationStatus()
    {
        return new AnimationStatus
        {
            IsWalking = navMeshAgent.enabled && !navMeshAgent.isStopped,
        };
    }

    private void UpdateDestNavAgentReached()
    {
        if (navMeshAgent.StoppedAtDestination() && !navMeshAgent.isStopped)
        {
            Finished();
        }
    }

    private void GoBackToGatherPoint()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.destination = objectToBringResourceBackTo.EntranceExit();
    }

    public void SetReturnTargetForAction(GameObject objectToBringResourceBackTo)
    {
        this.objectToBringResourceBackTo = objectToBringResourceBackTo;
    }
}