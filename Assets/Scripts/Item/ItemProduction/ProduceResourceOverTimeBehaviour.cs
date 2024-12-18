using UnityEngine;

public class ProduceResourceOverTimeBehaviour : MonoBehaviour, IProduceResourceOverTime
{
    [ComponentInject(Required.OPTIONAL)]
    private ConsumeRefillItemsBehaviour ConsumeRefillItemsBehaviour;

    public void Start()
    {        
        this.ComponentInject(); // TODO -> Waarschijnlijk bewust :o --> FIX????
    }

    public float TimeToProduceResourceInSeconds = 5;
    public float TimeToWaitAfterProducingInSeconds = 1.5f;
    public float GetTimeToProduceResourceInSeconds() => TimeToProduceResourceInSeconds;
    public float GetTimeToWaitAfterProducingInSeconds() => TimeToWaitAfterProducingInSeconds;
    public void StartProducing(ItemProduceSetting itemProduceSetting) 
    {
        if (ConsumeRefillItemsBehaviour != null)
        {
            var itemsToConsume = itemProduceSetting.ItemsConsumedToProduce;
            ConsumeRefillItemsBehaviour.TryConsumeRefillItems(itemsToConsume.ConvertAll(x => (ItemAmount)x));
        }
    }
    public void FinishProducing(ItemProduceSetting itemProduceSetting) { }

    public bool CanProduceResource() => true;
}