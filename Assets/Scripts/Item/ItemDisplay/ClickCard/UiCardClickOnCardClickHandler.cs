using UnityEngine.EventSystems;

public class UiCardClickOnCardClickHandler : MonoBehaviourCI, IPointerClickHandler
{
    [ComponentInject]
    public IUiCardLeftClick UiWrapperBehaviour;

    // BUTTON
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UiWrapperBehaviour.ClickOnCardLeftClick();
        }
    }
}