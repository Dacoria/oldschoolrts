using UnityEngine;

public class MoveObjectToSurface : MonoBehaviour
{
    public float DistanceSpawnedBelowSurface;

    private float MovementSpeed;
    private float TimeInSeconds;

    private bool EndPositionReached = false;
    private Vector3 EndPosition;

    [ComponentInject] private BuildingBehaviour BuildingBehaviour;

    void Awake()
    {
        this.ComponentInject();
    }

    public void Start()
    {
        if (BuildingBehaviour != null)
        {
            TimeInSeconds = BuildingBehaviour.TimeToBuildRealInSeconds;
            if (BuildingBehaviour.CurrentBuildStatus == BuildStatus.COMPLETED_BUILDING)
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
        if(BuildingBehaviour != null && BuildingBehaviour.CurrentBuildStatus == BuildStatus.COMPLETED_BUILDING)
        {
            enabled = false;
        }
        else if(!EndPositionReached)
        {
            if (transform.position != EndPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, EndPosition, MovementSpeed * Time.deltaTime);
            }
            else
            {
                enabled = false; // disable script (onnodige updates voorkomen)                
            }
        }
    }
}
