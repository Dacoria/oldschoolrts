using UnityEngine;

public class NavMeshStuckFixer : MonoBehaviourSlowUpdateTimeCI
{
    // Truc: Zit vast? Dan randomize 'initialAvoidancePriority' elke 0.5 sec

    [ComponentInject] private UnityEngine.AI.NavMeshAgent NavMashAgent;
    
    new void Start()
    {
        base.Start();
        initialAvoidancePriority = NavMashAgent.avoidancePriority;
    }

    private int initialAvoidancePriority;

    protected override int MsTillSlowUpdate => 500;
    protected override void SlowUpdate()
    {
        if(!NavMeshIsActive() || !IsStuck())
        {
            ResetToInitialNavmeshAgent();
        }
        else
        {
            if (!isStillStuck)
            {
                isStillStuck = true;
            }
            else
            {
                // randomizen lijkt het beste te werken -> ander gedrag forceren bij serfs tot je weer beweegt
                NavMashAgent.avoidancePriority = Random.Range(1, 99);
            }
        }

        prevLoc = transform.position;
    }

    private void ResetToInitialNavmeshAgent()
    {
        prevLoc = null;
        isStillStuck = false;
        if (NavMashAgent.avoidancePriority != initialAvoidancePriority)
        {
            NavMashAgent.avoidancePriority = initialAvoidancePriority;
        }
    }

    private bool NavMeshIsActive()
    {
        return NavMashAgent.enabled && 
            !NavMashAgent.isStopped && 
            NavMashAgent.hasPath;
    }

    private bool isStillStuck;
    private Vector3? prevLoc;    

    private bool IsStuck()
    {
        if(!prevLoc.HasValue)
        {
            return false;
        }    

        return Vector3.Distance(transform.position, prevLoc.Value) < 0.05f;
    }
}