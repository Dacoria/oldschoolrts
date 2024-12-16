using UnityEngine;
using UnityEngine.EventSystems;

public class UiCardCarouselRightClickHandler : MonoBehaviour, IPointerClickHandler
{
    [ComponentInject] private CardUICarouselHandler CardUICarouselHandler;

    void Awake()
    {
        this.ComponentInject();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            CardUICarouselHandler.CarouselRight_LeftClick();
        else if (eventData.button == PointerEventData.InputButton.Middle)
            CardUICarouselHandler.CarouselRight_MiddleClick();
    }
}