using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RefillBehaviour : BaseAEMono
{
    public List<SerfRequest> IncomingOrders;
    public List<ItemAmount> StockpileOfItemsRequired;

    [ComponentInject]
    public IRefillItems RefillItems;

    private new void Awake()
    {
        base.Awake();
        this.ComponentInject();
        StockpileOfItemsRequired = GetItemsToRefill()
            .Select(x => new ItemAmount() { ItemType = x.ItemType, Amount = 0 })
            .ToList();
    }

    private void Start()
    {
        IncomingOrders = new List<SerfRequest>();

        foreach (var itemAmount in GetItemsToRefill())
        {
            AddSerfRequestTillBuffer(itemAmount.ItemType);
        }
    }

    public void AddSerfRequestTillBuffer(ItemType itemType)
    {
        var orderCountIncoming = IncomingOrders.Count(x => x.ItemType == itemType);
        var orderCountStocked = StockpileOfItemsRequired.Single(x => x.ItemType == itemType).Amount;

        var itemCountToRefill = GetItemCountToRefill(itemType, orderCountIncoming, orderCountStocked);
        for (var i = (orderCountIncoming + orderCountStocked); i < itemCountToRefill; i++)
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
                    IncomingOrders.Remove(myRequest);
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
            GameObject = this.gameObject,
            IsOriginator = true,
            Direction = Direction.PULL,
            BufferDepth = IncomingOrders.Count,
        };
        AE.SerfRequest(serfRequest);
        IncomingOrders.Add(serfRequest);
    }

    public List<ItemAmount> GetItemsToRefill()
    {
        var settings = RefillItems.GetItemProduceSettings();
        var maxBufferPerItemtype = settings
            .SelectMany(x => x.ItemsConsumedToProduce)
            .GroupBy(x => x.ItemType)
            .Select(x => new ItemAmount
            {
                ItemType = x.First().ItemType,
                Amount = x.Max(x => x.MaxBuffer)
            })
            .ToList();

        return maxBufferPerItemtype;
    }

    public int GetItemCountToRefill(ItemType itemType, int countIncoming, int countStocked)
    {
        var alwaysRefillItems = RefillItems.AlwaysRefillItemsIgnoreBuffer();
        var settings = RefillItems.GetItemProduceSettings();

        if(alwaysRefillItems)
        {
            return 5 - countIncoming + countStocked; // altijd 5 orders om items te brengen 
        }
        
        return settings
            .SelectMany(x => x.ItemsConsumedToProduce)
            .Where(x => x.ItemType == itemType)
            .OrderByDescending(x => x.MaxBuffer)
            .First()
            .MaxBuffer;
    }
}