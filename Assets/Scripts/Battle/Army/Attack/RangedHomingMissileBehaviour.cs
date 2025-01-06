using Assets.Army;
using UnityEngine;

public class RangedHomingMissileBehaviour : MonoBehaviour
{
    private GameObject target;
    public GameObject GoOnDestroyOptionalPrefab;
    public float Speed;
    public Offence Offence { get; set; }

    void Update()
    {
        if (target == null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            float step = Speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == target)
        {
            HandleAttack.Handle(this.Offence, target.gameObject);
            if(GoOnDestroyOptionalPrefab != null)
            {
                Instantiate(GoOnDestroyOptionalPrefab, collider.transform.position, Quaternion.identity);
            }
            Destroy(this.gameObject);
        }
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }    
}