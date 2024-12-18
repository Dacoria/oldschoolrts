using System;
using UnityEngine;

public class TooltipHarvestableMaterialScript : MonoBehaviourCI, ITooltipUIText
{
    [ComponentInject]
    private HarvestableMaterialScript HarvestableMaterialScript;

    [ComponentInject(Required.OPTIONAL)]
    private RecoverHarvestableMaterialScript RecoverHarvestableMaterialScript;

    [ComponentInject(Required.OPTIONAL)]
    private GameObjectGrowScript GameObjectGrowScript;

    public string HeaderOptional;

    public void Start()
    {
        this.gameObject.AddComponent<TooltipUIHandler>();
    }

    public string GetContentText()
    {
        var content = "";
        content += $"Type: {HarvestableMaterialScript.MaterialType.ToString().Capitalize()}";
        content += "\n";
        content += $"Amount left: {HarvestableMaterialScript.MaterialCount}";

        if(HarvestableMaterialScript.ResourceIsBeingRetrieved)
        {
            content += "\n";
            content += "Status: Is now being harvested";
        }

        if (RecoverHarvestableMaterialScript != null && RecoverHarvestableMaterialScript.isRefillingResources)
        {
            var timeSpendInSec = (DateTime.Now - RecoverHarvestableMaterialScript.StartDateRefilling.Value).TotalSeconds;
            var progress = Math.Min(1, timeSpendInSec / (float)RecoverHarvestableMaterialScript.RecoveryTimeInSeconds);

            content += "\n";
            content += $"Refill resources; progress: {Math.Round(progress * 100).ToString()}%";
            content += "\n";
            content += $"Recovery amount: {RecoverHarvestableMaterialScript.RecoverAmount}";
        }

        if (GameObjectGrowScript != null)
        {
            if (GameObjectGrowScript.HasReachedGrowthTarget())
            {
                if(!HarvestableMaterialScript.ResourceIsBeingRetrieved)
                {
                    content += "\n";
                    content += "Fully grown, ready to be harvested";
                }                
            }
            else
            {
                var scaleToCover = GameObjectGrowScript.EndScale - GameObjectGrowScript.StartScale;
                var scalingDone = transform.localScale.x - GameObjectGrowScript.StartScale;

                var progress = Math.Min(1, scalingDone / (float)scaleToCover);

                content += "\n";
                content += $"Growing; progress: {Math.Round(progress * 100).ToString()}%";
            }            
        }

        return content;
    }
    public string GetHeaderText() => string.IsNullOrEmpty(HeaderOptional) ? HarvestableMaterialScript.MaterialType.ToString().Capitalize() : HeaderOptional;
  
}
