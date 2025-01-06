using System.Linq;
using UnityEngine;
using System;

public class SerfResourceCarryingBehaviour : MonoBehaviour
{
    private GameObject resourceBeingCarriedGo;
    private GameObject resourceBubbleToPickupGo;

    public void InitiateCarryingResource(ItemType itemType)
    {
        if (resourceBubbleToPickupGo != null)
        {
            Destroy(resourceBubbleToPickupGo);
        }

        if (resourceBeingCarriedGo != null) { throw new Exception("zou al destroyed moeten zijn"); }
        GameObject resourceGoPrefab = ResourcePrefabs.Get().Single(x => x.ItemType == itemType).ResourcePrefab; // is er altijd als het goed is          

        resourceBeingCarriedGo = Instantiate(resourceGoPrefab, this.transform, false);
        if (resourceGoPrefab == Load.GoMapRscToCarry[Constants.GO_PREFAB_RSC_TO_CARRY_CUBE_UNKNOWN])
        {
            resourceBeingCarriedGo.GetComponent<SetMaterialForItemTypeBehaviour>().SetMaterial(itemType); // juiste material
        }
        resourceBeingCarriedGo.transform.localPosition = new Vector3(0, 1.2f, 0.5f); // net voor de borst -> voor nu hardcoded, allemaal cubes
    }

    public void InitiateResourceBubbleToPickup(ItemType itemType)
    {
        //if (ResourceBubbleToPickup != null) { throw new Exception("zou al destroyed moeten zijn"); }
        if (resourceBubbleToPickupGo != null) { Destroy(resourceBubbleToPickupGo); }

        GameObject resourceGoPrefab = ResourcePrefabs.Get().Single(x => x.ItemType == itemType).ResourcePrefab; // is er altijd als het goed is
        resourceBubbleToPickupGo = Instantiate(resourceGoPrefab, this.transform, false);
        if (resourceGoPrefab == Load.GoMapRscToCarry[Constants.GO_PREFAB_RSC_TO_CARRY_CUBE_UNKNOWN])
        {
            resourceBubbleToPickupGo.GetComponent<SetMaterialForItemTypeBehaviour>().SetMaterial(itemType); // juiste material
        }
        resourceBubbleToPickupGo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        resourceBubbleToPickupGo.transform.localPosition = new Vector3(0, 2.4f, 0); // net voor de borst -> voor nu hardcoded, allemaal cubes
    }    

    public void FinishedCarryingResource()
    {       
        if (resourceBeingCarriedGo != null)
        {
            Destroy(resourceBeingCarriedGo);
        }
        if (resourceBubbleToPickupGo != null)
        {
            Destroy(resourceBubbleToPickupGo);
        }
    }

    public void UpdateSerfOrder(SerfOrder serfOrder)
    {     
        if (serfOrder.Status == Status.SUCCESS || serfOrder.Status == Status.FAILED)
        {
            FinishedCarryingResource();
        }
        else if (serfOrder.Status == Status.IN_PROGRESS_FROM)
        {
            InitiateResourceBubbleToPickup(serfOrder.ItemType);
        }
        else if (serfOrder.Status == Status.IN_PROGRESS_TO)
        {
            InitiateCarryingResource(serfOrder.ItemType);
        }        
    }
}