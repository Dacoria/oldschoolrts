using UnityEngine;
using UnityEngine.EventSystems;

public class UiCardClickOnCardClickHandler : MonoBehaviour, IPointerClickHandler
{
    [ComponentInject]
    public IUiCardLeftClick UiWrapperBehaviour;

    private void Awake()
    {
        this.ComponentInject();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UiWrapperBehaviour.ClickOnCardLeftClick();
        }
    }
}