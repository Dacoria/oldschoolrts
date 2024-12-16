using UnityEngine;
using UnityEngine.AI;

public class GoingToWorkStation : MonoBehaviour, IVillagerWorkAction
{
    [HideInInspector]
    public bool actionIsAvailable;

    [ComponentInject] private NavMeshAgent NavMeshAgent;
    private GameObject ObjectToBringResourceBackTo;

    private bool isActive;

    public void SetReturnTargetForAction(GameObject objectToBringResourceBackTo)
    {
        ObjectToBringResourceBackTo = objectToBringResourceBackTo;
    }

    public int GetPrio()
    {
        return 1;
    }

    public bool IsActive()
    {
        return isActive;
    }

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
    void Awake()
    {
        this.ComponentInject();
    }

    public void Start()
    {
        actionIsAvailable = true;
    }

    public void Update()
    {
        if(isActive && NavMeshAgent.StoppedAtDestination() && !NavMeshAgent.isStopped)
        {
            UpdateDestNavAgentReached();
        }
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
}
