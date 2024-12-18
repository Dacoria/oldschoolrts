using System;
using UnityEngine;

public class DeerDyingScript : MonoBehaviour,IRetrieveResourceFromObject
{
    private bool IsKilled => TimeKilled.HasValue;
    private bool IsBeingOrHasBeenRetrieved = false;
    private DateTime? TimeKilled;

    public HarvestMaterialResource ResourceIsRetrieved()
    {
        if (IsKilled)
        {
            return null; // ander houthakker heeft boom al omgehakt? niks meer om te retrieven
        }

        TimeKilled = DateTime.Now;
        this.GetComponentInChildren<Animator>().SetTrigger(Constants.ANIM_TRIGGER_DIE);

        var moveObjectDownSurfaceAndDestroy = this.gameObject.AddComponent<MoveObjectDownSurfaceAndDestroy>();
        moveObjectDownSurfaceAndDestroy.waitTimeInSecondsBeforeGoingDown = 2;
        moveObjectDownSurfaceAndDestroy.distanceBelowSurface = -3;
        moveObjectDownSurfaceAndDestroy.AlsoDestroyParent = false;

        return new HarvestMaterialResource(MaterialResourceType.WILDANIMAL, 1);
    }
  
    public bool CanRetrieveResource()
    {
        return !IsBeingOrHasBeenRetrieved;
    }
    public void StartRetrievingResource(int materialNumberRequestedToHarvest = 1)
    {
        IsBeingOrHasBeenRetrieved = true;
    }
}