using System;
using System.Collections.Generic;

public class HandleProduceResourceOrderBehaviour : BaseAEMonoCI
{
    public List<SerfRequest> OutputOrders = new List<SerfRequest>();
    [ComponentInject] private IOrderDestination orderDestination;

    private new void Awake()
    {
        base.Awake();
        
        // check zodat je altijd goed zit met events (en dit niet direct aftrapt)
        gameObject.AddComponent<ValidComponents>().DoCheck(
            actives: new List<Type> { typeof(ProduceCRBehaviour) });
    }

    public void ProduceItems(List<ItemOutput> ItemsToProduce) => ItemsToProduce.ForEach(item => ProduceItem(item));

    public void ProduceItem(ItemOutput itemProduced)
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