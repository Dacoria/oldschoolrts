using System.Linq;

public class FoodUIDetailsScript : MonoBehaviourCI
{
    public ImageTextBehaviour FoodValue;
    public ImageTextBehaviour TimeToConsumeFood;

    [ComponentInject] private TavernUiWrapperBehaviour TavernUiWrapperBehaviour;

    void Start()
    {
        var itemSettings = GameManager.Instance.ItemFoodRefillValues.Single(x => x.ItemType == TavernUiWrapperBehaviour.ItemType);
        FoodValue.Text.text = itemSettings.RefillValue * 100 + "%";
        TimeToConsumeFood.Text.text = itemSettings.TimeToConsumeInSec.ToString();
    }
}