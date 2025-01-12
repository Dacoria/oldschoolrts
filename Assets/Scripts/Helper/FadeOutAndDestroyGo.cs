using System;
using UnityEngine;

public class FadeOutAndDestroyGo : MonoBehaviourSlowUpdateFramesCI
{
    public int WaitBeforeFadeDuration;
    public int FadeDuration;

    [ComponentInject] private Renderer ciRenderer;

    private DateTime startTime;    

    private void Start()
    {
        startTime = DateTime.Now;
    }

    protected override int FramesTillSlowUpdate => 20;
    protected override void SlowUpdate()
    {
        if (DateTime.Now <= startTime.AddSeconds(WaitBeforeFadeDuration))
            return;        

        var timeSpendFadingInMs = (float)(DateTime.Now - (startTime.AddSeconds(WaitBeforeFadeDuration))).TotalMilliseconds;
        var partFading = timeSpendFadingInMs / (FadeDuration * 1000f);
        var a = 1 - partFading;
        if (a < 0)
        {
            Destroy(gameObject);
        }
        else
        {
            ciRenderer.material.color = ciRenderer.material.color.SetA(a);
        }
    }
}
