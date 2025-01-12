using UnityEngine;

public class MoveObjectToSurface : MonoBehaviourCI
{
    private float movementSpeed;
    private float timeInSeconds;
    private bool endPositionReached = false;
    private Vector3 endPosition;

    [ComponentInject] private BuildingBehaviour buildingBehaviour;
    [ComponentInject] private BoxCollider boxCollider;

    public float distanceSpawnedBelowSurface => -boxCollider.size.y;

    public void Start()
    {
        if (buildingBehaviour != null)
        {
            timeInSeconds = buildingBehaviour.BuildingType.GetBuildDurationSettings().TimeToBuildRealInSeconds;
            if (buildingBehaviour.CurrentBuildStatus == BuildStatus.COMPLETED_BUILDING)
            {
                enabled = false;
                return; // gebouw is al af --> dit script is overbodig
            }
        }
        else
        {
            timeInSeconds = 1; // default;
        }

        var orig = this.transform.position;
        this.transform.position = new Vector3(this.transform.position.x, distanceSpawnedBelowSurface, this.transform.position.z);
        this.endPosition = orig;  //new Vector3(this.transform.position.x, 0.11f, this.transform.position.z); 

        movementSpeed = (this.endPosition.y - this.transform.position.y) / timeInSeconds;        
    }
  
    void Update()
    {
        if(buildingBehaviour != null && buildingBehaviour.CurrentBuildStatus == BuildStatus.COMPLETED_BUILDING)
        {
            Destroy(this);
        }
        else if(!endPositionReached)
        {
            if (transform.position != endPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, endPosition, movementSpeed * Time.deltaTime);
            }
            else
            {
                Destroy(this);
            }
        }
    }
}