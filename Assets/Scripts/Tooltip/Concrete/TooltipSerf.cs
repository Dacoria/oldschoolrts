using UnityEngine;

public class TooltipSerf : MonoBehaviourCI, ITooltipUIText
{
    [ComponentInject]
    private SerfBehaviour SerfBehaviour;

    [ComponentInject]
    private HealthBehaviour HealthBehaviour;

    [ComponentInject(Required.OPTIONAL)]
    private FoodConsumptionBehaviour FoodConsumptionBehaviour;

    public void Start()
    {
        gameObject.AddComponent<TooltipUIHandler>(); // regelt het tonen vd juiste text + gedrag -> via ITooltipUIText
    }

    public string GetHeaderText() => "Serf";
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

        if (SerfBehaviour._currentSerfOrder == null)
        {
            content += "Order status: None";
        }
        else
        {
            var order = SerfBehaviour._currentSerfOrder;
            content += "== Order ==";
            content += "\n";
            content += $"Type: {order.ItemType}";
            content += "\n";
            content += $"From: {GetDisplayName(order.From.GameObject.name)}";
            content += "\n";
            content += $"To: {GetDisplayName(order.To.GameObject.name)}";
            content += "\n";
            content += $"Status: {order.Status}";
        }

        return content;
    }

    private string GetDisplayName(string name)
    {
        return name.Replace("Prefab", "");
    }
}