using UnityEngine;

public class OnlyShowWhileProducing : MonoBehaviour
{
    public GameObject GoToShowDuringProducing;

    [ComponentInject] 
    private ProduceResourceOrderBehaviour ProduceResourceBehaviour;   

    private void Start()
    {
        this.ComponentInject(); //ProduceResourceOrderBehaviour wordt later aangemaakt -> daarom in de start
        GoToShowDuringProducing.SetActive(ProduceResourceBehaviour.IsProducingResourcesRightNow);
    }

    private bool previousIsProducing;

    void Update()
    {
        var isProducing = ProduceResourceBehaviour.IsProducingResourcesRightNow;
        if(isProducing != previousIsProducing)
        {
            GoToShowDuringProducing.SetActive(isProducing);
        }

        previousIsProducing = isProducing;
    }
}
