using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardDecreaseNewItemsClickHandler : MonoBehaviour, IPointerClickHandler
{
    public CardUiItemHandler CardUiItemHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            CardUiItemHandler.DecreaseItemType(1);
        else if (eventData.button == PointerEventData.InputButton.Middle)
            CardUiItemHandler.DecreaseItemType(10);
    }
}