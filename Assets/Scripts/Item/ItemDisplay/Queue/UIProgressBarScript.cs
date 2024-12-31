using System;
using UnityEngine;
using UnityEngine.UI;

public class UIProgressBarScript : MonoBehaviourSlowUpdateFramesCI
{
    public Image ProgressionBar;
    public Text ProgressText;
    public IProcesOneItemUI procesOneItemUI;

    void Start()
    {
        procesOneItemUI = MonoHelper.Instance.FindChildComponentInParents<IProcesOneItemUI>(gameObject);
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
        var item = procesOneItemUI.GetCurrentItemProcessed();
        if (item != null && item.IsBeingBuild)
        {
            var timeBetweenStartProcessAndNow = (DateTime.Now - item.StartTimeBeingBuild.Value).TotalSeconds;
            var perc = (float)timeBetweenStartProcessAndNow / procesOneItemUI.GetBuildTimeInSeconds(procesOneItemUI.GetCurrentItemProcessed().Type);
            return Math.Min(perc, 1);
        }

        return -1;
    }
}