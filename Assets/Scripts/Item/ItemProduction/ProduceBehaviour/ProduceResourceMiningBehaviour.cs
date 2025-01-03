using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class ProduceResourceMiningBehaviour : MonoBehaviourCI, ILocationOfResource, ICardOneProdBuilding
{
    public RangeType GetRangeTypeToFindResource() => RangeType.BoxColliderExpand;
    public int GetMaxRangeForResource() => 2;

    [ComponentInject] private BuildingBehaviour buildingBehaviour;
    private RefillBehaviour refillBehaviour;
    private ConsumeRefillItemsBehaviour consumeRefillItemsBehaviour;
    private ProduceCRBehaviour produceCRBehaviour;
    private HandleProduceResourceOrderBehaviour handleProduceResourceOrderBehaviour;

    private new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour), typeof(ProduceCRBehaviour), typeof(HandleProduceResourceOrderBehaviour) });

        refillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItemsBehaviour = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
        produceCRBehaviour = gameObject.AddComponent<ProduceCRBehaviour>();
        handleProduceResourceOrderBehaviour = gameObject.AddComponent<HandleProduceResourceOrderBehaviour>();
    }

    private void Start()
    {
        StartCoroutine(TryToProduceOverXSeconds());
    }

    private IEnumerator TryToProduceOverXSeconds()
    {
        var itemToProduceSettings = buildingBehaviour.BuildingType.GetItemProduceSettings().FirstOrDefault(x => CanProduce(x));
        if (itemToProduceSettings == null)
        {
            yield return Wait4Seconds.Get(0.1f); // kan nog niet produceren, doe check opnieuw na x secondes
            StartCoroutine(TryToProduceOverXSeconds());
        }
        else
        {
            produceCRBehaviour.ProduceOverTime(new ProduceSetup(
                itemToProduceSettings.ItemsToProduce,
                handleProduceResourceOrderBehaviour,
                produceCallback: () => MineResource(consumeResource: true),
                waitAfterProduceCallback: () => StartCoroutine(TryToProduceOverXSeconds())));
        }
    }

    private bool CanProduce(ItemProduceSetting itemProduceSetting)
    {
        if (ItemProdHelper.HasReachedRscProductionBuffer(itemProduceSetting.ItemsToProduce, handleProduceResourceOrderBehaviour))
            return false;

        if(!MineResource(consumeResource: false))
            return false;

        if (!produceCRBehaviour.IsReadyForNextProduction())
            return false;

        return true;
    }

    private bool MineResource(bool consumeResource)
    {
        var resourceToMine = GetResourceToRetrieve();
        if(resourceToMine != null)
        {        
            if (consumeResource)
            {
                // elke plek heeft een script -> vandaar elke keer component ophalen
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

    public GameObject GetGameObject() => gameObject;
    public List<TypeProcessing> GetCurrentTypesProcessed() => produceCRBehaviour.CurrentTypesProcessed;
}