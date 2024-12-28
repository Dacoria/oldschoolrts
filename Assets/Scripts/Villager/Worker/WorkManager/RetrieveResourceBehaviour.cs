using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RetrieveResourceBehaviour : MonoBehaviourCI, IVillagerWorkAction, IToolShowVillager
{
    [ComponentInject] private NavMeshAgent NavMeshAgent;
    private GameObject ObjectToBringResourceBackTo;
    private GameObject objectToRetrieveResourceFrom;

    private IRetrieveResourceFromObject RetrieveResourceScript;
    private ILocationOfResource LocationOfResourceScript;

    private HarvestMaterialResource ResourceCarriedBack;

    public float stopDistanceFromObjectToRetrieveResourceFrom = 2f;

    private bool isCarryingResourceToBringBack => ResourceCarriedBack != null;
    private bool isDroppingResourceOff;
    private bool isRetrievingResource;

    public float timeToDropOffResourceInSeconds = 1f;
    public float timeToRetrieveResourceInSeconds = 5f;
    public float timeToWaitForRetrievalOfResourceInSeconds = 0f;

    public int ResourceCountMaterialHarvestedPerRun = 1;

    private bool isActive;

    public void SetReturnTargetForAction(GameObject objectToBringResourceBackTo)
    {
        ObjectToBringResourceBackTo = objectToBringResourceBackTo;
        LocationOfResourceScript = ObjectToBringResourceBackTo.GetComponent<ILocationOfResource>(); // waar moet de resource vandaan gehaald worden?
        if (LocationOfResourceScript == null) { throw new System.Exception("LocationOfResourceScript -> Nodig voor bepalen objectToRetrieveResourceFrom"); }

        // 1 resources te halen? dan is dat je rec. count
        var produceResourceBehaviour = ObjectToBringResourceBackTo.GetComponent<ProduceResourceBehaviour>();
        if (produceResourceBehaviour != null && produceResourceBehaviour.IsSingleProducingItemWithoutConsuming())
        {
            ResourceCountMaterialHarvestedPerRun = produceResourceBehaviour.GetSingleProducingItemWithoutConsuming().ProducedPerProdCycle;
        }
    }

    public int GetPrio() => 2;
    public bool IsActive() => isActive;

    public bool CanDoAction()
    {
        if(LocationOfResourceScript != null && ObjectToBringResourceBackTo != null)
        {        
            objectToRetrieveResourceFrom = LocationOfResourceScript.GetResourceToRetrieve();
            return objectToRetrieveResourceFrom != null && HasEnoughBufferForResource();
        }

        return false;
    }

    private bool HasEnoughBufferForResource()
    {
        var produceResourceBehaviour = ObjectToBringResourceBackTo.GetComponent<ProduceResourceBehaviour>();
        if (produceResourceBehaviour != null && produceResourceBehaviour.IsSingleProducingItemWithoutConsuming())
        {
            var stockPile = ObjectToBringResourceBackTo.GetComponent<ProduceResourceOrderBehaviour>().CurrentOutstandingOrders;
            var producedPerRun = ResourceCountMaterialHarvestedPerRun;
            var maxBuffer = produceResourceBehaviour.GetSingleProducingItemWithoutConsuming().MaxBuffer;

            return stockPile + producedPerRun <= maxBuffer;
        }

        return true;
    }

    public void Init()
    {
        isActive = true;
        objectToRetrieveResourceFrom = LocationOfResourceScript.GetResourceToRetrieve();
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
            IsWalking = NavMeshAgent.enabled && !NavMeshAgent.isStopped,
            IsWorking = isRetrievingResource,
            IsIdle = isDroppingResourceOff
        };
    }

    public void Update()
    {
        if (isActive && NavMeshAgent.StoppedAtDestination() && !NavMeshAgent.isStopped)
        {
            UpdateDestNavAgentReached();
        }
    }

    private void UpdateDestNavAgentReached()
    {
        if (NavMeshAgent.StoppedAtDestination() && !NavMeshAgent.isStopped)
        {
            NavMeshAgent.isStopped = true;
            if (objectToRetrieveResourceFrom != null && objectToRetrieveResourceFrom.transform.position.IsSameXAndZ(NavMeshAgent.destination))
            {
                if(RetrieveResourceScript.CanRetrieveResource())
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
            else if (ObjectToBringResourceBackTo != null && ObjectToBringResourceBackTo.EntranceExit().IsSameXAndZ(NavMeshAgent.destination))
            {
                NavMeshAgent.isStopped = true;
                if(isCarryingResourceToBringBack)
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
    }   

    private void GoBackToGatherPoint()
    {
        NavMeshAgent.isStopped = false;
        NavMeshAgent.destination = ObjectToBringResourceBackTo.EntranceExit();
    }  

    private IEnumerator RetrievingResource()
    {
        isRetrievingResource = true;
        RetrieveResourceScript.StartRetrievingResource(ResourceCountMaterialHarvestedPerRun);
        yield return Wait4Seconds.Get(timeToRetrieveResourceInSeconds);
        isRetrievingResource = false;
        ResourceCarriedBack = RetrieveResourceScript.ResourceIsRetrieved();

        var waitTime = ResourceCarriedBack?.MaterialCount > 0 ? timeToWaitForRetrievalOfResourceInSeconds : 0; // bv als een andere villiager de resource voor je neus heeft weggekaapt; dan niet wachten
        MonoHelper.Instance.Do_CR(waitTime, () => BringResourceBack());
    }

    private void BringResourceBack()
    {
        NavMeshAgent.isStopped = false;
        NavMeshAgent.destination = ObjectToBringResourceBackTo.EntranceExit();
        NavMeshAgent.stoppingDistance = 0;
    }

    private IEnumerator DroppingResourceOff()
    {
        isDroppingResourceOff = true;
        yield return Wait4Seconds.Get(timeToDropOffResourceInSeconds);
        ObjectToBringResourceBackTo.GetComponent<ProduceResourceOrderBehaviour>()?.ProduceItems();

        isDroppingResourceOff = false;
        ResourceCarriedBack = null;
        Finished();
    }   

    private void TargetResourceToRetrieve()
    {
        RetrieveResourceScript = objectToRetrieveResourceFrom.GetComponent<IRetrieveResourceFromObject>(); // op dit moment optioneel (mag null blijven) -> gebruikt om resource te consumeren (bv boom laten verdwijnen als deze is omgehakt; evt voor stonequery langzaam laten depleten)

        // TODO nette fix --> voor nu nodig om de interfaces werkend te krijgen
        if(RetrieveResourceScript == null) { RetrieveResourceScript = new RetrieveResourceDummy(); }

        NavMeshAgent.SetDestination(objectToRetrieveResourceFrom.transform.position);
        NavMeshAgent.stoppingDistance = stopDistanceFromObjectToRetrieveResourceFrom;
        NavMeshAgent.isStopped = false;
    }

    public bool IsCarryingResource()
    {
        return isCarryingResourceToBringBack &&
            !NavMeshAgent.isStopped && 
            NavMeshAgent.destination.IsSameXAndZ(ObjectToBringResourceBackTo.EntranceExit()            
        );
    }

    public bool ShouldShowTool()
    {
        return
            !isActive || // nog niet actief script
            isRetrievingResource || // bezig met ophalen resources
            (
            objectToRetrieveResourceFrom != null && // gaat naar resource toe lopen
            NavMeshAgent.destination.IsSameXAndZ(objectToRetrieveResourceFrom.transform.position)
            );
    }
}