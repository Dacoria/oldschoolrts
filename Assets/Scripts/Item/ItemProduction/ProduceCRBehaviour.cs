using System;
using System.Collections;
using System.Collections.Generic;

public class ProduceCRBehaviour : MonoBehaviourCI
{
    [ComponentInject] private BuildingBehaviour buildingBehaviour;
    public bool IsProducingResourcesRightNow => CurrentTypesProcessed != null && ProduceDurations != null;
    public bool IsReadyForNextProduction => ProduceDurations != null;

    public List<TypeProcessing> CurrentTypesProcessed { get; private set; }
    public ProduceDurations ProduceDurations { get; private set; }

    public void ProduceOverTime(ProduceSetup produceSetup)
    {
        var durations = buildingBehaviour.BuildingType.GetProductionDurationSettings();
        StartCoroutine(CR_ProduceOverTime(produceSetup, durations));
    }   

    private IEnumerator CR_ProduceOverTime(ProduceSetup produceSetup, ProduceDurations durations)
    {
        CurrentTypesProcessed = produceSetup.ProduceTypes.ConvertAll(t => new TypeProcessing { Type = t, StartTimeBeingBuild = DateTime.Now});
        var typesToProduce = CurrentTypesProcessed.ConvertAll(x => x.Type);
        this.ProduceDurations = durations;
        AE.StartedProducingAction?.Invoke(buildingBehaviour, typesToProduce);

        yield return Wait4Seconds.Get(durations.TimeToProduceResourceInSeconds);

        produceSetup.ProduceAction();        
        CurrentTypesProcessed = null;
        AE.FinishedProducingAction?.Invoke(buildingBehaviour, typesToProduce);

        yield return Wait4Seconds.Get(durations.TimeToWaitAfterProducingInSeconds);

        ProduceDurations = null;
        AE.FinishedWaitingAfterProducingAction?.Invoke(buildingBehaviour);
    }
}