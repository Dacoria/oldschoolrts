using System;
using UnityEngine;
using UnityEngine.UI;

public class UIQueueProgressBarScript : MonoBehaviour
{
    public Image ProgressionBar;
    public Text ProgressText;

    private UiQueueHandler DisplayQueueUIHandler;


    void Awake()
    {
        DisplayQueueUIHandler = transform.parent.parent.GetComponentInChildren<UiQueueHandler>();
    }


    private int updateCounter;

    void Update()
    {
        if(updateCounter == 0)
        {
            UpdateDisplayQueueProgression();
        }
        updateCounter++;
        if(updateCounter >= 10)
        {
            updateCounter = 0;
        }
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
            var perc = (float)timeBetweenStartProcessAndNow / DisplayQueueUIHandler.CallingQueueForBuildingBehaviour.BuildTimeInSeconds;
            return Math.Min(perc, 1);
        }

        return -1;
    }
}
