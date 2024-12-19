using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class CheckCollision : MonoBehaviour
{
    private BoxCollider boxCollider;
    private Vector3 sizeChangeInCollisionCheck = Vector3.zero;
    private Vector3 initialBoxColliderSize;

    public void Init(int? inclLayers = null, int? exclLayers = null, Vector3? v3SizeChangeWhenEnabled = null)
    {
        boxCollider = GetComponent<BoxCollider>(); // wordt pas later aangemaakt -> daarom geen CI gebruiken
        boxCollider.isTrigger = false;
        initialBoxColliderSize = boxCollider.size;

        if (inclLayers.HasValue)
        {
            boxCollider.includeLayers = inclLayers.Value;
        }
        if (exclLayers.HasValue)
        {
            boxCollider.excludeLayers = exclLayers.Value;
        }
        if (v3SizeChangeWhenEnabled.HasValue)
        {
            boxCollider.size += v3SizeChangeWhenEnabled.Value;
        }
    }    

    private void OnDestroy()
    {
        // back to defaults
        StopAllCoroutines();
        boxCollider.isTrigger = true;
        boxCollider.size = initialBoxColliderSize;
        boxCollider.excludeLayers = 1 << Constants.LAYER_TERRAIN;
    }


    private int notUpdatedCollidingCounter;
    private List<Collision> CollisionsOnLocation = new List<Collision>();
    [HideInInspector][ComponentInject(Required.OPTIONAL)] public CheckResourceCollisionForBuilding CheckResourceCollisionForBuilding;

    // Deze waarde gaat het om; deze moet correct zijn
    public bool IsColliding;

    protected class Collision
    {
        public bool IsUnitCollision;
        public bool IsResourceCollision;
        public bool IsRelevantResourceCollision;
        public bool IsRoadCollision;
        public bool IsFarmFieldCollision;
        public GameObject GameObject;
    }

    private Vector3 LastPositionCheck;

    private void Update()
    {
        if(LastPositionCheck != transform.position)
        {
            LastPositionCheck = transform.position;
            notUpdatedCollidingCounter = 0;
            CollisionsOnLocation.Clear();
        }

        notUpdatedCollidingCounter++;

        var framesToCheckAgain = IsColliding ? 50 : 10; // sneller collision checken; die wil je sneller goed hebben

        if(notUpdatedCollidingCounter > framesToCheckAgain)
        {
            UpdateCollisionStatus();
            notUpdatedCollidingCounter = 0;
        }
    }

    void OnTriggerStay(Collider other)
    {
        var isRelevantResourceCollision = CheckResourceCollisionForBuilding != null && CheckResourceCollisionForBuilding.IsCollidingWithRequiredResource(other);
        var collision = new Collision
        {
            IsRelevantResourceCollision = isRelevantResourceCollision,
            IsResourceCollision = other.transform.tag == Constants.TAG_RESOURCE,
            IsUnitCollision = other.transform.tag == Constants.TAG_UNIT,
            IsRoadCollision = other.gameObject.IsRoad(),
            IsFarmFieldCollision = other.gameObject.IsFarmField(),
            GameObject = other.transform.gameObject
        };

        CollisionsOnLocation.Add(collision);        
    }

    private DateTime? notCollidingDateTime;

    private IEnumerator UpdateCollisionStatusAfterDelay(float waitTimeInSeconds, DateTime? checkNotCollidingDateTime)
    {
        yield return Wait4Seconds.Get(waitTimeInSeconds);
        UpdateCollisionStatus(checkNotCollidingDateTime);
    }

    private void UpdateCollisionStatus(DateTime? checkNotCollidingDateTime = null)
    {
        var go = gameObject;
        var relevantCollisions = CollisionsOnLocation.Where(x => !x.IsUnitCollision).ToList();

        bool isCollidingInThisCheck;

        if (CheckResourceCollisionForBuilding != null)
        {
            isCollidingInThisCheck =
                relevantCollisions.All(x => !x.IsRelevantResourceCollision) ||
                relevantCollisions
                .Where(x => !x.IsRelevantResourceCollision)
                .Any();
        }
        else
        {
            isCollidingInThisCheck = relevantCollisions.Any();
        }

        UpdateCollisionStatusWithCheckResult(isCollidingInThisCheck, checkNotCollidingDateTime);
    }

    private void UpdateCollisionStatusWithCheckResult(bool isCollidingInThisCheck, DateTime? checkNotCollidingDateTime)
    {
        // Check + huidige status zeggen: collide. Consistent; alleen resetten
        if (isCollidingInThisCheck && IsColliding)
        {
            notCollidingDateTime = null;
            return;
        }

        // Check + huidige status zeggen: geen collide. Consistent; alleen resetten
        if (!isCollidingInThisCheck && !IsColliding)
        {
            notCollidingDateTime = null;
            return;
        }

        // Check zegt collide + huidige status zegt niet -> update huidige status
        if (isCollidingInThisCheck && !IsColliding)
        {
            IsColliding = true;
            notCollidingDateTime = null;
            return;
        }

        // Check zegt GEEN collide + huidige status zegt wel -> Dan wachten tot het zo blijft; zo ja updaten. Manier: Via timestamp dat je zolang hebt gewacht
        // Waarom zo? Voorkomt flikkeren + veilig qua bouwen

        if (!isCollidingInThisCheck && IsColliding)
        {
            if (!notCollidingDateTime.HasValue)
            {
                // 1e keer -> update timestamp + retry na X ms
                var now = DateTime.Now;
                notCollidingDateTime = now;
                StartCoroutine(UpdateCollisionStatusAfterDelay(waitTimeInSeconds: 0.15f, now));
            }
            else
            {
                // 2e keer -> retry na X ms -> nog steeds geen collide in check -> Update status!
                // (waarom timestamp? in de wachttijd kan je opnieuw een collide krijgen, en daarna weer eruit gaan)
                if (checkNotCollidingDateTime.HasValue &&
                notCollidingDateTime.HasValue &&
                checkNotCollidingDateTime.Value == notCollidingDateTime.Value)
                {
                    IsColliding = false;
                    notCollidingDateTime = null;
                }
            }
        }
    }
}