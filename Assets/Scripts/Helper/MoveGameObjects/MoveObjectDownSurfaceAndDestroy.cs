using UnityEngine;

public class MoveObjectDownSurfaceAndDestroy : MonoBehaviour
{
    public float MovementSpeed = 1f;
    public float DistanceBelowSurface = -1f;
    public float WaitTimeInSecondsBeforeGoingDown = 0;
    public bool AlsoDestroyParent = true;

    private Vector3 endPosition;

    // Use this for initialization
    void Start()
    {
        this.endPosition = new Vector3(this.transform.position.x, this.transform.position.y + DistanceBelowSurface, this.transform.position.z);
    }

    private bool canGoDown;
    private float waitTimer;
    
    void Update()
    {
        if(canGoDown)
        {
            UpdateGoDownAndDie();
        }
        else
        {
            WaitToGoDown();
        }
    }

    private void WaitToGoDown()
    {
        waitTimer += Time.deltaTime;
        if (waitTimer >= WaitTimeInSecondsBeforeGoingDown)
        {
            canGoDown = true;
        }
    }

    private void UpdateGoDownAndDie()
    {
        
        if (transform.position != endPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, MovementSpeed * Time.deltaTime);
        }
        else
        {
            if(AlsoDestroyParent && this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }            
        }        
    }
}