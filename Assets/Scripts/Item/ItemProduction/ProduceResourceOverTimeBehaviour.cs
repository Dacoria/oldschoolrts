using System.Collections.Generic;

public class ProduceResourceOverTimeBehaviour : ProduceResourceAbstract, IProduceResourceOverTimeDurations
{
    public List<ItemProduceSetting> ResourcesToProduce;
    private HandleProduceResourceOrderOverTimeBehaviour handleProduceResourceOrderOverTimeBehaviour;

    public float ProduceTimeInSeconds;
    public float WaitAfterProduceTimeInSeconds;

    public float TimeToProduceResourceInSeconds => ProduceTimeInSeconds;
    public float TimeToWaitAfterProducingInSeconds => WaitAfterProduceTimeInSeconds;

    private new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> { typeof(HandleProduceResourceOrderOverTimeBehaviour) });

        handleProduceResourceOrderOverTimeBehaviour = gameObject.AddComponent<HandleProduceResourceOrderOverTimeBehaviour>();
    }

    protected override List<ItemProduceSetting> GetConcreteResourcesToProduce() => ResourcesToProduce;
}
