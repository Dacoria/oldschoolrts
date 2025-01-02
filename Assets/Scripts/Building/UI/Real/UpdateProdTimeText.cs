using System;
using System.Linq;
using TMPro;

public class UpdateProdTimeText : MonoBehaviourCI
{
    public TextMeshPro TimeRemainingText;

    [ComponentInject(Required.OPTIONAL)] // optioneel; heb je niks, toon dan niks
    private ProduceCRBehaviour ProduceBehaviour;

    void Start()
    {
        if (ProduceBehaviour != null)
        {
            TimeRemainingText.transform.gameObject.SetActive(true); // Workaround --> direct enablen zorgt ervoor dat de text blokken wordt...... bug Unity
        }        
    }

    void Update()
    {        
        if (ProduceBehaviour != null && ProduceBehaviour.IsProducingResourcesRightNow)
        {
            var producingTimeInSeconds = ProduceBehaviour.ProduceDurations.TimeToProduceResourceInSeconds;
            var timeProducingInMs = producingTimeInSeconds - (DateTime.Now - ProduceBehaviour.CurrentTypesProcessed.First().StartTimeBeingBuild).TotalMilliseconds / 1000f;
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