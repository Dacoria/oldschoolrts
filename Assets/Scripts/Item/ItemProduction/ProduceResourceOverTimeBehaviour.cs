using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ProduceResourceOverTimeBehaviour : MonoBehaviourCI
{
    [ComponentInject] private BuildingBehaviour buildingBehaviour;
    private RefillBehaviour refillBehaviour;
    private ConsumeRefillItemsBehaviour consumeRefillItemsBehaviour;
    private ProduceCRBehaviour produceCRBehaviour;
    private HandleProduceResourceOrderBehaviour handleProduceResourceOrderBehaviour;

    private new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour), typeof(ProduceCRBehaviour), typeof(HandleProduceResourceOrderBehaviour) });

        refillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItemsBehaviour = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
        produceCRBehaviour = gameObject.AddComponent<ProduceCRBehaviour>();
        handleProduceResourceOrderBehaviour = gameObject.AddComponent<HandleProduceResourceOrderBehaviour>();
    }

    private void Start()
    {
        StartCoroutine(TryToProduceOverXSeconds());
    }

    public bool CanProduce(ItemProduceSetting itemProduceSetting)
    {
        if (ItemProdHelper.HasReachedRscProductionBuffer(itemProduceSetting.ItemsToProduce, handleProduceResourceOrderBehaviour))
            return false;

        if (!consumeRefillItemsBehaviour.CanConsumeRefillItems(itemProduceSetting.ItemsConsumedToProduce))
            return false;

        if (!produceCRBehaviour.IsReadyForNextProduction())
            return false;

        return true;
    }

    private IEnumerator TryToProduceOverXSeconds()
    {
        var itemToProduceSettings = buildingBehaviour.BuildingType.GetItemProduceSettings().FirstOrDefault(x => CanProduce(x));
        if (itemToProduceSettings == null)
        {
            yield return Wait4Seconds.Get(0.1f); // kan nog niet produceren, doe check opnieuw na x secondes
            StartCoroutine(TryToProduceOverXSeconds());
        }
        else
        {
            consumeRefillItemsBehaviour.TryConsumeRefillItems(itemToProduceSettings.ItemsConsumedToProduce);
            produceCRBehaviour.ProduceOverTime(new ProduceSetup(
                itemToProduceSettings.ItemsToProduce, 
                handleProduceResourceOrderBehaviour,
                waitAfterProduceCallback: () => StartCoroutine(TryToProduceOverXSeconds())));
        }
    }
}