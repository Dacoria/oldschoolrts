using UnityEngine;

public class TooltipWorker : MonoBehaviourCI, ITooltipUIText
{
    [ComponentInject] private WorkManager WorkManager;

    [ComponentInject] private HealthBehaviour HealthBehaviour;

    [ComponentInject(Required.OPTIONAL)]
    private FoodConsumptionBehaviour FoodConsumptionBehaviour;

    void Start()
    {
        gameObject.AddComponent<TooltipUIHandler>(); // regelt het tonen vd juiste text + gedrag -> via ITooltipUIText
    }

    public string GetHeaderText() => WorkManager.VillagerUnitType.ToString().Capitalize();
    public string GetContentText()
    {
        var content = "";        

        content += $"Current Health: {HealthBehaviour.CurrentHealth}";
        content += "\n";
        if (FoodConsumptionBehaviour != null)
        {
            content += $"Food%: {Mathf.Round(FoodConsumptionBehaviour.FoodSatisfactionPercentage * 100)}%";
            content += "\n";
            if (FoodConsumptionBehaviour.Status == FoodConsumptionStatus.NEEDS_REFILL ||
                FoodConsumptionBehaviour.Status == FoodConsumptionStatus.GO_TOWARDS_REFILLL_POINT ||
                FoodConsumptionBehaviour.Status == FoodConsumptionStatus.IS_REFILLING)
            {
                content += $"Food status: {FoodConsumptionBehaviour.Status}";
                content += "\n";
            }
        }

        var currentWorkAction = WorkManager.GetActiveWorkAction();
        if (currentWorkAction == null)
        {
            content += "No action";
        }
        else{
            content += $"Action: {currentWorkAction.GetType().ToString().Replace("Behaviour", "")}";
        }

        return content;
    }

    private string GetDisplayName(string name)
    {
        return name.Replace("Prefab", "");
    }
}