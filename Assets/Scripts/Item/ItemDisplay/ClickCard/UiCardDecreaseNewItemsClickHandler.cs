using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UiCardDecreaseNewItemsClickHandler : MonoBehaviour, IPointerClickHandler
{
    public UiCardBehaviour UiCardBehaviour;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            UiCardBehaviour.DecreaseAmount(1);
        else if (eventData.button == PointerEventData.InputButton.Middle)
            UiCardBehaviour.DecreaseAmount(5);
    }
}