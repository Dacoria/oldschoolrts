using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleProduceResourceOrderOverTimeBehaviour : MonoBehaviourCI
{
    [ComponentInject] public IProduceResourceOverTimeDurations Durations;
    [ComponentInject] private IResourcesToProduceSettings resourcesToProduceSettings;

    public HandleProduceResourceOrderBehaviour ProduceResourceOrderBehaviour;
    public Action FinishedProducingAction; // evt aanhaken op deze events
    public Action FinishedWaitingAfterProducingAction; // evt aanhaken op deze events

    public bool IsProducingResourcesRightNow;
    public DateTime StartTimeProducing;

    private new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<Type> { typeof(HandleProduceResourceOrderBehaviour) });

        ProduceResourceOrderBehaviour = gameObject.AddComponent<HandleProduceResourceOrderBehaviour>();        
    }

    private void Start()
    {
        StartCoroutine(TryToProduceResourceOverXSeconds());
    }

    private IEnumerator TryToProduceResourceOverXSeconds()
    {
        var itemToProduceSettings = resourcesToProduceSettings.GetItemToProduceSettings();
        if (itemToProduceSettings == null)
        {
            yield return Wait4Seconds.Get(0.1f); // kan nog niet produceren, doe check opnieuw na x secondes
            StartCoroutine(TryToProduceResourceOverXSeconds());
        }
        else
        {
            resourcesToProduceSettings.ConsumeRequiredResources(itemToProduceSettings);
            StartCoroutine(ProduceResourceOverTime(itemToProduceSettings.ItemsToProduce, Durations.TimeToProduceResourceInSeconds, Durations.TimeToWaitAfterProducingInSeconds));
        }
    }

    private IEnumerator ProduceResourceOverTime(List<ItemOutput> ItemsToProduce, float produceTimeInSec, float waitTimeInSec)
    {
        StartTimeProducing = DateTime.Now;
        IsProducingResourcesRightNow = true;
        yield return Wait4Seconds.Get(produceTimeInSec);
        IsProducingResourcesRightNow = false;

        FinishedProducingAction?.Invoke();
        ProduceResourceOrderBehaviour.ProduceItemsNoConsumption(ItemsToProduce);

        yield return Wait4Seconds.Get(waitTimeInSec);
        FinishedWaitingAfterProducingAction?.Invoke();

        StartCoroutine(TryToProduceResourceOverXSeconds());
    }
}