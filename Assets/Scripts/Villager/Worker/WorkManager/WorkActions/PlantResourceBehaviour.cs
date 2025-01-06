using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlantResourceBehaviour : MonoBehaviourCI, IVillagerWorkAction
{
    [ComponentInject] private NavMeshAgent navMeshAgent;
    private ILocForNewResource locForNewResourceScript;

    private GameObject newRscToPlantGo;
    private GameObject objectToBringResourceBackTo;
    public GameObject ObjectToPlantPrefab;

    public float TimeToCreateNewResourceInSeconds = 5f;
    public float StopDistanceFromObjectToCreateNewResource = 2f;
    public bool PlantNewResourceOnFarmField;

    private bool isActive;

    private bool isCreatingNewResource; // voor animatie

    public int GetPrio() => 2;
    public bool IsActive() => isActive;

    public void SetReturnTargetForAction(GameObject objectToBringResourceBackTo)
    {
        this.objectToBringResourceBackTo = objectToBringResourceBackTo;
        locForNewResourceScript = this.objectToBringResourceBackTo.GetComponent<ILocForNewResource>(); // waar kan/mag de nieuwe resource gemaakt & geplaatst worden? + teruggeven prefab nieuwe GO
        if (locForNewResourceScript == null) { throw new System.Exception("LocationOfNewResourceScript -> Nodig voor bepalen objectToRetrieveResourceFrom"); }
    }

    public bool CanDoAction()
    {
        return locForNewResourceScript.GetLocationForNewResource() != null;
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
            IsWalking = navMeshAgent.enabled && !navMeshAgent.isStopped,
            IsWorking2 = isCreatingNewResource
        };
    }
    public void Update()
    {
        if(isActive && navMeshAgent.StoppedAtDestination() && !navMeshAgent.isStopped)
        {
            UpdateDestNavAgentReached();
        }
    }

    private void UpdateDestNavAgentReached()
    {
        if (navMeshAgent.StoppedAtDestination() && !navMeshAgent.isStopped)
        {
            navMeshAgent.isStopped = true;
            if (newRscToPlantGo != null && newRscToPlantGo.transform.position.IsSameXAndZ(navMeshAgent.destination))
            {                
                StartCoroutine(CreateNewResource());
            }
            else if (objectToBringResourceBackTo != null && objectToBringResourceBackTo.EntranceExit().IsSameXAndZ(navMeshAgent.destination))
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
        yield return Wait4Seconds.Get(TimeToCreateNewResourceInSeconds);
        isCreatingNewResource = false;

        newRscToPlantGo.SetActive(true); // bv een boom -> maakt zichtbaar
        newRscToPlantGo = null; // wordt evt later gekoppeld als resource om te harvesten --> nu resetten

        GoBackToGatherPoint();
    }

    private void GoBackToGatherPoint()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.destination = objectToBringResourceBackTo.EntranceExit();
    }   

    private bool CreateNewResourceTarget()
    {
        // bij een boom wil je alleen een locatie, bij een farmfield, wil je het farmfield terugkrijgen, en die als parent maken van het nieuwe obj --> nu maar zo gedaan (niet trots op :( )
        var locRsc = locForNewResourceScript.GetLocationForNewResource();
        if (locRsc == null)
        {
            return false;
        }

        newRscToPlantGo = Instantiate(ObjectToPlantPrefab, locRsc.V3, Quaternion.identity, locRsc?.GO?.transform);
        newRscToPlantGo.SetActive(false); // wordt active gemaakt bij het planten van de resource

        navMeshAgent.SetDestination(newRscToPlantGo.transform.position);
        navMeshAgent.stoppingDistance = StopDistanceFromObjectToCreateNewResource;
        navMeshAgent.isStopped = false;

        return true;
    }

    public bool IsCarryingResource() => newRscToPlantGo != null && (!navMeshAgent.isStopped && navMeshAgent.destination.IsSameXAndZ(newRscToPlantGo.transform.position));
}