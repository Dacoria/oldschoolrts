using System;
using UnityEngine;
using UnityEngine.UI;

public class UITavernFoodItemProgressBarScript : MonoBehaviour
{
    public Image ProgressionBar;
    public Image FullBar;

    [ComponentInject]
    private UiTavernRefillCardBehaviour UiTavernQueueCardBehaviour;

    void Awake()
    {
        this.ComponentInject();
    }


    private int updateCounter;

    void Update()
    {
        if (updateCounter == 0)
        {
            if(UiTavernQueueCardBehaviour.SelectedTavernQueueItem != null)
            {
                ProgressionBar.enabled = true;
                FullBar.enabled = true;
                UpdateDisplayQueueProgression();
            }
            else
            {
                ProgressionBar.enabled = false;
                FullBar.enabled = false;
            }            
        }
        updateCounter++;
        if (updateCounter >= 10)
        {
            updateCounter = 0;
        }
    }

    private void UpdateDisplayQueueProgression()
    {
        var itemBeingProcessed = UiTavernQueueCardBehaviour.SelectedTavernQueueItem;
        if (itemBeingProcessed != null)
        {
            var progressPerc = GetProgressPercentageOfItemProcessed();
            ProgressionBar.fillAmount = progressPerc;
        }
        else
        {
            ProgressionBar.fillAmount = 0;
        }
    }

    private float GetProgressPercentageOfItemProcessed()
    {
        var item = UiTavernQueueCardBehaviour.SelectedTavernQueueItem;
        if (item != null && item.IsConsumingFood)
        {
            var timeBetweenStartProcessAndNow = (DateTime.Now - item.StartTimeConsumingFood.Value).TotalSeconds;

            var perc = (float)timeBetweenStartProcessAndNow / item.TimeToConsumeFoodInSeconds;
            return Math.Min(perc, 1);
        }

        return -1;
    }
}
