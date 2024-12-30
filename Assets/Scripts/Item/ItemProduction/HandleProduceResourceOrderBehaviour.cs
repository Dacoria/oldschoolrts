using System.Collections.Generic;

public class HandleProduceResourceOrderBehaviour : BaseAEMonoCI
{
    public List<SerfRequest> OutputOrders = new List<SerfRequest>();
    [ComponentInject] private IOrderDestination orderDestination;
    
    public bool ProduceItemsWithConsumption(IResourcesToProduceSettings resourcesToProduce)
    {
        var itemProduceSetting = resourcesToProduce.GetItemToProduceSettings();
        var hasConsumedResources = resourcesToProduce.ConsumeRequiredResources(itemProduceSetting);
        if(hasConsumedResources)
        {
            ProduceItemsNoConsumption(itemProduceSetting.ItemsToProduce);
            return true;
        }
        return false;
    }

    public void ProduceItemsNoConsumption(List<ItemOutput> ItemsToProduce)
    {
        foreach (var item in ItemsToProduce)
        {
            ProduceItem(item);
        }
    }

    private void ProduceItem(ItemOutput itemProduced)
    {
        for (int i = 0; i < itemProduced.ProducedPerProdCycle; i++)
        {
            var serfRequest = new SerfRequest
            {
                OrderDestination = this.orderDestination,
                ItemType = itemProduced.ItemType,
                Purpose = Purpose.LOGISTICS,
                BufferDepth = OutputOrders.Count,
                Direction = Direction.PUSH,
                IsOriginator = true,
            };
            AE.SerfRequest?.Invoke(serfRequest);
            OutputOrders.Add(serfRequest);
        }
    }

    protected override void OnOrderStatusChanged(SerfOrder serfOrder, Status prevStatus)
    {
        if (OutputOrders.Contains(serfOrder.From))
        {
            if (serfOrder.Status == Status.IN_PROGRESS_TO)
            {
                OutputOrders.Remove(serfOrder.From);
            }
        }
    }    
}