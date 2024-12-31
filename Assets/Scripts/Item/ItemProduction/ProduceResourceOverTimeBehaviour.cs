using System.Collections.Generic;

public class ProduceResourceOverTimeBehaviour : ProduceResourceAbstract
{
    private HandleProduceResourceOrderOverTimeBehaviour handleProduceResourceOrderOverTimeBehaviour;

    private new void Awake()
    {
        base.Awake();
        gameObject.AddComponent<ValidComponents>().DoCheck(
            inactives: new List<System.Type> { typeof(HandleProduceResourceOrderOverTimeBehaviour) });

        handleProduceResourceOrderOverTimeBehaviour = gameObject.AddComponent<HandleProduceResourceOrderOverTimeBehaviour>();
    }
}