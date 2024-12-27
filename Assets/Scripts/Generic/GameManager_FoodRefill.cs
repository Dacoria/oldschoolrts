using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : BaseAEMonoCI
{
    public List<ItemFoodRefillValue> ItemFoodRefillValues => new List<ItemFoodRefillValue>
    {
        new ItemFoodRefillValue{ ItemType = ItemType.BREAD, RefillValue = 0.5f, TimeToConsumeInSec = 5},
        new ItemFoodRefillValue{ ItemType = ItemType.FISH, RefillValue = 0.3f, TimeToConsumeInSec = 6},
        new ItemFoodRefillValue{ ItemType = ItemType.SAUSAGE, RefillValue = 0.6f, TimeToConsumeInSec = 10},
        new ItemFoodRefillValue{ ItemType = ItemType.WILDMEAT, RefillValue = 0.4f, TimeToConsumeInSec = 25},
        new ItemFoodRefillValue{ ItemType = ItemType.REDBERRIES, RefillValue = 0.15f, TimeToConsumeInSec = 3},
    };
}