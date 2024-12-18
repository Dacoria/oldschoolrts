using System;
using UnityEngine.UI;

public class UITavernFoodItemProgressBarScript : MonoBehaviourSlowUpdateFramesCI
{
    public Image ProgressionBar;
    public Image FullBar;

    [ComponentInject] private UiTavernRefillCardBehaviour UiTavernQueueCardBehaviour;

    protected override int FramesTillSlowUpdate => 20;

    protected override void SlowUpdate()
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