using System;
using TMPro;

public class UpdateQueueTimeText : MonoBehaviourCI
{
    public TextMeshPro TimeRemainingText;

    [ComponentInject(Required.OPTIONAL)] // optioneel; heb je niks, toon dan niks
    private QueueForBuildingBehaviour QueueForBuildingBehaviour;

    void Update()
    {        
        if (QueueForBuildingBehaviour != null && QueueForBuildingBehaviour.GetCurrentItemProcessed() != null)
        {
            var totalBuildTimeInSeconds = QueueForBuildingBehaviour.GetBuildTimeInSeconds();
            var timeProducingInMs =  (DateTime.Now - QueueForBuildingBehaviour.GetCurrentItemProcessed().StartTimeBeingBuild.Value).TotalMilliseconds / 1000f;
            var percShown = Math.Min(Math.Round(timeProducingInMs / totalBuildTimeInSeconds * 100), 99) + "%";
            TimeRemainingText.SetText(percShown);
        }
        else
        {
            TimeRemainingText.SetText("");
        }        
    }
}