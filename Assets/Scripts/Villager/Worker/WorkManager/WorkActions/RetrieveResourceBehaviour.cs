using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RetrieveResourceBehaviour : MonoBehaviourCI, IVillagerWorkAction, IToolShowVillager
{
    [ComponentInject] private NavMeshAgent navMeshAgent;
    private GameObject objectToBringResourceBackTo;
    private GameObject objectToRetrieveResourceFrom;

    private IRetrieveResourceFromObject retrieveResourceScript;
    private ILocationOfResource locationOfResourceScript;

    private HarvestMaterialResource resourceCarriedBack;
    private ProduceResourceManualBehaviour produceResourceBehaviour;

    public float StopDistanceFromObjectToRetrieveResourceFrom = 2f;

    private bool isCarryingResourceToBringBack => resourceCarriedBack != null;
    private bool isDroppingResourceOff;
    private bool isRetrievingResource;

    public float TimeToDropOffResourceInSeconds = 1f;
    public float TimeToRetrieveResourceInSeconds = 5f;
    public float TimeToWaitForRetrievalOfResourceInSeconds = 0f;

    private bool isActive;

    public void SetReturnTargetForAction(GameObject objectToBringResourceBackTo)
    {
        this.objectToBringResourceBackTo = objectToBringResourceBackTo;
        locationOfResourceScript = this.objectToBringResourceBackTo.GetComponent<ILocationOfResource>(); // waar moet de resource vandaan gehaald worden?
        if (locationOfResourceScript == null) { throw new System.Exception("LocationOfResourceScript -> Nodig voor bepalen objectToRetrieveResourceFrom"); }

        produceResourceBehaviour = this.objectToBringResourceBackTo.GetComponent<ProduceResourceManualBehaviour>();
        if (produceResourceBehaviour == null) { throw new System.Exception("ProduceResourceManualBehaviour -> Nodig voor ophalen rsc"); }
    }

    public int GetPrio() => 2;
    public bool IsActive() => isActive;

    public bool CanDoAction()
    {
        if(locationOfResourceScript != null && objectToBringResourceBackTo != null)
        {        
            objectToRetrieveResourceFrom = locationOfResourceScript.GetResourceToRetrieve();
            return objectToRetrieveResourceFrom != null && HasEnoughBufferForResource();
        }

        return false;
    }

    private bool HasEnoughBufferForResource()
    {
        var produceResourceBehaviour = objectToBringResourceBackTo.GetComponent<ProduceResourceManualBehaviour>();
        if (produceResourceBehaviour != null)
        {
            var stockPile = objectToBringResourceBackTo.GetComponent<HandleProduceResourceOrderBehaviour>().OutputOrders.Count;
            var producedPerRun = produceResourceBehaviour.ProducedPerRun;
            var maxBuffer = produceResourceBehaviour.MaxBuffer;

            return stockPile + producedPerRun <= maxBuffer;
        }

        return true;
    }

    public void Init()
    {
        isActive = true;
        objectToRetrieveResourceFrom = locationOfResourceScript.GetResourceToRetrieve();
        if(objectToRetrieveResourceFrom != null)
        {
            TargetResourceToRetrieve();
        }
        else
        {
            GoBackToGatherPoint();
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
            IsWorking = isRetrievingResource,
            IsIdle = isDroppingResourceOff
        };
    }

    public void Update()
    {
        if (isActive && navMeshAgent.StoppedAtDestination() && !navMeshAgent.isStopped)
        {
            UpdateDestNavAgentReached();
        }
    }

    private void UpdateDestNavAgentReached()
    {
        if (navMeshAgent.StoppedAtDestination() && !navMeshAgent.isStopped)
        {
            navMeshAgent.isStopped = true;
            DestinationReached();
        }
    }

    private void DestinationReached()
    {
        // probeert rsc op te halen?
        if (objectToRetrieveResourceFrom != null && objectToRetrieveResourceFrom.transform.position.IsSameXAndZ(navMeshAgent.destination))
        {
            if (retrieveResourceScript.CanRetrieveResource())
            {
                StartCoroutine(RetrievingResource());
            }
            else
            {
                // bv bij boom omhakken, dat een andere forester al bezig is --> ga dan terug
                GoBackToGatherPoint();
            }
        }
        // brengt resource terug?
        else if (objectToBringResourceBackTo != null && objectToBringResourceBackTo.EntranceExit().IsSameXAndZ(navMeshAgent.destination))
        {
            navMeshAgent.isStopped = true;
            if (isCarryingResourceToBringBack)
            {
                StartCoroutine(DroppingResourceOff());
            }
            else
            {
                Finished();
            }
        }
        else
        {
            // Wel bestemming bereikt; maar licht blijkbaar niks --> terug naar basis voor fallback
            Debug.Log("Bestemming bereikt zonder actie --> zou niet moeten");
            GoBackToGatherPoint();
        }
    }

    private void GoBackToGatherPoint()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.destination = objectToBringResourceBackTo.EntranceExit();
    }  

    private IEnumerator RetrievingResource()
    {
        isRetrievingResource = true;
        retrieveResourceScript.StartRetrievingResource(produceResourceBehaviour.ProducedPerRun);
        yield return Wait4Seconds.Get(TimeToRetrieveResourceInSeconds);
        isRetrievingResource = false;
        resourceCarriedBack = retrieveResourceScript.ResourceIsRetrieved();

        var waitTime = resourceCarriedBack?.MaterialCount > 0 ? TimeToWaitForRetrievalOfResourceInSeconds : 0; // bv als een andere villiager de resource voor je neus heeft weggekaapt; dan niet wachten
        MonoHelper.Instance.Do_CR(waitTime, () => BringResourceBack());
    }

    private void BringResourceBack()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.destination = objectToBringResourceBackTo.EntranceExit();
        navMeshAgent.stoppingDistance = 0;
    }

    private IEnumerator DroppingResourceOff()
    {
        isDroppingResourceOff = true;
        yield return Wait4Seconds.Get(TimeToDropOffResourceInSeconds);
        produceResourceBehaviour.ProduceResource();

        isDroppingResourceOff = false;
        resourceCarriedBack = null;
        Finished();
    }   

    private void TargetResourceToRetrieve()
    {
        retrieveResourceScript = objectToRetrieveResourceFrom.GetComponent<IRetrieveResourceFromObject>(); // op dit moment optioneel (mag null blijven) -> gebruikt om resource te consumeren (bv boom laten verdwijnen als deze is omgehakt; evt voor stonequery langzaam laten depleten)

        // TODO nette fix --> voor nu nodig om de interfaces werkend te krijgen
        if(retrieveResourceScript == null) { retrieveResourceScript = new RetrieveResourceDummy(); }

        navMeshAgent.SetDestination(objectToRetrieveResourceFrom.transform.position);
        navMeshAgent.stoppingDistance = StopDistanceFromObjectToRetrieveResourceFrom;
        navMeshAgent.isStopped = false;
    }

    public bool IsCarryingResource()
    {
        return isCarryingResourceToBringBack &&
            !navMeshAgent.isStopped && 
            navMeshAgent.destination.IsSameXAndZ(objectToBringResourceBackTo.EntranceExit()            
        );
    }

    public bool ShouldShowTool()
    {
        return
            !isActive || // nog niet actief script
            isRetrievingResource || // bezig met ophalen resources
            (
            objectToRetrieveResourceFrom != null && // gaat naar resource toe lopen
            navMeshAgent.destination.IsSameXAndZ(objectToRetrieveResourceFrom.transform.position)
            );
    }
}