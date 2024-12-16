using Assets;
using Assets.CrossCutting;
using System;

public class TavernRefillItem
{
    public ItemType FoodType;
    public VillagerUnitType VillagerType;
    public DateTime? StartTimeConsumingFood;
    public float TimeToConsumeFoodInSeconds;
    public bool IsConsumingFood => StartTimeConsumingFood.HasValue;
    public FoodConsumptionBehaviour FoodConsumptionBehaviour;
}