using UnityEngine;
using UnityEngine.UI;

public class StockpileResourceWrapperBehaviour : MonoBehaviour, IUiCardAddNewItemsClick
{
    public ItemType ItemType;
    public Image Image;
    public Text Text;
    public StockpileUiBehaviour StockpileUiBehaviour;


    private void Start()
    {
        var tooltip = gameObject.AddComponent<TooltipTriggerCanvas>();
        tooltip.Content = ItemType.ToString().Capitalize();
    }

    public void SetAmount(int amount)
    {
        var recTrans = Text.gameObject.transform as RectTransform;
        recTrans.sizeDelta = new Vector2(50, 20);
        recTrans.localPosition = new Vector3(58, -20, 0);
        Text.text = amount.ToString();
    }

    public void AddAmount(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            StockpileUiBehaviour.IncreaseStockpileAmountOnClick(ItemType);
        }
    }
}