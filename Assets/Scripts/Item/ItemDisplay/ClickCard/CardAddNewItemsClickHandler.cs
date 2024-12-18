using UnityEngine;
using UnityEngine.EventSystems;

public class CardAddNewItemsClickHandler : MonoBehaviour, IPointerClickHandler
{
    public CardUiItemHandler CardUiItemHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            CardUiItemHandler.AddItemType(1);
        else if (eventData.button == PointerEventData.InputButton.Middle)
            CardUiItemHandler.AddItemType(10);
    }
}