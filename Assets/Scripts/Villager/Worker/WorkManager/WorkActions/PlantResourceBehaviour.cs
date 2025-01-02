using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlantResourceBehaviour : MonoBehaviourCI, IVillagerWorkAction
{
    [ComponentInject] private NavMeshAgent NavMeshAgent;
    private ILocForNewResource LocForNewResourceScript;

    private GameObject NewRscToPlantGo;
    private GameObject ObjectToBringResourceBackTo;
    public GameObject ObjectToPlantPrefab;

    public float timeToCreateNewResourceInSeconds = 5f;
    public float stopDistanceFromObjectToCreateNewResource = 2f;
    public bool PlantNewResourceOnFarmField;

    private bool isActive;

    private bool isCreatingNewResource; // voor animatie

    public int GetPrio() => 2;
    public bool IsActive() => isActive;

    public void SetReturnTargetForAction(GameObject objectToBringResourceBackTo)
    {
        ObjectToBringResourceBackTo = objectToBringResourceBackTo;
        LocForNewResourceScript = ObjectToBringResourceBackTo.GetComponent<ILocForNewResource>(); // waar kan/mag de nieuwe resource gemaakt & geplaatst worden? + teruggeven prefab nieuwe GO
        if (LocForNewResourceScript == null) { throw new System.Exception("LocationOfNewResourceScript -> Nodig voor bepalen objectToRetrieveResourceFrom"); }
    }

    public bool CanDoAction()
    {
        return LocForNewResourceScript.GetLocationForNewResource() != null;
    }

    public void Init()
    {
        isActive = true;
        var succes = CreateNewResourceTarget();
        if(!succes)
        {
            isActive = false;
        }
    }

    public void Finished()
    {
        isActive = false;
    }

    public AnimationStatus GetAnimationStatus()
    {
        return new AnimationStatus
        {
            IsWalking = NavMeshAgent.enabled && !NavMeshAgent.isStopped,
            IsWorking2 = isCreatingNewResource
        };
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
            NavMeshAgent.isStopped = true;
            if (NewRscToPlantGo != null && NewRscToPlantGo.transform.position.IsSameXAndZ(NavMeshAgent.destination))
            {                
                StartCoroutine(CreateNewResource());
            }
            else if (ObjectToBringResourceBackTo != null && ObjectToBringResourceBackTo.EntranceExit().IsSameXAndZ(NavMeshAgent.destination))
            {
                Finished();                
            }
            else
            {
                // Wel bestemming bereikt; maar ligt blijkbaar niks --> terug naar basis voor fallback
                Debug.Log("Bestemming bereikt zonder actie --> zou niet moeten");
                GoBackToGatherPoint();
            }
        }
    }

    private IEnumerator CreateNewResource()
    {
        isCreatingNewResource = true;
        yield return Wait4Seconds.Get(timeToCreateNewResourceInSeconds);
        isCreatingNewResource = false;

        NewRscToPlantGo.SetActive(true); // bv een boom -> maakt zichtbaar
        NewRscToPlantGo = null; // wordt evt later gekoppeld als resource om te harvesten --> nu resetten

        GoBackToGatherPoint();
    }

    private void GoBackToGatherPoint()
    {
        NavMeshAgent.isStopped = false;
        NavMeshAgent.destination = ObjectToBringResourceBackTo.EntranceExit();
    }   

    private bool CreateNewResourceTarget()
    {
        // bij een boom wil je alleen een locatie, bij een farmfield, wil je het farmfield terugkrijgen, en die als parent maken van het nieuwe obj --> nu maar zo gedaan (niet trots op :( )
        var locRsc = LocForNewResourceScript.GetLocationForNewResource();
        if (locRsc == null)
        {
            return false;
        }

        NewRscToPlantGo = Instantiate(ObjectToPlantPrefab, locRsc.V3, Quaternion.identity, locRsc?.GO?.transform);
        NewRscToPlantGo.SetActive(false); // wordt active gemaakt bij het planten van de resource

        NavMeshAgent.SetDestination(NewRscToPlantGo.transform.position);
        NavMeshAgent.stoppingDistance = stopDistanceFromObjectToCreateNewResource;
        NavMeshAgent.isStopped = false;

        return true;
    }

    public bool IsCarryingResource() => NewRscToPlantGo != null && (!NavMeshAgent.isStopped && NavMeshAgent.destination.IsSameXAndZ(NewRscToPlantGo.transform.position));
}