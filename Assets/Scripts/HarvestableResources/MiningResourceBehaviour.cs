using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MiningResourceBehaviour : MonoBehaviour, IProduceResourceOverTime, ILocationOfResource
{
    public MaterialResourceType MaterialResourceType;        

    public float TimeToProduceResourceInSeconds = 5;
    public float TimeToWaitAfterProducingInSeconds = 1.5f;
    public int MaxRangeForResources = 2;

    public int GetMaxRangeForResource() => MaxRangeForResources;
    public float GetTimeToProduceResourceInSeconds() => TimeToProduceResourceInSeconds;
    public float GetTimeToWaitAfterProducingInSeconds() => TimeToWaitAfterProducingInSeconds;
    public RangeType GetRangeTypeToFindResource() => RangeType.BoxColliderExpand;

    public void StartProducing(ItemProduceSetting itemProduceSetting) { }
    public bool CanProduceResource() => MineResource(consumeResource: false);    
    public void FinishProducing(ItemProduceSetting itemProduceSetting) => MineResource(consumeResource: true);

    private bool MineResource(bool consumeResource)
    {
        var resourceToMine = GetResourceToRetrieve();
        if(resourceToMine != null)
        {        
            if (consumeResource)
            {
               var harvestScript = resourceToMine.GetComponent<HarvestableMaterialScript>();

                harvestScript.StartRetrievingResource();
                // evt voor animatie/overgang oid in 2 stappen
                harvestScript.ResourceIsRetrieved();
            }

            return true;
        }

        return false;
    }

    private List<HarvestableMaterialScript> GetOptionsToMine(MaterialResourceType materialToFind, Bounds areaToFindMaterial)
    {
        var result = new List<HarvestableMaterialScript>();

        var resourceGos = GameObject.FindGameObjectsWithTag(StaticHelper.TAG_RESOURCE);
        foreach (var resourceGo in resourceGos)
        {
            var harvestResourceScript = resourceGo.GetComponent<HarvestableMaterialScript>();
            if (harvestResourceScript != null &&
                harvestResourceScript.CanRetrieveResource() &&
                harvestResourceScript.MaterialType == materialToFind &&
                areaToFindMaterial.Intersects(resourceGo.GetComponent<Collider>().bounds))
            {   
                result.Add(harvestResourceScript);                
            }
        }

        return result;
    }

    public GameObject GetResourceToRetrieve()
    {
        var mineResourcesBounds = (transform.parent.GetComponent<Collider>().bounds).Copy();
        mineResourcesBounds.Expand(GetMaxRangeForResource()); // voor nu: Scope van gebouw + 1 veld als mining area

        var optionsToMineResourceScripts = GetOptionsToMine(MaterialResourceType, mineResourcesBounds);
        if (optionsToMineResourceScripts.Count > 0)
        {
            // voor nu: Pak altijd de resource met de meeste materialen
            var bestOptionResourceScript = optionsToMineResourceScripts.OrderByDescending(x => x.MaterialCount).First();
            return bestOptionResourceScript.gameObject;            
        }

        return null;
    }    
}
