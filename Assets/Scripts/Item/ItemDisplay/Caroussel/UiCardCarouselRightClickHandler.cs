using UnityEngine.EventSystems;

public class UiCardCarouselRightClickHandler : MonoBehaviourCI, IPointerClickHandler
{
    [ComponentInject] private CardUICarouselHandler CardUICarouselHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //CardUICarouselHandler.CarouselRight_LeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            //CardUICarouselHandler.CarouselRight_MiddleClick();
        }
    }
}