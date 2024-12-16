using System.Linq;
using UnityEngine;

public class FoodUIDetailsScript : MonoBehaviour
{
    public ImageTextBehaviour FoodValue;
    public ImageTextBehaviour TimeToConsumeFood;

    [ComponentInject]
    private TavernUiWrapperBehaviour TavernUiWrapperBehaviour;

    private void Awake()
    {
        this.ComponentInject();
    }    

    void Start()
    {
        var itemSettings = GameManager.Instance.ItemFoodRefillValues.Single(x => x.ItemType == TavernUiWrapperBehaviour.ItemType);
        FoodValue.Text.text = itemSettings.RefillValue * 100 + "%";
        TimeToConsumeFood.Text.text = itemSettings.TimeToConsumeInSec.ToString();
    }
}
