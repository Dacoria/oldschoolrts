using UnityEngine;
using UnityEngine.AI;

public class GoingToWorkStation : MonoBehaviourCI, IVillagerWorkAction
{
    [HideInInspector] public bool actionIsAvailable;
    [ComponentInject] private NavMeshAgent NavMeshAgent;
    private GameObject ObjectToBringResourceBackTo;

    private bool isActive;

    public void Start()
    {
        actionIsAvailable = true;
    }

    public void Update()
    {
        if (isActive && NavMeshAgent.StoppedAtDestination() && !NavMeshAgent.isStopped)
        {
            UpdateDestNavAgentReached();
        }
    }    

    public int GetPrio() => 1;
    public bool IsActive() => isActive;

    public bool CanDoAction()
    {
        // MEER?
        return actionIsAvailable;
    }

    public void Init()
    {
        isActive = true;
        GoBackToGatherPoint();
    }

    public void Finished()
    {
        actionIsAvailable = false; // standaard maar 1x -> kan overschreven worden (public)
        isActive = false;
    }

    public AnimationStatus GetAnimationStatus()
    {
        return new AnimationStatus
        {
            IsWalking = NavMeshAgent.enabled && !NavMeshAgent.isStopped,
        };
    }

    private void UpdateDestNavAgentReached()
    {
        if (NavMeshAgent.StoppedAtDestination() && !NavMeshAgent.isStopped)
        {
            Finished();
        }
    }

    private void GoBackToGatherPoint()
    {
        NavMeshAgent.isStopped = false;
        NavMeshAgent.destination = ObjectToBringResourceBackTo.EntranceExit();
    }

    public void SetReturnTargetForAction(GameObject objectToBringResourceBackTo)
    {
        ObjectToBringResourceBackTo = objectToBringResourceBackTo;
    }
}