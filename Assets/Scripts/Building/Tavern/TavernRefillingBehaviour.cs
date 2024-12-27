using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TavernRefillingBehaviour : MonoBehaviour
{
    public List<TavernRefillItem> TavernRefillItems;
    public TavernBehaviour CallingTavern;

    void Awake()
    {
        TavernRefillItems = new List<TavernRefillItem>();
        CallingTavern = GetComponent<TavernBehaviour>();
    }

    public void StartFoodRefilling(ItemType foodType, FoodConsumptionBehaviour foodConsumptionBehaviour)
    {
        var villagerUnitType = VillagerManager.Instance.DetermineVillagerUnitType(gameObject);
        var itemToPutOnQueue = new TavernRefillItem
        {
            FoodType = foodType,
            FoodConsumptionBehaviour = foodConsumptionBehaviour,
            VillagerType = villagerUnitType,
            TimeToConsumeFoodInSeconds = FoodConsumptionSettings.ItemFoodRefillValues.Single(x => x.ItemType == foodType).TimeToConsumeInSec,
            StartTimeConsumingFood = DateTime.Now
        };

        TavernRefillItems.Add(itemToPutOnQueue);
    }

    public void Update()
    {
        // omdat er ook verwijderd wordt => forloop inverse
        for (int i = TavernRefillItems.Count - 1; i >= 0; i--)
        {
            var item = TavernRefillItems[i];
            var timeItemFinished = item.StartTimeConsumingFood.Value.AddSeconds(item.TimeToConsumeFoodInSeconds);
            if (timeItemFinished <= DateTime.Now)
            {
                HandleItemFinished(item);
            }
        }              
    }

    private void HandleItemFinished(TavernRefillItem item)
    { 
        // doe actie
        CallingTavern.ConsumeFoodItem(item);

        // verwijder item van queue
        TavernRefillItems.Remove(item);
    }
}