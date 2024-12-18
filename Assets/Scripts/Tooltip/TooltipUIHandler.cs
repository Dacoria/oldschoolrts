using System;
using UnityEngine;

public class TooltipUIHandler : BaseAEMonoCI
{
    private bool toolTipIsActive;
    private OutlineBehaviour OutlineComponent;

    [ComponentInject]
    public ITooltipUIText CallingTooltipBahaviour;


    public void OnDestroy()
    {
        if (toolTipIsActive)
        {
            RemoveTooltip();
        }
    }

    protected override void OnLeftClickOnGo(GameObject go)
    {
        if(go == transform?.gameObject &&
            !toolTipIsActive 
            && (DateTime.Now - TimeTooltipRemoved).TotalMilliseconds > 100)
        {
            var header = CallingTooltipBahaviour.GetHeaderText();
            var content = CallingTooltipBahaviour.GetContentText();
            TooltipSystem.instance.Show(content, header, waitBeforeShowing: false, activeTooltipGo: gameObject);

            toolTipIsActive = true;

            OutlineComponent = gameObject.GetComponent<OutlineBehaviour>(); ;

            if(OutlineComponent == null)
            {
                OutlineComponent = gameObject.AddComponent<OutlineBehaviour>();
                OutlineComponent.OutlineColor = new Color(23 / 255f, 171 / 255f, 178 / 255f);
                OutlineComponent.OutlineWidth = 4.5f;
            }
            TimeTooltipShown = DateTime.Now;

        }
    }

    private DateTime TimeTooltipRemoved;
    private DateTime TimeTooltipShown;

    private void RemoveTooltip()
    {
        if(OutlineComponent != null)
        {
            Destroy(OutlineComponent);
        }
        TooltipSystem.instance.Hide();
        toolTipIsActive = false;
        TimeTooltipRemoved = DateTime.Now;
    }

    private int updateCounter;

    public void Update()
    {
        if (!toolTipIsActive)
        {
            return;
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) 
            && (DateTime.Now - TimeTooltipShown).TotalMilliseconds > 20)
        {
            RemoveTooltip();            
        }

        if (updateCounter == 0)
        {
            UpdateTooltipText();
        }

        updateCounter++;
        if (updateCounter > 50)
        {
            updateCounter = 0;
        }
    }
    
    private void UpdateTooltipText()
    {
        var header = CallingTooltipBahaviour.GetHeaderText();
        var content = CallingTooltipBahaviour.GetContentText();
        var updateTextSuccesfull = TooltipSystem.instance.UpdateText(gameObject, content, header);

        if(!updateTextSuccesfull)
        {
            RemoveTooltip();
        }
    }  
}
