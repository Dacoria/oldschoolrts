using System.Collections;
using UnityEngine;

public class OnlyShowWhileProducing : MonoBehaviour
{
    public GameObject GoToShowDuringProducing;

    [ComponentInject(Required.OPTIONAL) ] private ProduceCRBehaviour produceBehaviour;   

    private IEnumerator Start()
    {
        while (true)
        {
            this.ComponentInject(); //ProduceResourceOrderBehaviour wordt later aangemaakt -> daarom in de start
            if (produceBehaviour != null)
            {
                break;
            }
            else
            {
                yield return Wait4Seconds.Get(0.2f);
            }            
        }
        GoToShowDuringProducing.SetActive(produceBehaviour.IsProducingResourcesRightNow);
    }

    private bool previousIsProducing;

    void Update()
    {
        if(produceBehaviour == null)
        {
            return;
        }

        var isProducing = produceBehaviour.IsProducingResourcesRightNow;
        if(isProducing != previousIsProducing)
        {
            GoToShowDuringProducing.SetActive(isProducing);
        }

        previousIsProducing = isProducing;
    }
}