using System.Linq;
using UnityEngine;
using System;

public class SerfResourceCarryingBehaviour : MonoBehaviour
{
    private GameObject ResourceBeingCarriedGo;
    private GameObject ResourceBubbleToPickupGo;

    public void InitiateCarryingResource(ItemType itemType)
    {
        if (ResourceBubbleToPickupGo != null)
        {
            Destroy(ResourceBubbleToPickupGo);
        }

        if (ResourceBeingCarriedGo != null) { throw new Exception("zou al destroyed moeten zijn"); }
        GameObject resourceGoPrefab = ResourcePrefabs.Get().Single(x => x.ItemType == itemType).ResourcePrefab; // is er altijd als het goed is          

        ResourceBeingCarriedGo = Instantiate(resourceGoPrefab, this.transform, false);
        if (resourceGoPrefab == Load.GoMap["CubeUnknownBeingCarried"])
        {
            ResourceBeingCarriedGo.GetComponent<SetMaterialForItemTypeBehaviour>().SetMaterial(itemType); // juiste material
        }
        ResourceBeingCarriedGo.transform.localPosition = new Vector3(0, 1.2f, 0.5f); // net voor de borst -> voor nu hardcoded, allemaal cubes
    }

    public void InitiateResourceBubbleToPickup(ItemType itemType)
    {
        //if (ResourceBubbleToPickup != null) { throw new Exception("zou al destroyed moeten zijn"); }
        if (ResourceBubbleToPickupGo != null) { Destroy(ResourceBubbleToPickupGo); }

        GameObject resourceGoPrefab = ResourcePrefabs.Get().Single(x => x.ItemType == itemType).ResourcePrefab; // is er altijd als het goed is
        ResourceBubbleToPickupGo = Instantiate(resourceGoPrefab, this.transform, false);
        if (resourceGoPrefab == Load.GoMap["CubeUnknownBeingCarried"])
        {
            ResourceBubbleToPickupGo.GetComponent<SetMaterialForItemTypeBehaviour>().SetMaterial(itemType); // juiste material
        }
        ResourceBubbleToPickupGo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        ResourceBubbleToPickupGo.transform.localPosition = new Vector3(0, 2.4f, 0); // net voor de borst -> voor nu hardcoded, allemaal cubes
    }    

    public void FinishedCarryingResource()
    {       
        if (ResourceBeingCarriedGo != null)
        {
            Destroy(ResourceBeingCarriedGo);
        }
        if (ResourceBubbleToPickupGo != null)
        {
            Destroy(ResourceBubbleToPickupGo);
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