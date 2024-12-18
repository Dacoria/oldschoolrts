using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlantResourceBehaviour : MonoBehaviourCI, IVillagerWorkAction
{
    [ComponentInject] private NavMeshAgent NavMeshAgent;
    private ILocationOfNewResource LocationOfNewResourceScript;

    private GameObject IniatiatedPlantedNewResource;
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
        LocationOfNewResourceScript = ObjectToBringResourceBackTo.GetComponent<ILocationOfNewResource>(); // waar kan/mag de nieuwe resource gemaakt & geplaatst worden? + teruggeven prefab nieuwe GO
        if (LocationOfNewResourceScript == null) { throw new System.Exception("LocationOfNewResourceScript -> Nodig voor bepalen objectToRetrieveResourceFrom"); }
    }

    public bool CanDoAction()
    {
        if(PlantNewResourceOnFarmField)
        {
            // voor bv farmfields
            return LocationOfNewResourceScript.GetGameObjectForNewResource() != null;
        }

        return !LocationOfNewResourceScript.GetCoordinatesForNewResource().IsAlmostEmptyVector();        
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
            if (IniatiatedPlantedNewResource != null && IniatiatedPlantedNewResource.transform.position.IsSameXAndZ(NavMeshAgent.destination))
            {                
                StartCoroutine(CreateNewResource());
            }
            else if (ObjectToBringResourceBackTo != null && ObjectToBringResourceBackTo.EntranceExit().IsSameXAndZ(NavMeshAgent.destination))
            {
                NavMeshAgent.isStopped = true;
                Finished();                
            }
            else
            {
                // Wel bestemming bereikt; maar licht blijkbaar niks --> terug naar basis voor fallback
                Debug.Log("Bestemming bereikt zonder actie --> zou niet moeten");
                GoBackToGatherPoint();
            }
        }
    }

    private IEnumerator CreateNewResource()
    {
        isCreatingNewResource = true;
        yield return MonoHelper.Instance.GetCachedWaitForSeconds(timeToCreateNewResourceInSeconds);
        isCreatingNewResource = false;

        IniatiatedPlantedNewResource.SetActive(true); // bv een boom -> maakt zichtbaar
        IniatiatedPlantedNewResource = null; // wordt evt later gekoppeld als resource om te harvesten --> nu resetten

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
        
        if(PlantNewResourceOnFarmField)
        {
            // TODO expliciet casten naar object ongewenst; moet mooier/netter kunnen
            var farmField = LocationOfNewResourceScript.GetGameObjectForNewResource();
            var farmFieldScript = farmField.GetComponent<FarmFieldScript>();

            IniatiatedPlantedNewResource = Instantiate(ObjectToPlantPrefab, farmField.transform.position, Quaternion.identity);
            IniatiatedPlantedNewResource.SetActive(false); // wordt active gemaakt bij het planten van de resource

            farmFieldScript.ObjectGrownOnField = IniatiatedPlantedNewResource;
            IniatiatedPlantedNewResource.transform.parent = farmField.transform;
        }
        else
        {
            var newLocNewResource = LocationOfNewResourceScript.GetCoordinatesForNewResource();
            if(newLocNewResource.IsAlmostEmptyVector())
            {
                return false;
            }

            IniatiatedPlantedNewResource = Instantiate(ObjectToPlantPrefab, newLocNewResource, Quaternion.identity);
            IniatiatedPlantedNewResource.SetActive(false); // wordt active gemaakt bij het planten van de resource
        }

        NavMeshAgent.SetDestination(IniatiatedPlantedNewResource.transform.position);
        NavMeshAgent.stoppingDistance = stopDistanceFromObjectToCreateNewResource;
        NavMeshAgent.isStopped = false;

        return true;
    }

    public bool IsCarryingResource() => IniatiatedPlantedNewResource != null && (!NavMeshAgent.isStopped && NavMeshAgent.destination.IsSameXAndZ(IniatiatedPlantedNewResource.transform.position));
}