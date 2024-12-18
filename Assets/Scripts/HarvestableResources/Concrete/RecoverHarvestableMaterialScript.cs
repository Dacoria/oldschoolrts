using System;
using System.Collections;

public class RecoverHarvestableMaterialScript : MonoBehaviourSlowUpdateFramesCI
{
    [ComponentInject]
    private HarvestableMaterialScript HarvestableMaterialScript;

    public int RecoverAmount;
    public int RecoveryTimeInSeconds;

    public DateTime? StartDateRefilling;
    public bool isRefillingResources => StartDateRefilling.HasValue;
        
    protected override int FramesTillSlowUpdate => 100;
    protected override void SlowUpdate()
    {
        if(HarvestableMaterialScript.MaterialCount == 0)
        {
            StartCoroutine(RecoverHarvestableMaterial());
        }
    }

    private IEnumerator RecoverHarvestableMaterial()
    {
        StartDateRefilling = DateTime.Now;
        yield return MonoHelper.Instance.GetCachedWaitForSeconds(RecoveryTimeInSeconds);
        HarvestableMaterialScript.MaterialCount = RecoverAmount;
        StartDateRefilling = null;
    }
}