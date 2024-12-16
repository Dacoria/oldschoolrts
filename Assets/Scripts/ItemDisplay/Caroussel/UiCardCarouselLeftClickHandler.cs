using UnityEngine;
using UnityEngine.EventSystems;

public class UiCardCarouselLeftClickHandler : MonoBehaviour, IPointerClickHandler
{
    [ComponentInject] private CardUICarouselHandler CardUICarouselHandler;

    void Awake()
    {
        this.ComponentInject();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            CardUICarouselHandler.CarouselLeft_LeftClick();
        else if (eventData.button == PointerEventData.InputButton.Middle)
            CardUICarouselHandler.CarouselLeft_MiddleClick();
    }
}