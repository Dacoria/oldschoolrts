using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProduceMiningResourceBehaviour : ProduceResourceAbstract, ILocationOfResource, IProduceResourceOverTimeDurations
{
    public MaterialResourceType MaterialResourceType;
    public int MaxBuffer = 6;
    public int ProducedPerProdCycle = 1;

    public override List<ItemProduceSetting> GetResourcesToProduce()
    {
        var itemToProduce = new ItemOutput
        {
            MaxBuffer = MaxBuffer,
            ProducedPerProdCycle = ProducedPerProdCycle,
            ItemType = Enum.Parse<ItemType>(MaterialResourceType.ToString())
        };
        return new List<ItemProduceSetting> { new ItemProduceSetting { ItemsToProduce = new List<ItemOutput> { itemToProduce } } };
    }


    public int MaxRangeForResources = 2;
    public float ProduceTimeInSeconds;
    public float WaitAfterProduceTimeInSeconds;

    public float TimeToProduceResourceInSeconds => ProduceTimeInSeconds;
    public float TimeToWaitAfterProducingInSeconds => WaitAfterProduceTimeInSeconds;
    public RangeType GetRangeTypeToFindResource() => RangeType.BoxColliderExpand;
    public int GetMaxRangeForResource() => MaxRangeForResources;

    [HideInInspector] public HandleAutoProduceResourceOrderOverTimeBehaviour HandleAutoProduceResourceOrderOverTimeBehaviour;

    private new void Start()
    {
        base.Start();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> { typeof(HandleAutoProduceResourceOrderOverTimeBehaviour) });

        HandleAutoProduceResourceOrderOverTimeBehaviour = gameObject.AddComponent<HandleAutoProduceResourceOrderOverTimeBehaviour>();
        HandleAutoProduceResourceOrderOverTimeBehaviour.HandleProduceResourceOrderOverTimeBehaviour.FinishedProducingAction += OnFinishedProducingAction;
    }
    private void OnDestroy() => HandleAutoProduceResourceOrderOverTimeBehaviour.HandleProduceResourceOrderOverTimeBehaviour.FinishedProducingAction -= OnFinishedProducingAction;


    private void OnFinishedProducingAction() => MineResource(consumeResource: true);

    protected override bool CanProduceResource(ItemProduceSetting itemProduceSetting) => base.CanProduceResource(itemProduceSetting) && MineResource(consumeResource: false);

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