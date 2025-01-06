using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTriggerCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string Content;
    public string Header;


    private Vector2 positionMouseOnStartTooltip;

    private bool activeTooltip;

    public void Update()
    {
        // voor de zekerheid altijd doen - zo kom je altijd vd tooltips af (vb: on hover stockpile -> rechter muisknop op item na hover)
        if (Input.GetMouseButtonDown(1))
        {
            StopTooltip();
        }

        if (!activeTooltip)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            StopTooltip();
        }

        if(Vector2.Distance(positionMouseOnStartTooltip, Input.mousePosition) > 50)
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
        positionMouseOnStartTooltip = Input.mousePosition;
        TooltipSystem.instance.Show(Content, Header);
        activeTooltip = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopTooltip();
    }
}