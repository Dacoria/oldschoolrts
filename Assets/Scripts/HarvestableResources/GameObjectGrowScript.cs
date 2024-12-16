using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectGrowScript : MonoBehaviour
{
    public float GrowSpeed = 10f;
    public float StartScale = 0.1f;
    public float EndScale = 1f;

    private Vector3 TargetSize;

    public bool HasReachedGrowthTarget()
    {
        return Vector3.SqrMagnitude(transform.localScale - TargetSize) < 0.01;
    }

    // Start is called before the first frame update
    void Start()
    {
        TargetSize = transform.localScale * EndScale;

        transform.localPosition = new Vector3(transform.localPosition.x, 0.04f, transform.localPosition.z);
        transform.localScale = transform.localScale * StartScale;
    }    

    // Update is called once per frame
    void Update()
    {
        if(!HasReachedGrowthTarget())
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, TargetSize, GrowSpeed * Time.deltaTime);
        }
    }
}
