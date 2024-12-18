using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class TavernBehaviour : BaseAEMonoCI
{
    public int MaxBufferForEachFoodType = 5;

    public List<SerfRequest> IncomingOrders;
    public List<ItemAmount> StockpileOfItemsRequired;

    [ComponentInject]
    private TavernRefillingBehaviour TavernRefillingBehaviour;

    new void Awake()
    {
        base.Awake();
        InitiateStockpile(); // moet voor Start --> CanProduce wordt al eerder aangeroepen
    }
    void Start()
    {
        IncomingOrders = new List<SerfRequest>();

        foreach (var itemAmount in GameManager.Instance.ItemFoodRefillValues)
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

                TavernRefillingBehaviour.StartFoodRefilling(foodToConsume.ItemType, foodConsumptionBehaviour);
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
        var refillValueOfFood = GameManager.Instance.ItemFoodRefillValues.Single(x => x.ItemType == item.FoodType).RefillValue;

        foodConsumption.RefillFood(refillValueOfFood);
    }

    private ItemAmount GetFoodToConsume()
    {
        var stockpileItemsAvailable = StockpileOfItemsRequired.Where(x => x.Amount >= 1).ToList();
        var randomListIndex = Random.Range(0, stockpileItemsAvailable.Count() - 1 );
        var randomFood = stockpileItemsAvailable[randomListIndex];

        return randomFood;
    }

    private bool TavernIsFull() => TavernRefillingBehaviour.TavernRefillItems.Count >= FoodConsumptionSettings.Tavern_Max_Refill_Items;    
       

    // evt andere logica bij vegans oid --> voor nu: is er iets van voedsel/drinken beschikbaar
    public bool HasFoodForRefill() => StockpileOfItemsRequired.Sum(x => x.Amount) > 0;

    private void InitiateStockpile()
    {
        StockpileOfItemsRequired = new List<ItemAmount>();
        foreach (var itemConsumedToProduce in GameManager.Instance.ItemFoodRefillValues)
        {
            StockpileOfItemsRequired.Add(new ItemAmount() { ItemType = itemConsumedToProduce.ItemType, Amount = 0 });
        }
    }

    public void AddSerfRequestTillBuffer(ItemType itemType)
    {
        var orderCountIncoming = IncomingOrders.Count(x => x.ItemType == itemType);
        var orderCountStocked = StockpileOfItemsRequired.Single(x => x.ItemType == itemType).Amount;
        var itemTypeConfig = GameManager.Instance.ItemFoodRefillValues.Single(x => x.ItemType == itemType);

        for (var i = (orderCountIncoming + orderCountStocked); i < MaxBufferForEachFoodType; i++)
        {
            NewRequest(itemType);
        }
    }

    protected override void OnOrderStatusChanged(SerfOrder serfOrder)
    {
        var myRequest = IncomingOrders.FirstOrDefault(serfOrder.Has);
        if (myRequest != null)
        {
            switch (serfOrder.Status)
            {
                case Status.SUCCESS:
                    var stockpileItem = StockpileOfItemsRequired.Single(x => x.ItemType == serfOrder.ItemType);
                    stockpileItem.Amount++;

                    IncomingOrders.Remove(myRequest);
                    AddSerfRequestTillBuffer(myRequest.ItemType);
                    break;
                case Status.FAILED:
                    {
                        IncomingOrders.Remove(myRequest);
                        AddSerfRequestTillBuffer(myRequest.ItemType);
                        break;
                    }
            }
        }
    }

    private void NewRequest(ItemType itemType)
    {
        var serfRequest = new SerfRequest
        {
            Purpose = Purpose.LOGISTICS,
            ItemType = itemType,
            GameObject = this.gameObject,
            IsOriginator = true,
            Direction = Direction.PULL,
            BufferDepth = IncomingOrders.Count,
        };
        AE.SerfRequest?.Invoke(serfRequest);
        IncomingOrders.Add(serfRequest);
    }
}