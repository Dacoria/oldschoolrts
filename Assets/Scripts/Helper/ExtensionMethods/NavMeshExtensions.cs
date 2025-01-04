using UnityEngine.AI;

public static class NavMeshExtensions
{
    public static bool StoppedAtDestination(this NavMeshAgent myNavMeshAgent, float additionalStoppingDistance = 0)
    {
        return myNavMeshAgent.isActiveAndEnabled &&
                !myNavMeshAgent.pathPending &&
                myNavMeshAgent.pathStatus != NavMeshPathStatus.PathInvalid &&
                myNavMeshAgent.remainingDistance <= myNavMeshAgent.stoppingDistance + additionalStoppingDistance &&
                myNavMeshAgent.velocity.sqrMagnitude == 0f;
    }
}