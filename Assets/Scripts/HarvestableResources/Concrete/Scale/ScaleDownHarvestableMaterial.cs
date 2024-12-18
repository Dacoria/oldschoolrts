using UnityEngine;

public class ScaleDownHarvestableMaterial : MonoBehaviourSlowUpdateFramesCI
{
    [ComponentInject]
    private HarvestableMaterialScript HarvestableMaterialScript;

    protected override int FramesTillSlowUpdate => 30;
    protected override void SlowUpdate()
    {
        var scaleMultiplier = HarvestableMaterialScript.MaterialCount / (float)HarvestableMaterialScript.InitialMaterialCount;
        transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
    }
}