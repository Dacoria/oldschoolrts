using System;
using System.Collections;
using System.Collections.Generic;

public class ProduceCRBehaviour : MonoBehaviourCI
{
    [ComponentInject] private BuildingBehaviour buildingBehaviour;
    public bool IsProducingResourcesRightNow => CurrentTypesProcessed != null && ProduceDurations != null;
    public bool IsReadyForNextProduction() => ProduceDurations == null;

    public List<TypeProcessing> CurrentTypesProcessed { get; private set; }
    public ProduceDurations ProduceDurations { get; private set; }

    public void ProduceOverTime(ProduceSetup produceSetup)
    {
        var durations = buildingBehaviour.BuildingType.GetProductionDurationSettings();
        StartCoroutine(CR_ProduceOverTime(produceSetup, durations));
    }

    public void ProduceInstant(ProduceSetup produceSetup)
    {
        var noDurations = new ProduceDurations { TimeToProduceResourceInSeconds = 0, TimeToWaitAfterProducingInSeconds = 0};
        StartCoroutine(CR_ProduceOverTime(produceSetup, noDurations));
    }

    private IEnumerator CR_ProduceOverTime(ProduceSetup produceSetup, ProduceDurations durations)
    {
        CurrentTypesProcessed = produceSetup.ProduceTypes.ConvertAll(t => new TypeProcessing { Type = t, StartTimeBeingBuild = DateTime.Now});
        var typesToProduce = CurrentTypesProcessed.ConvertAll(x => x.Type);
        this.ProduceDurations = durations;
        AE.StartedProducingAction?.Invoke(buildingBehaviour, typesToProduce);

        yield return Wait4Seconds.Get(durations.TimeToProduceResourceInSeconds);

        produceSetup.ProduceAction.Produce(produceSetup.ProduceTypes); // voor daadwerkelijke productie van iets (via I forceren)
        produceSetup.ProduceCallback?.Invoke();         // voor callback voor een actie na produceren; bv bij een mine -> rsc eraf halen)
        CurrentTypesProcessed = null;
        AE.FinishedProducingAction?.Invoke(buildingBehaviour, typesToProduce); // voor bijhouden globale events; bv populatie-verhoging is klaar

        yield return Wait4Seconds.Get(durations.TimeToWaitAfterProducingInSeconds);

        ProduceDurations = null;
        produceSetup.WaitAfterProduceCallback?.Invoke();
        AE.FinishedWaitingAfterProducingAction?.Invoke(buildingBehaviour);
    }
}