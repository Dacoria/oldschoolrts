using System;
using UnityEngine;

public class FoodConsumption
{
    public float FoodSatisfactionPercentage;
    public float FoodDeclinePercPerSecond;
    private float PercLimitForFoodRefill;

    public bool NeedsFoodRefill;
    public GameObject TavernTargetedForFoodRefill;

    public FoodConsumption(float FoodSatisfactionPercentage, float FoodDeclinePercPerSecond, float PercLimitForFoodRefill)
    {
        this.FoodSatisfactionPercentage = FoodSatisfactionPercentage;
        this.FoodDeclinePercPerSecond = FoodDeclinePercPerSecond;
        this.PercLimitForFoodRefill = PercLimitForFoodRefill;
        NeedsFoodRefill = FoodSatisfactionPercentage <= PercLimitForFoodRefill;
    }

    public void RefillFood(float refillValue)
    {
        FoodSatisfactionPercentage = Math.Min(FoodSatisfactionPercentage + refillValue, 1f);
        FoodConsumptionStatus = FoodConsumptionStatus.REFILL_SUCCESS;
    }

    public bool TrySetTavernToGetFood()
    {
        TavernTargetedForFoodRefill = MonoHelper.Instance.GetClosestTavernWithFood();
        if (TavernTargetedForFoodRefill != null)
        {            
            return true;
        }

        return false;
    }

    public void ConsumeFood()
    {
        if (_foodConsumptionStatus != FoodConsumptionStatus.IS_REFILLING)
        {
            FoodSatisfactionPercentage = Mathf.Max(FoodSatisfactionPercentage - FoodDeclinePercPerSecond, 0);
            NeedsFoodRefill = FoodSatisfactionPercentage <= PercLimitForFoodRefill;
            if(NeedsFoodRefill &&
                !IsBusyWithFoodRefillig() &&
                MonoHelper.Instance.GetClosestTavernWithFood() != null // als er geen voedsel te halen valt --> dan hoef je het ook niet te proberen
                )
            {
                FoodConsumptionStatus = FoodConsumptionStatus.NEEDS_REFILL; // start event
            }

            if (FoodSatisfactionPercentage == 0)
            {
                AE.NoFoodToConsume(this);
            }
        }
    }  

    private FoodConsumptionStatus _foodConsumptionStatus;
    public FoodConsumptionStatus FoodConsumptionStatus
    {
        get => _foodConsumptionStatus;
        set
        {
            if (_foodConsumptionStatus != value)
            {
                var previousValue = _foodConsumptionStatus;
                _foodConsumptionStatus = value;
                AE.FoodStatusHasChanged(this, previousValue);
            }
        }
    }

    private bool IsBusyWithFoodRefillig()
    {
        switch (FoodConsumptionStatus) {
            case FoodConsumptionStatus.NEEDS_REFILL:
            case FoodConsumptionStatus.IS_REFILLING:
            case FoodConsumptionStatus.GO_TOWARDS_REFILLL_POINT:
                return true;
            default:
                return false;
        }
    }    
}
