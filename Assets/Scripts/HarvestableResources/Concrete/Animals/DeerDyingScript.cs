using System;
using UnityEngine;

public class DeerDyingScript : MonoBehaviour,IRetrieveResourceFromObject
{
    private bool isKilled => timeKilled.HasValue;
    private bool isBeingOrHasBeenRetrieved = false;
    private DateTime? timeKilled;

    public HarvestMaterialResource ResourceIsRetrieved()
    {
        if (isKilled)
        {
            return null; // ander houthakker heeft boom al omgehakt? niks meer om te retrieven
        }

        timeKilled = DateTime.Now;
        this.GetComponentInChildren<Animator>().SetTrigger(Constants.ANIM_TRIGGER_DIE);

        var moveObjectDownSurfaceAndDestroy = this.gameObject.AddComponent<MoveObjectDownSurfaceAndDestroy>();
        moveObjectDownSurfaceAndDestroy.waitTimeInSecondsBeforeGoingDown = 2;
        moveObjectDownSurfaceAndDestroy.distanceBelowSurface = -3;
        moveObjectDownSurfaceAndDestroy.AlsoDestroyParent = false;

        return new HarvestMaterialResource(MaterialResourceType.WILDANIMAL, 1);
    }
  
    public bool CanRetrieveResource()
    {
        return !isBeingOrHasBeenRetrieved;
    }
    public void StartRetrievingResource(int materialNumberRequestedToHarvest = 1)
    {
        isBeingOrHasBeenRetrieved = true;
    }
}