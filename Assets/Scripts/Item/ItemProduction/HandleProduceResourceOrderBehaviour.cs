using System.Collections.Generic;

public class HandleProduceResourceOrderBehaviour : BaseAEMonoCI
{
    public List<SerfRequest> OutputOrders = new List<SerfRequest>();
    [ComponentInject] public IResourcesToProduce ResourcesToProduce;
    [ComponentInject] private IOrderDestination orderDestination;

    public void ProduceItems(ItemProduceSetting itemProduceSetting)
    {
        foreach (var item in itemProduceSetting.ItemsToProduce)
        {
            ProduceItem(item);
        }
    }

    public void ProduceItems()
    {
        var itemProduceSetting = ResourcesToProduce.GetItemToProduce();
        ResourcesToProduce.ConsumeRequiredResources(itemProduceSetting);              

        foreach (var itemProduced in itemProduceSetting.ItemsToProduce)
        {
            ProduceItem(itemProduced);
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