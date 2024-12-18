using UnityEngine;

public class TooltipBuilder : MonoBehaviourCI, ITooltipUIText
{
    [ComponentInject]
    private BuilderBehaviour BuilderBehaviour;

    [ComponentInject]
    private HealthBehaviour HealthBehaviour;

    [ComponentInject(Required.OPTIONAL)]
    private FoodConsumptionBehaviour FoodConsumptionBehaviour;

    public void Start()
    {
        gameObject.AddComponent<TooltipUIHandler>(); // regelt het tonen vd juiste text + gedrag -> via ITooltipUIText
    }

    public string GetHeaderText() => "Builder";
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

        if (BuilderBehaviour._currentBuilderRequest == null)
        {
            content += "Request status: None";
        }
        else
        {
            var request = BuilderBehaviour._currentBuilderRequest;
            content += "== Request ==";
            content += "\n";
            content += $"Purpose: {request.Purpose}";
            content += "\n";
            content += $"Obj to build: {GetDisplayName(request.GameObject.name)}";
            content += "\n";
            content += $"Status: {request.Status}";
        }

        return content;
    }

    private string GetDisplayName(string name)
    {
        return name.Replace("Prefab", "").Replace("(Clone)", "");
    }
}