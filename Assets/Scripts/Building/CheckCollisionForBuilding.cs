using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckCollisionForBuilding : MonoBehaviourCI
{
    private int notUpdatedCollidingCounter;

    private List<Collision> CollisionsOnLocation = new List<Collision>();

    protected class Collision
    {
        public bool IsUnitCollision;
        public bool IsResourceCollision;
        public bool IsRelevantResourceCollision;
        public bool IsRoad;
        public bool IsFarmField;
        public GameObject GameObject;
    }


    [ComponentInject(Required.OPTIONAL)] public CheckResourceCollisionForBuilding CheckResourceCollisionForBuilding;
    [ComponentInject] private BoxCollider boxCollider;
    [ComponentInject] private BuildingBehaviour buildingBehaviour;

    private Vector3 LastPositionCheck;

    private void OnEnable()
    {
        if (GetComponentInChildren<GhostBuildingBehaviour>() != null && !this.gameObject.IsFarmField() && !this.gameObject.IsRoad())
        {
            boxCollider.size += new Vector3(1, 1, 1);
        }
    }

    private void OnDisable()
    {
        if (GetComponentInChildren<GhostBuildingBehaviour>() != null && !this.gameObject.IsFarmField() && !this.gameObject.IsRoad())
        {
            boxCollider.size -= new Vector3(1, 1, 1);
        }
    }

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
        if (other.transform.gameObject.layer != Constants.LAYER_TERRAIN)
        {
            var isRelevantResourceCollision = CheckResourceCollisionForBuilding != null && CheckResourceCollisionForBuilding.IsCollidingWithRequiredResource(other);
            var collision = new Collision
            {
                IsRelevantResourceCollision = isRelevantResourceCollision,
                IsResourceCollision = other.transform.tag == Constants.TAG_RESOURCE,
                IsUnitCollision = other.transform.tag == Constants.TAG_UNIT,
                IsRoad = other.gameObject.IsRoad(),
                IsFarmField = other.gameObject.IsFarmField(),
                GameObject = other.transform.gameObject
            };

            CollisionsOnLocation.Add(collision);
        }
    }

    private bool isColliding;
    private void UpdateCollisionStatus()
    {
        var go = gameObject;
        var relevantCollisions = CollisionsOnLocation.Where(x => !x.IsUnitCollision && !x.IsRoad && !x.IsFarmField).ToList();

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