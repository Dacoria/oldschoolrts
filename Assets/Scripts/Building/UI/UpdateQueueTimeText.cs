using System;
using TMPro;
using UnityEngine;

public class UpdateQueueTimeText : MonoBehaviour
{
    public TextMeshPro TimeRemainingText;

    [ComponentInject(Required.OPTIONAL)] // optioneel; heb je niks, toon dan niks
    private QueueForBuildingBehaviour QueueForBuildingBehaviour;

    public void Awake()
    {
        this.ComponentInject();
    }   

    void Update()
    {        
        if (QueueForBuildingBehaviour != null && QueueForBuildingBehaviour.GetCurrentItemProcessed() != null)
        {
            var totalBuildTimeInSeconds = QueueForBuildingBehaviour.BuildTimeInSeconds;
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
