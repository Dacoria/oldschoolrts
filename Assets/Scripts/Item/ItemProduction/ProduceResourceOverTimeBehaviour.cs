using System.Collections.Generic;

public class ProduceResourceOverTimeBehaviour : ProduceResourceAbstract, IProduceResourceOverTimeDurations
{
    public List<ItemProduceSetting> ResourcesToProduce;
    private HandleAutoProduceResourceOrderOverTimeBehaviour handleAutoProduceResourceOrderOverTimeBehaviour;

    public float ProduceTimeInSeconds;
    public float WaitAfterProduceTimeInSeconds;

    public float TimeToProduceResourceInSeconds => ProduceTimeInSeconds;
    public float TimeToWaitAfterProducingInSeconds => WaitAfterProduceTimeInSeconds;

    private new void Start()
    {
        base.Start();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> { typeof(HandleAutoProduceResourceOrderOverTimeBehaviour) });

        handleAutoProduceResourceOrderOverTimeBehaviour = gameObject.AddComponent<HandleAutoProduceResourceOrderOverTimeBehaviour>();
    }

    public override List<ItemProduceSetting> GetResourcesToProduce() => ResourcesToProduce;
}
