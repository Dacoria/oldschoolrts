using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleProduceResourceOrderOverTimeBehaviour : BaseAEMonoCI
{
    public HandleProduceResourceOrderBehaviour ProduceResourceOrderBehaviour;
    public Action FinishedProducingAction; // evt aanhaken op deze events
    public Action FinishedWaitingAfterProducingAction; // evt aanhaken op deze events

    private new void Awake()
    {
        base.Awake();     
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<Type> { typeof(HandleProduceResourceOrderBehaviour) });

        ProduceResourceOrderBehaviour = gameObject.AddComponent<HandleProduceResourceOrderBehaviour>();        
    }

    public void ProduceResourceOverTime(ItemProduceSetting itemProduceSetting, float produceTimeInSec, float waitTimeInSec)
    {        
        StartCoroutine(CR_ProduceResourceOverTime(itemProduceSetting, produceTimeInSec, waitTimeInSec));
    }

    private IEnumerator CR_ProduceResourceOverTime(ItemProduceSetting itemProduceSetting, float produceTimeInSec, float waitTimeInSec)
    {
        yield return Wait4Seconds.Get(produceTimeInSec);

        FinishedProducingAction?.Invoke();
        ProduceResourceOrderBehaviour.ProduceItems(itemProduceSetting);

        yield return Wait4Seconds.Get(waitTimeInSec);
        FinishedWaitingAfterProducingAction?.Invoke();
    }
}