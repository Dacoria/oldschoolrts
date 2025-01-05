using UnityEngine.EventSystems;

public class UiCardAddNewItemsClickHandler : MonoBehaviourCI, IPointerClickHandler
{
    [ComponentInject] private IUiCardAddItemClick UiCardAddNewItemsClick;

    // BUTTON
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UiCardAddNewItemsClick.AddAmount(1);
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            UiCardAddNewItemsClick.AddAmount(10);
        }
    }
}