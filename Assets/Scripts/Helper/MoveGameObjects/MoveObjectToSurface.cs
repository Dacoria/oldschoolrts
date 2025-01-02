using UnityEngine;

public class MoveObjectToSurface : MonoBehaviourCI
{
    public float DistanceSpawnedBelowSurface;

    private float MovementSpeed;
    private float TimeInSeconds;

    private bool EndPositionReached = false;
    private Vector3 EndPosition;

    [ComponentInject] private BuildingBehaviour buildingBehaviour;

    public void Start()
    {
        if (buildingBehaviour != null)
        {
            TimeInSeconds = buildingBehaviour.BuildingType.GetBuildDurationSettings().TimeToBuildRealInSeconds;
            if (buildingBehaviour.CurrentBuildStatus == BuildStatus.COMPLETED_BUILDING)
            {
                enabled = false;
                return; // gebouw is al af --> dit script is overbodig
            }
        }
        else
        {
            TimeInSeconds = 1; // default;
        }

        var orig = this.transform.position;
        this.transform.position = new Vector3(this.transform.position.x, DistanceSpawnedBelowSurface, this.transform.position.z);
        this.EndPosition = orig;  //new Vector3(this.transform.position.x, 0.11f, this.transform.position.z); 

        MovementSpeed = (this.EndPosition.y - this.transform.position.y) / TimeInSeconds;        
    }
  
    void Update()
    {
        if(buildingBehaviour != null && buildingBehaviour.CurrentBuildStatus == BuildStatus.COMPLETED_BUILDING)
        {
            Destroy(this);
        }
        else if(!EndPositionReached)
        {
            if (transform.position != EndPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, EndPosition, MovementSpeed * Time.deltaTime);
            }
            else
            {
                Destroy(this);
            }
        }
    }
}