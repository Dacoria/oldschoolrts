using System.Collections;
using UnityEngine;

public class TreeFallingScript : MonoBehaviour
{
    private bool isFalling;
    private GameObject TreeStanding;
    public float FallingSpeed = 60f;

    private void Awake()
    {
        for(var i  = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            if(child.name == "Tree")
            {
                TreeStanding = child;
            }           
        }
   }

    private void Start()
    {
        isFalling = true;
    }

    void Update()
    {
        if(isFalling)
        {
            if (transform.rotation.eulerAngles.x > 85)
            {
                TreeHasFallen();
                isFalling = false;
            }

            if (isFalling)
            {
                transform.Rotate(FallingSpeed * Time.deltaTime, 0, 0);
            }
        }        
    }

    private void TreeHasFallen()
    {
        if(TreeStanding != null)
        {
            StartCoroutine(DestroyTreeAfterXSeconds(1));
        }
    }

    private IEnumerator DestroyTreeAfterXSeconds(int secondToWait)
    {
        yield return Wait4Seconds.Get(secondToWait);
        if (TreeStanding != null)
        {
            Destroy(TreeStanding.transform.parent.gameObject);
        }
    }
}
