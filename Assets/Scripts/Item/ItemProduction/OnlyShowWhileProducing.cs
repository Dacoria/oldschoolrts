using UnityEngine;

public class OnlyShowWhileProducing : MonoBehaviour
{
    public GameObject GoToShowDuringProducing;

    [ComponentInject] private ProduceCRBehaviour ProduceBehaviour;   

    private void Start()
    {
        this.ComponentInject(); //ProduceResourceOrderBehaviour wordt later aangemaakt -> daarom in de start
        GoToShowDuringProducing.SetActive(ProduceBehaviour.IsProducingResourcesRightNow);
    }

    private bool previousIsProducing;

    void Update()
    {
        var isProducing = ProduceBehaviour.IsProducingResourcesRightNow;
        if(isProducing != previousIsProducing)
        {
            GoToShowDuringProducing.SetActive(isProducing);
        }

        previousIsProducing = isProducing;
    }
}