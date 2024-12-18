using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectDownSurfaceAndDestroy : MonoBehaviour
{
    public float movementSpeed = 1f;
    public float distanceBelowSurface = -1f;
    public float waitTimeInSecondsBeforeGoingDown = 0;
    public bool AlsoDestroyParent = true;

    private Vector3 endPosition;

    // Use this for initialization
    void Start()
    {
        this.endPosition = new Vector3(this.transform.position.x, this.transform.position.y + distanceBelowSurface, this.transform.position.z);
    }

    private bool CanGoDown;
    private float waitTimer;
    
    void Update()
    {
        if(CanGoDown)
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
        if (waitTimer >= waitTimeInSecondsBeforeGoingDown)
        {
            CanGoDown = true;
        }
    }

    private void UpdateGoDownAndDie()
    {
        
        if (transform.position != endPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, movementSpeed * Time.deltaTime);
        }
        else
        {
            if(AlsoDestroyParent && this.transform.parent != null)
            {
                GameObject.Destroy(this.transform.parent.gameObject);
            }
            else
            {
                GameObject.Destroy(this.gameObject);
            }
            
        }        
    }
}
