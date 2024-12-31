using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProduceResourceMiningBehaviour : ProduceResourceAbstract, ILocationOfResource, IProduceResourceOverTimeDurations
{
    public float TimeToProduceResourceInSeconds => 10;
    public float TimeToWaitAfterProducingInSeconds => 2;
    public RangeType GetRangeTypeToFindResource() => RangeType.BoxColliderExpand;
    public int GetMaxRangeForResource() => 2;

    [HideInInspector] public HandleProduceResourceOrderOverTimeBehaviour HandleProduceResourceOrderOverTimeBehaviour;

    new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> { typeof(HandleProduceResourceOrderOverTimeBehaviour) });

        HandleProduceResourceOrderOverTimeBehaviour = gameObject.AddComponent<HandleProduceResourceOrderOverTimeBehaviour>();
        HandleProduceResourceOrderOverTimeBehaviour.FinishedProducingAction += OnFinishedProducingAction;
    }

    private void OnDestroy() => HandleProduceResourceOrderOverTimeBehaviour.FinishedProducingAction -= OnFinishedProducingAction;

    private void OnFinishedProducingAction(List<ItemOutput> list) => MineResource(consumeResource: true);

    protected override bool CanProduceResource(ItemProduceSetting itemProduceSetting) => base.CanProduceResource(itemProduceSetting) && MineResource(consumeResource: false);

    private bool MineResource(bool consumeResource)
    {
        var resourceToMine = GetResourceToRetrieve();
        if(resourceToMine != null)
        {        
            if (consumeResource)
            {
                // TODO WTF? FIX DIT
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

        var resourceGos = GameObject.FindGameObjectsWithTag(Constants.TAG_RESOURCE);
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

        var optionsToMineResourceScripts = GetOptionsToMine(buildingBehaviour.BuildingType.GetMaterialResourceType(), mineResourcesBounds);
        if (optionsToMineResourceScripts.Count > 0)
        {
            // voor nu: Pak altijd de resource met de meeste materialen
            var bestOptionResourceScript = optionsToMineResourceScripts.OrderByDescending(x => x.MaterialCount).First();
            return bestOptionResourceScript.gameObject;            
        }

        return null;
    }
}