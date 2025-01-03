using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TavernUiWrapperBehaviour : MonoBehaviourSlowUpdateFramesCI
{
    [HideInInspector] public ItemType ItemType;
    [HideInInspector] public UiTavernBehaviour TavernUiBehaviour;

    public Image Image;
    public Text TitleText;
    public Text FoodCount;

    protected override int FramesTillSlowUpdate => 30;
    protected override void SlowUpdate()
    {
        if (TavernUiBehaviour.CallingTavern != null)
        {
            var foodCount = TavernUiBehaviour.CallingTavern.StockpileOfItemsRequired.Single(x => x.ItemType == ItemType).Amount;
            FoodCount.text = foodCount.ToString();
        }
    }
}