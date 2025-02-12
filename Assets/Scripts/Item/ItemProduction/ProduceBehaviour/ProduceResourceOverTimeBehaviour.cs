using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProduceResourceOverTimeBehaviour : MonoBehaviourCI, ICardOneProdBuilding
{
    [ComponentInject] private BuildingBehaviour buildingBehaviour;
    private RefillBehaviour refillBehaviour;
    private ConsumeRefillItemsBehaviour consumeRefillItemsBehaviour;
    private ProduceCRBehaviour produceCRBehaviour;
    private HandleProduceResourceOrderBehaviour handleProduceResourceOrderBehaviour;

    // via start -> zorgt dat bij real activeren, geen nieuwe comp. worden aangemaakt
    private void Start()
    {
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> { typeof(RefillBehaviour), typeof(ConsumeRefillItemsBehaviour), typeof(ProduceCRBehaviour), typeof(HandleProduceResourceOrderBehaviour) });

        refillBehaviour = gameObject.AddComponent<RefillBehaviour>();
        consumeRefillItemsBehaviour = gameObject.AddComponent<ConsumeRefillItemsBehaviour>();
        produceCRBehaviour = gameObject.AddComponent<ProduceCRBehaviour>();
        handleProduceResourceOrderBehaviour = gameObject.AddComponent<HandleProduceResourceOrderBehaviour>();

        StartCoroutine(TryToProduceOverXSeconds());
    }   

    private bool CanProduce(ItemProduceSetting itemProduceSetting)
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

    public GameObject GetGameObject() => gameObject;
    public List<TypeProcessing> GetCurrentTypesProcessed() => produceCRBehaviour.CurrentTypesProcessed;
}