using UnityEngine;
using UnityEngine.EventSystems;

public class UiCardAddNewItemsClickHandler : MonoBehaviour, IPointerClickHandler
{
    [ComponentInject] private IUiCardAddNewItemsClick UiCardAddNewItemsClick;

    public void Awake()
    {
        this.ComponentInject();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            UiCardAddNewItemsClick.AddAmount(1);
        else if (eventData.button == PointerEventData.InputButton.Middle)
            UiCardAddNewItemsClick.AddAmount(5);
    }
}