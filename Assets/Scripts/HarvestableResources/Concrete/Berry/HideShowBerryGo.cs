using UnityEngine;

public class HideShowBerryGo : MonoBehaviourSlowUpdateFramesCI
{
    public GameObject HasBerries;
    public GameObject NoBerries;

    [ComponentInject]
    private HarvestableMaterialScript HarvestableMaterialScript;

    protected override int FramesTillSlowUpdate => 100;

    protected override void SlowUpdate()
    {
        if (HarvestableMaterialScript.MaterialCount == 0 && !NoBerries.activeSelf)
        {
            UpdateActiveBerriesGos();
        }
        if (HarvestableMaterialScript.MaterialCount > 0 && !HasBerries.activeSelf)
        {
            UpdateActiveBerriesGos();
        }
    }

    private void UpdateActiveBerriesGos()
    {
        NoBerries.SetActive(HarvestableMaterialScript.MaterialCount == 0);
        HasBerries.SetActive(HarvestableMaterialScript.MaterialCount != 0);
    }
}