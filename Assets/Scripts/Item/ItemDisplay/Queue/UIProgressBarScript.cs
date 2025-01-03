using System;
using UnityEngine;
using UnityEngine.UI;

public class UIProgressBarScript : MonoBehaviourSlowUpdateFramesCI
{
    public Image ProgressionBar;
    public IProcesOneItemUI procesOneItemUI;

    void Start()
    {
        procesOneItemUI = MonoHelper.Instance.FindChildComponentInParents<IProcesOneItemUI>(gameObject);
    }

    private void OnEnable()
    {
        ProgressionBar.fillAmount = 0;
    }

    protected override int FramesTillSlowUpdate => 20;
    protected override void SlowUpdate()
    {
        UpdateDisplayQueueProgression();
    }

    private void UpdateDisplayQueueProgression()
    {
        var itemBeingProcessed = procesOneItemUI.GetCurrentItemProcessed();
        if(itemBeingProcessed != null)
        {
            var progressPerc = GetProgressPercentageOfItemProcessed(itemBeingProcessed);
            var fullPerc = Mathf.Round(progressPerc * 100);

            ProgressionBar.fillAmount = progressPerc;
        }              
    }

    private float GetProgressPercentageOfItemProcessed(TypeProcessing item)
    {
        if (item != null)
        {
            var timeBetweenStartProcessAndNow = (DateTime.Now - item.StartTimeBeingBuild).TotalSeconds;
            var perc = (float)timeBetweenStartProcessAndNow / procesOneItemUI.GetBuildTimeInSeconds();
            return Math.Min(perc, 1);
        }

        return -1;
    }
}