using System;
using System.Collections;

public class RecoverHarvestableMaterialScript : MonoBehaviourSlowUpdateFramesCI
{
    [ComponentInject]
    private HarvestableMaterialScript harvestableMaterialScript;

    public int RecoverAmount;
    public int RecoveryTimeInSeconds;

    public DateTime? StartDateRefilling;
    public bool IsRefillingResources => StartDateRefilling.HasValue;
        
    protected override int FramesTillSlowUpdate => 30;
    protected override void SlowUpdate()
    {
        if(harvestableMaterialScript.MaterialCount == 0)
        {
            StartCoroutine(RecoverHarvestableMaterial());
        }
    }

    private IEnumerator RecoverHarvestableMaterial()
    {
        StartDateRefilling = DateTime.Now;
        yield return Wait4Seconds.Get(RecoveryTimeInSeconds);
        harvestableMaterialScript.MaterialCount = RecoverAmount;
        StartDateRefilling = null;
    }
}