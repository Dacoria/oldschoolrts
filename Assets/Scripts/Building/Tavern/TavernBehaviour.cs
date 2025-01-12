using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class TavernBehaviour : BaseAEMonoCI
{
    private int maxBufferForEachFoodType = 5;

    public List<ItemAmount> StockpileOfItemsRequired;

    private List<SerfRequest> incomingOrders = new List<SerfRequest>();

    [ComponentInject] private TavernRefillingBehaviour tavernRefillingBehaviour;
    [ComponentInject] private IOrderDestination orderDestination;

    void Start()
    {
        InitiateStockpile();
        foreach (var itemAmount in FoodConsumptionSettings.ItemFoodRefillValues)
        {
            AddSerfRequestTillBuffer(itemAmount.ItemType);
        }
    }

    protected override void OnReachedFoodRefillingPoint(TavernBehaviour tavernScript, FoodConsumptionBehaviour foodConsumptionBehaviour)
    {
        if(tavernScript == this)
        {
            if(HasFoodForRefill() && !TavernIsFull())
            {
                // TODO LOGICA VOOR WELKE --> MEERDERE FOODS CONSUMEREN????? voor nu 1
                foodConsumptionBehaviour.FoodConsumption.FoodConsumptionStatus = FoodConsumptionStatus.IS_REFILLING;
                var foodToConsume = GetFoodToConsume();
                foodToConsume.Amount--;

                tavernRefillingBehaviour.StartFoodRefilling(foodToConsume.ItemType, foodConsumptionBehaviour);
            }
            else
            {
                foodConsumptionBehaviour.FoodConsumption.FoodConsumptionStatus = FoodConsumptionStatus.REFILL_FAILED; // triggert event
            }            
        }
    }

    public void ConsumeFoodItem(TavernRefillItem item)
    {
        var foodConsumption = item.FoodConsumptionBehaviour.FoodConsumption;
        var refillValueOfFood = FoodConsumptionSettings.ItemFoodRefillValues.Single(x => x.ItemType == item.FoodType).RefillValue;

        foodConsumption.RefillFood(refillValueOfFood);
    }

    private ItemAmount GetFoodToConsume()
    {
        var stockpileItemsAvailable = StockpileOfItemsRequired.Where(x => x.Amount >= 1).ToList();
        var randomListIndex = Random.Range(0, stockpileItemsAvailable.Count() - 1 );
        var randomFood = stockpileItemsAvailable[randomListIndex];

        return randomFood;
    }

    private bool TavernIsFull() => tavernRefillingBehaviour.TavernRefillItems.Count >= FoodConsumptionSettings.Tavern_Max_Refill_Items;    
       

    // evt andere logica bij vegans oid --> voor nu: is er iets van voedsel/drinken beschikbaar
    public bool HasFoodForRefill() => StockpileOfItemsRequired.Sum(x => x.Amount) > 0;

    private void InitiateStockpile()
    {
        StockpileOfItemsRequired = new List<ItemAmount>();
        foreach (var itemConsumedToProduce in FoodConsumptionSettings.ItemFoodRefillValues)
        {
            StockpileOfItemsRequired.Add(new ItemAmount() { ItemType = itemConsumedToProduce.ItemType, Amount = 0 });
        }
    }

    public void AddSerfRequestTillBuffer(ItemType itemType)
    {
        var orderCountIncoming = incomingOrders.Count(x => x.ItemType == itemType);
        var orderCountStocked = StockpileOfItemsRequired.Single(x => x.ItemType == itemType).Amount;
        var itemTypeConfig = FoodConsumptionSettings.ItemFoodRefillValues.Single(x => x.ItemType == itemType);

        for (var i = (orderCountIncoming + orderCountStocked); i < maxBufferForEachFoodType; i++)
        {
            NewRequest(itemType);
        }
    }

    protected override void OnOrderStatusChanged(SerfOrder serfOrder, Status prevStatus)
    {
        var myRequest = incomingOrders.FirstOrDefault(serfOrder.Has);
        if (myRequest != null)
        {
            switch (serfOrder.Status)
            {
                case Status.SUCCESS:
                    var stockpileItem = StockpileOfItemsRequired.Single(x => x.ItemType == serfOrder.ItemType);
                    stockpileItem.Amount++;

                    incomingOrders.Remove(myRequest);
                    AddSerfRequestTillBuffer(myRequest.ItemType);
                    break;
                case Status.FAILED:                    
                    incomingOrders.Remove(myRequest);
                    AddSerfRequestTillBuffer(myRequest.ItemType);
                    break;
                    
            }
        }
    }

    private void NewRequest(ItemType itemType)
    {
        var serfRequest = new SerfRequest
        {
            Purpose = Purpose.LOGISTICS,
            ItemType = itemType,
            OrderDestination = this.orderDestination,
            IsOriginator = true,
            Direction = Direction.PULL,
            BufferDepth = incomingOrders.Count,
        };
        AE.SerfRequest?.Invoke(serfRequest);
        incomingOrders.Add(serfRequest);
    }
}