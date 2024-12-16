using Assets;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TavernUiWrapperBehaviour : MonoBehaviour
{
    [HideInInspector]
    public ItemType ItemType;

    [HideInInspector]
    public UiTavernBehaviour TavernUiBehaviour;

    public Image Image;
    public Text TitleText;
    public Text FoodCount;

    private int updateCounter;

    public void Update()
    {
        if(updateCounter == 0 && TavernUiBehaviour.CallingTavern != null)
        {
            var foodCount = TavernUiBehaviour.CallingTavern.StockpileOfItemsRequired.Single(x => x.ItemType == ItemType).Amount;
            FoodCount.text = foodCount.ToString();
        }

        updateCounter++;
        if(updateCounter > 30)
        {
            updateCounter = 0;
        }

    }

}