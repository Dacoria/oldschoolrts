using System;
using UnityEngine;

public class TooltipSummoner : MonoBehaviour, ITooltipUIText
{
    [ComponentInject]
    private SummonGoScript SummonGoScript;
       
    public void Awake()
    {
        this.ComponentInject();
        gameObject.AddComponent<TooltipUIHandler>(); // regelt het tonen vd juiste text + gedrag -> via ITooltipUIText
    }

    public string GetHeaderText() => "Summon pillar";
    public string GetContentText()
    {
        var content = "";

        content += $"Type summoned: {GetDisplayName(SummonGoScript.SummonPrefab.name)}";
        content += "\n";
        content += $"Spawnrate (sec): {SummonGoScript.TimeToWaitForSpawnInSeconds}";
        content += "\n";
        content += $"Spawn limit: {SummonGoScript.SpawnLimit}";
        content += "\n";
        content += $"Is summoning: {SummonGoScript.IsTryingToSummon}";

        if(SummonGoScript.IsTryingToSummon)
        {
            content += "\n";

            var timeSpendOnSummoningInSec = (DateTime.Now - SummonGoScript.TimeStartedForSpawnTimer.Value).TotalSeconds;
            var progress = Math.Min(1, timeSpendOnSummoningInSec / (float)SummonGoScript.TimeToWaitForSpawnInSeconds);
            content += $"Summon progress: {Math.Round(progress * 100).ToString()}%";
        }

        return content;
    }



    private string GetDisplayName(string name)
    {
        return name.Replace("Prefab", "");
    }
}
