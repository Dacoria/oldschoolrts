using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceResourceOrderBehaviour : BaseAEMonoCI
{
    [HideInInspector]
    public int CurrentOutstandingOrders = 0;

    public List<SerfRequest> OutputOrders;

    [ComponentInject(Required.OPTIONAL)]  public IProduceResourceOverTime ProduceOverTime;

    [ComponentInject] public IResourcesToProduce ResourcesToProduce;

    void Start()
    {
        OutputOrders = new List<SerfRequest>();

        if (IsProducingResourcesOverTime())
        {
            StartCoroutine(TryToProduceResourceOverXSeconds());
        }
    }

    public bool IsProducingResourcesOverTime() => ProduceOverTime != null;

    private IEnumerator TryToProduceResourceOverXSeconds()
    {
        if (!ResourcesToProduce.CanProduceResource())
        {
            yield return MonoHelper.Instance.GetCachedWaitForSeconds(0.3f); // kan nog niet produceren, doe check opnieuw na x secondes
            StartCoroutine(TryToProduceResourceOverXSeconds());
        }
        else
        {
            var resourceToProduce = ResourcesToProduce.GetItemToProduce();
            ResourcesToProduce.ConsumeRequiredResources(resourceToProduce);
            StartCoroutine(ProduceResourceOverXSeconds(resourceToProduce));
        }       
    }


    [HideInInspector]
    public bool IsProducingResourcesRightNow; // voor wel/niet tonen gears die draaien -> animatie door ander script

    [HideInInspector]
    public DateTime StartTimeProducing; // voor wel/niet tonen van rest. sec prod in text

    private IEnumerator ProduceResourceOverXSeconds(ItemProduceSetting itemProduceSetting)
    {
        ProduceOverTime.StartProducing(itemProduceSetting);

        IsProducingResourcesRightNow = true;
        StartTimeProducing = DateTime.Now;
        yield return MonoHelper.Instance.GetCachedWaitForSeconds(ProduceOverTime.GetTimeToProduceResourceInSeconds());

        ProduceOverTime.FinishProducing(itemProduceSetting);
        ProduceItemsOverTime(itemProduceSetting);
        IsProducingResourcesRightNow = false;

        yield return MonoHelper.Instance.GetCachedWaitForSeconds(ProduceOverTime.GetTimeToWaitAfterProducingInSeconds());
        StartCoroutine(TryToProduceResourceOverXSeconds());
    }  

    public void Update()
    {
        // TODO JELLE --> Null pointer
        CurrentOutstandingOrders = OutputOrders != null ? OutputOrders.Count : 0; 
    }

    protected override void OnOrderStatusChanged(SerfOrder serfOrder)
    {
        if (OutputOrders.Contains(serfOrder.From))
        {
            if (serfOrder.Status == Status.IN_PROGRESS_TO)
            {
                OutputOrders.Remove(serfOrder.From);
            }
        }
    }

    private void ProduceItemsOverTime(ItemProduceSetting itemProduceSetting)
    {
        foreach (var itemProduced in itemProduceSetting.ItemsToProduce)
        {
            ProduceItem(itemProduced);
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
                GameObject = this.gameObject,
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
}
