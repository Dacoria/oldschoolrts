using System;
using System.Collections.Generic;
using UnityEngine;

public class FindResourceScript : MonoBehaviour, ILocationOfResource
{
    public int MaxRangeForResource = 20;
    public MaterialResourceType MaterialResourceType;

    public int GetMaxRangeForResource() => MaxRangeForResource;
    public RangeType GetRangeTypeToFindResource() => RangeType.Circle;

    public GameObject GetResourceToRetrieve()
    {
        var goResources = GetResourceGameObjects(MaterialResourceType);
        GameObject clostestResource = null;
        var closestDistance = 9999999f;

        foreach (var goResource in goResources)
        {
            var distance = Vector3.Distance(goResource.transform.position, transform.position);

            if (distance < MaxRangeForResource && distance < closestDistance)
            {               
                clostestResource = goResource;
                closestDistance = distance;                              
            }
        }

        return clostestResource;
    }

    private List<GameObject> GetResourceGameObjects(MaterialResourceType materialResourceType)
    {
        var result = new List<GameObject>(); 

        var resourceGameObjects = GameObject.FindGameObjectsWithTag(Constants.TAG_RESOURCE);
        foreach (var resource in resourceGameObjects)
        {
            var harvestableScript = resource.GetComponent<HarvestableMaterialScript>();
            if(harvestableScript != null && harvestableScript.MaterialType == materialResourceType && harvestableScript.CanRetrieveResource())
            {
                result.Add(resource);
            }
            if(materialResourceType == MaterialResourceType.WILDANIMAL)
            {
                var deerDyingScript = resource.GetComponent<DeerDyingScript>();
                if (deerDyingScript != null && deerDyingScript.CanRetrieveResource())
                {
                    result.Add(resource);
                }
            }            
        }

        return result;
    }
}
