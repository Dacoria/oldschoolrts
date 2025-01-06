using System.Collections;
using UnityEngine;

public class TreeFallingScript : MonoBehaviour
{
    private bool isFalling;
    private GameObject treeStanding;
    public float FallingSpeed = 60f;

    private void Awake()
    {
        for(var i  = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            if(child.name == "Tree")
            {
                treeStanding = child;
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
        if(treeStanding != null)
        {
            StartCoroutine(DestroyTreeAfterXSeconds(1));
        }
    }

    private IEnumerator DestroyTreeAfterXSeconds(int secondToWait)
    {
        yield return Wait4Seconds.Get(secondToWait);
        if (treeStanding != null)
        {
            Destroy(treeStanding.transform.parent.gameObject);
        }
    }
}