using UnityEngine;
using UnityEngine.EventSystems;

public class UiCardDecreaseNewItemsClickHandler : MonoBehaviourCI, IPointerClickHandler
{
    [ComponentInject] private IUiCardDecreaseItemClick UiCardDecreaseItemClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UiCardDecreaseItemClick.DecreaseAmount(1);
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            UiCardDecreaseItemClick.DecreaseAmount(5);
        }
    }
}