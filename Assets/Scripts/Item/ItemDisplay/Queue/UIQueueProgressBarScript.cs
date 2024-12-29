using System;
using UnityEngine;
using UnityEngine.UI;

public class UIQueueProgressBarScript : MonoBehaviourSlowUpdateFramesCI
{
    public Image ProgressionBar;
    public Text ProgressText;

    private UiQueueHandler DisplayQueueUIHandler;


    void Start()
    {
        DisplayQueueUIHandler = transform.parent.parent.GetComponentInChildren<UiQueueHandler>();
    }

    protected override int FramesTillSlowUpdate => 20;
    protected override void SlowUpdate()
    {
        UpdateDisplayQueueProgression();
    }

    private void UpdateDisplayQueueProgression()
    {
        var itemBeingProcessed = DisplayQueueUIHandler.GetCurrentItemProcessed();
        if(itemBeingProcessed != null)
        {
            var progressPerc = GetProgressPercentageOfItemProcessed();
            var fullPerc = Mathf.Round(progressPerc * 100);

            ProgressText.text = fullPerc.ToString() + "%";
            ProgressionBar.fillAmount = progressPerc;
        }
        else
        {
            ProgressText.text = "";
            ProgressionBar.fillAmount = 0;
        }        
    }

    private float GetProgressPercentageOfItemProcessed()
    {
        var item = DisplayQueueUIHandler.GetCurrentItemProcessed();
        if (item != null && item.IsBeingBuild)
        {
            var timeBetweenStartProcessAndNow = (DateTime.Now - item.StartTimeBeingBuild.Value).TotalSeconds;
            var perc = (float)timeBetweenStartProcessAndNow / DisplayQueueUIHandler.CallingQueueForBuildingBehaviour.GetBuildTimeInSeconds();
            return Math.Min(perc, 1);
        }

        return -1;
    }
}