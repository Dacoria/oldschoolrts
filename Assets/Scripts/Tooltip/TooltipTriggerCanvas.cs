using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTriggerCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string Content;
    public string Header;


    private Vector2 PositionMouseOnStartTooltip;

    private bool activeTooltip;

    public void Update()
    {
        if(!activeTooltip)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            StopTooltip();
        }

        if(Vector2.Distance(PositionMouseOnStartTooltip, Input.mousePosition) > 50)
        {
            StopTooltip();
        }
    }

    private void StopTooltip()
    {
        TooltipSystem.instance.Hide(ignoreWaitBuffer: true);
        activeTooltip = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PositionMouseOnStartTooltip = Input.mousePosition;
        TooltipSystem.instance.Show(Content, Header);
        activeTooltip = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopTooltip();
    }
}