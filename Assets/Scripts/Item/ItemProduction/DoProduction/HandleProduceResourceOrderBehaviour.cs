using System;
using System.Collections.Generic;

public class HandleProduceResourceOrderBehaviour : BaseAEMonoCI, IProduce
{
    public List<SerfRequest> OutputOrders = new List<SerfRequest>();
    [ComponentInject] private IOrderDestination orderDestination;

    public void Produce(List<Enum> types)
    {
        types.ForEach(type => ProduceItem((ItemType)type));
    }

    private void ProduceItem(ItemType itemType)
    {
        var serfRequest = new SerfRequest
        {
            OrderDestination = this.orderDestination,
            ItemType = itemType,
            Purpose = Purpose.LOGISTICS,
            BufferDepth = OutputOrders.Count,
            Direction = Direction.PUSH,
            IsOriginator = true,
        };
        AE.SerfRequest?.Invoke(serfRequest);
        OutputOrders.Add(serfRequest);        
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