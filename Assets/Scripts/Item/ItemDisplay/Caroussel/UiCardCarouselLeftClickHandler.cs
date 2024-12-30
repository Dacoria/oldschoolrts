using UnityEngine.EventSystems;

public class UiCardCarouselLeftClickHandler : MonoBehaviourCI, IPointerClickHandler
{
    [ComponentInject] private CardUICarouselHandler CardUICarouselHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //CardUICarouselHandler.CarouselLeft_LeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            //CardUICarouselHandler.CarouselLeft_MiddleClick();
        }
    }
}