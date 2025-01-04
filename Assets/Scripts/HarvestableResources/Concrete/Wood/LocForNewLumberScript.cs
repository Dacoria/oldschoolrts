using UnityEngine;
using System.Linq;

public class LocForNewLumberScript : MonoBehaviour, ILocForNewResource, ILocationOfResource
{
    public int MinRangeForTrees = 1;
    public int MaxRangeForTrees;

    public int GetMaxRangeForResource() => MaxRangeForTrees;
    public RangeType GetRangeTypeToFindResource() => RangeType.Circle;
    
    public LocForRsc GetLocationForNewResource()
    {
        // 20x random positie proberen, anders 0 - vector teruggeven
        for (var i = 0; i < 60; i++)
        {
            var randomVector2 = VectorExtensions.Random(MinRangeForTrees, MaxRangeForTrees);

            var newLoc = transform.position + new Vector3(randomVector2.x, 0, randomVector2.y);
            var distance = Vector3.Distance(newLoc, transform.position);
            if (distance < MaxRangeForTrees && CanPlantTreeOnLoc(newLoc))
            {
                //Debug.Log("GetCoordinatesNewTree: " + x + ", " +  z);
                return new LocForRsc { V3 = newLoc };
            }
        }

        // geen geschikte locatie gevonden
        return null;
    }

    private bool CanPlantTreeOnLoc(Vector3 position)
    {
        var intersecting = Physics.OverlapSphere(position, 1.1f).ToList();
        return !intersecting.Any(x => IsCollidingObjectToPlantTree(x));
    }

    private bool IsCollidingObjectToPlantTree(Collider collider)
    {
        return
            collider.transform.gameObject.layer != Constants.LAYER_TERRAIN &&
            collider.transform.tag != Constants.TAG_UNIT &&           
            collider.transform.tag != Constants.TAG_ENTRANCE_EXIT;
    }

    public GameObject GetResourceToRetrieve()
    {
        var resources = GameObject.FindGameObjectsWithTag(Constants.TAG_RESOURCE);
        GameObject closestTree = null;
        var closestDistance = 9999999f;

        foreach (var resource in resources)
        {
            var harvestableMaterial = resource.GetComponent<HarvestableMaterialScript>();
            if (harvestableMaterial != null && harvestableMaterial.MaterialType == MaterialResourceType.WOODLOG)
            {
                var distance = Vector3.Distance(resource.transform.position, transform.position);
                if (distance < MaxRangeForTrees && distance < closestDistance)
                {
                    var growScript = resource.GetComponent<GameObjectGrowScript>();
                    if (!growScript.enabled || growScript.HasReachedGrowthTarget())
                    {
                        var retrieveResourceScript = resource.GetComponent<IRetrieveResourceFromObject>();
                        if (retrieveResourceScript != null && retrieveResourceScript.CanRetrieveResource())
                        {
                            closestTree = resource;
                            closestDistance = distance;
                        }
                    }
                }
            }   
        }

        return closestTree;
    }
}