using UnityEngine;

public class GameObjectGrowScript : MonoBehaviour
{
    public float GrowSpeed = 10f;
    public float StartScale = 0.1f;
    public float EndScale = 1f;

    private Vector3 targetSize;

    public bool HasReachedGrowthTarget() => Vector3.SqrMagnitude(transform.localScale - targetSize) < 0.01;

    void Start()
    {
        targetSize = transform.localScale * EndScale;

        transform.localPosition = new Vector3(transform.localPosition.x, 0.04f, transform.localPosition.z);
        transform.localScale = transform.localScale * StartScale;
    }    

    void Update()
    {
        if(!HasReachedGrowthTarget())
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, GrowSpeed * Time.deltaTime);
        }
    }
}