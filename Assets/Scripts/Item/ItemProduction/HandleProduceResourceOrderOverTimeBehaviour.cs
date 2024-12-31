using System;
using System.Collections;
using System.Collections.Generic;

public class HandleProduceResourceOrderOverTimeBehaviour : MonoBehaviourCI
{
    [ComponentInject] private IResourcesToProduceSettings resourcesToProduceSettings;
    [ComponentInject] private BuildingBehaviour buildingBehaviour;

    public HandleProduceResourceOrderBehaviour ProduceResourceOrderBehaviour;
    public Action<List<ItemOutput>> StartedProducingAction; // evt aanhaken op deze events
    public Action<List<ItemOutput>> FinishedProducingAction; // evt aanhaken op deze events
    public Action FinishedWaitingAfterProducingAction; // evt aanhaken op deze events

    public List<ItemOutput> ItemsBeingProduced;
    public bool IsProducingResourcesRightNow => ItemsBeingProduced != null;
    public DateTime StartTimeProducing;
    public float ProductionTimeItemBeingProduced;

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
            var buildTimes = buildingBehaviour.BuildingType.GetProductionDurationSettings(); // verschilt momenteel niet per item
            StartCoroutine(ProduceResourceOverTime(itemToProduceSettings.ItemsToProduce, buildTimes.TimeToProduceResourceInSeconds, buildTimes.TimeToWaitAfterProducingInSeconds));
        }
    }

    private IEnumerator ProduceResourceOverTime(List<ItemOutput> itemsToProduce, float produceTimeInSec, float waitTimeInSec)
    {
        StartTimeProducing = DateTime.Now;
        ItemsBeingProduced = itemsToProduce;
        ProductionTimeItemBeingProduced = produceTimeInSec;
        StartedProducingAction?.Invoke(itemsToProduce);
        yield return Wait4Seconds.Get(produceTimeInSec);
                
        ProduceResourceOrderBehaviour.ProduceItemsNoConsumption(itemsToProduce);
        ItemsBeingProduced = null;
        ProductionTimeItemBeingProduced = 0f;
        FinishedProducingAction?.Invoke(itemsToProduce);

        yield return Wait4Seconds.Get(waitTimeInSec);
        FinishedWaitingAfterProducingAction?.Invoke();

        StartCoroutine(TryToProduceResourceOverXSeconds());
    }
}