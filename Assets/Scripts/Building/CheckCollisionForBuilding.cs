using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollisionForBuilding : MonoBehaviour
{
    private int notUpdatedCollidingCounter;

    private List<Collision> CollisionsOnLocation = new List<Collision>();

    protected class Collision
    {
        public bool IsUnitCollision;
        public bool IsResourceCollision;
        public bool IsRelevantResourceCollision;
        public GameObject GameObject;
    }


    [ComponentInject(Required.OPTIONAL)] public CheckResourceCollisionForBuilding CheckResourceCollisionForBuilding;

    public void Awake()
    {
        this.ComponentInject();
    }

    private Vector3 LastPositionCheck;

    private void Update()
    {
        // wacht even met updaten om flikkeren te voorkomen. Andere positie = opnieuw wachten

        if(LastPositionCheck != transform.position)
        {
            LastPositionCheck = transform.position;
            notUpdatedCollidingCounter = 0;
            CollisionsOnLocation.Clear();
        }

        notUpdatedCollidingCounter++;
        if(notUpdatedCollidingCounter > 10)
        {
            UpdateCollisionStatus();
        }
    }


    void OnTriggerStay(Collider other)
    {
        if (other.transform.gameObject.layer != StaticHelper.LAYER_TERRAIN)
        {
            var isRelevantResourceCollision = CheckResourceCollisionForBuilding != null && CheckResourceCollisionForBuilding.IsCollidingWithRequiredResource(other);
            var collision = new Collision
            {
                IsRelevantResourceCollision = isRelevantResourceCollision,
                IsResourceCollision = other.transform.tag == StaticHelper.TAG_RESOURCE,
                IsUnitCollision = other.transform.tag == StaticHelper.TAG_UNIT,
                GameObject = other.transform.gameObject
            };

            CollisionsOnLocation.Add(collision);
        }
    }

    private bool isColliding;
    private void UpdateCollisionStatus()
    {
        var go = gameObject;
        var relevantCollisions = CollisionsOnLocation.Where(x => !x.IsUnitCollision).ToList();

        if (CheckResourceCollisionForBuilding != null)
        {
            isColliding =
                relevantCollisions.All(x => !x.IsRelevantResourceCollision) || 

                relevantCollisions
                .Where(x => !x.IsRelevantResourceCollision)
                .Any();
        }
        else
        {
            isColliding = relevantCollisions.Any();
        }
    }

    public bool IsColliding() => isColliding;
}
