using System;
using TMPro;

public class UpdateProdTimeText : MonoBehaviourCI
{
    public TextMeshPro TimeRemainingText;

    [ComponentInject(Required.OPTIONAL)] // optioneel; heb je niks, toon dan niks
    private HandleProduceResourceOrderOverTimeBehaviour ProduceResourceBehaviour;

    void Start()
    {
        if (ProduceResourceBehaviour != null)
        {
            TimeRemainingText.transform.gameObject.SetActive(true); // Workaround --> direct enablen zorgt ervoor dat de text blokken wordt...... bug Unity
        }        
    }

    void Update()
    {        
        if (ProduceResourceBehaviour != null && ProduceResourceBehaviour.IsProducingResourcesRightNow)
        {
            var producingTimeInSeconds = ProduceResourceBehaviour.ProductionTimeItemBeingProduced;
            var timeProducingInMs = producingTimeInSeconds - (DateTime.Now - ProduceResourceBehaviour.StartTimeProducing).TotalMilliseconds / 1000f;
            if (timeProducingInMs >= 0)
            {
                TimeRemainingText.SetText(timeProducingInMs.ToString("F1") + " sec");
            }
            else
            {
                TimeRemainingText.SetText("");
            }
        }
        else
        {
            TimeRemainingText.SetText("");
        }        
    }
}