using System;
using System.Collections;
using System.Collections.Generic;

public class HandleAutoProduceResourceOrderOverTimeBehaviour : MonoBehaviourCI
{
    [ComponentInject] public IProduceResourceOverTimeDurations Durations;
    public HandleProduceResourceOrderOverTimeBehaviour HandleProduceResourceOrderOverTimeBehaviour;

    public bool IsProducingResourcesOverTime() => true;
    public bool IsProducingResourcesRightNow;
    public DateTime StartTimeProducing;

    private new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<Type> { typeof(HandleProduceResourceOrderBehaviour) });

        HandleProduceResourceOrderOverTimeBehaviour = gameObject.AddComponent<HandleProduceResourceOrderOverTimeBehaviour>();
    }

    void Start()
    {
        StartCoroutine(TryToProduceResourceOverXSeconds());
    }

    private void OnEnable()
    {
        HandleProduceResourceOrderOverTimeBehaviour.FinishedProducingAction += OnFinishedProducingAction;
        HandleProduceResourceOrderOverTimeBehaviour.FinishedWaitingAfterProducingAction += OnFinishedWaitingAfterProducingAction;
    }

    private void OnDisable()
    {
        HandleProduceResourceOrderOverTimeBehaviour.FinishedProducingAction -= OnFinishedProducingAction;
        HandleProduceResourceOrderOverTimeBehaviour.FinishedWaitingAfterProducingAction -= OnFinishedWaitingAfterProducingAction;
    }

    private void OnFinishedProducingAction() => IsProducingResourcesRightNow = false;
    private void OnFinishedWaitingAfterProducingAction() => StartCoroutine(TryToProduceResourceOverXSeconds());

    private IEnumerator TryToProduceResourceOverXSeconds()
    {        
        var rscToProduce = HandleProduceResourceOrderOverTimeBehaviour.ProduceResourceOrderBehaviour.ResourcesToProduce;
        if (!rscToProduce.CanProduceResource())
        {
            yield return Wait4Seconds.Get(0.3f); // kan nog niet produceren, doe check opnieuw na x secondes
            StartCoroutine(TryToProduceResourceOverXSeconds());
        }
        else
        {
            IsProducingResourcesRightNow = true;
            StartTimeProducing = DateTime.Now;
            var resourceToProduce = rscToProduce.GetItemToProduce();
            rscToProduce.ConsumeRequiredResources(resourceToProduce);
            HandleProduceResourceOrderOverTimeBehaviour.ProduceResourceOverTime(resourceToProduce, Durations.TimeToProduceResourceInSeconds, Durations.TimeToWaitAfterProducingInSeconds);
        }       
    }
}