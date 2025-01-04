using Assets.Army;
using UnityEngine;

public class RangedHomingMissileBehaviour : MonoBehaviour
{
    private GameObject Target;
    public GameObject GoOnDestroyOptionalPrefab;
    public float Speed;
    public Offence Offence { get; set; }

    void Update()
    {
        if (Target == null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            float step = Speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, step);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == Target)
        {
            HandleAttack.Handle(this.Offence, Target.gameObject);
            if(GoOnDestroyOptionalPrefab != null)
            {
                Instantiate(GoOnDestroyOptionalPrefab, collider.transform.position, Quaternion.identity);
            }
            Destroy(this.gameObject);
        }
    }

    public void SetTarget(GameObject target)
    {
        Target = target;
    }    
}