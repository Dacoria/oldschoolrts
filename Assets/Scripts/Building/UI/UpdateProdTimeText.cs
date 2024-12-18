using System;
using TMPro;
using UnityEngine;

public class UpdateProdTimeText : MonoBehaviourCI
{
    public TextMeshPro TimeRemainingText;

    [ComponentInject(Required.OPTIONAL)] // optioneel; heb je niks, toon dan niks
    private ProduceResourceOrderBehaviour ProduceResourceBehaviour;

    void Start()
    {
        if (ProduceResourceBehaviour != null)
        {
            TimeRemainingText.transform.gameObject.SetActive(true); // Workaround --> direct enablen zorgt ervoor dat de text blokken wordt...... bug Unity
        }        
    }

    void Update()
    {        
        if (ProduceResourceBehaviour != null && ProduceResourceBehaviour.IsProducingResourcesOverTime() && ProduceResourceBehaviour.IsProducingResourcesRightNow)
        {
            var producingTimeInSeconds = ProduceResourceBehaviour.ProduceOverTime.GetTimeToProduceResourceInSeconds();
            var timeProducingInMs = producingTimeInSeconds - (DateTime.Now - ProduceResourceBehaviour.StartTimeProducing).TotalMilliseconds / 1000f;
            TimeRemainingText.SetText(timeProducingInMs.ToString("F1") + " sec");
        }
        else
        {
            TimeRemainingText.SetText("");
        }        
    }
}
