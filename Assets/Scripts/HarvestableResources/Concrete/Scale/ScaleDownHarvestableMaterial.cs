using UnityEngine;

public class ScaleDownHarvestableMaterial : MonoBehaviourSlowUpdateFramesCI
{
    [ComponentInject]
    private HarvestableMaterialScript harvestableMaterialScript;

    protected override int FramesTillSlowUpdate => 30;
    protected override void SlowUpdate()
    {
        var scaleMultiplier = harvestableMaterialScript.MaterialCount / (float)harvestableMaterialScript.InitialMaterialCount;
        transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
    }
}