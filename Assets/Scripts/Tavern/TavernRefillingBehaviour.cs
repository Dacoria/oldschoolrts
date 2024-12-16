using Assets;
using Assets.CrossCutting;
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
        VillagerUnitType villagerUnitType;
        if (foodConsumptionBehaviour.GetComponent<SerfBehaviour>() != null)
        {
            villagerUnitType = VillagerUnitType.Serf;
        }
        else if (foodConsumptionBehaviour.GetComponent<BuilderBehaviour>() != null)
        {
            villagerUnitType = VillagerUnitType.Builder;
        }
        else
        {
            villagerUnitType = foodConsumptionBehaviour.GetComponent<WorkManager>().VillagerUnitType;
        }

        AddItemOnFoodConsumptionQueue(foodType, villagerUnitType, foodConsumptionBehaviour);
    }

    private void AddItemOnFoodConsumptionQueue(ItemType foodType, VillagerUnitType villagerType, FoodConsumptionBehaviour foodConsumptionBehaviour)
    {
        var itemToPutOnQueue = new TavernRefillItem
        {
            FoodType = foodType,
            FoodConsumptionBehaviour = foodConsumptionBehaviour,
            VillagerType = villagerType,
            TimeToConsumeFoodInSeconds = GameManager.Instance.ItemFoodRefillValues.Single(x => x.ItemType == foodType).TimeToConsumeInSec,
            StartTimeConsumingFood = DateTime.Now
        };

        TavernRefillItems.Add(itemToPutOnQueue);
    }

    public void RemoveItemFromQueue(TavernRefillItem queueItem)
    {
        TavernRefillItems.Remove(queueItem);
    }

    public void Update()
    {
        // omdat er ook verwijderd wordt => forloop
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
