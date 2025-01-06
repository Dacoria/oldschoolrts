using UnityEngine;

public class HideShowBerryGo : MonoBehaviourSlowUpdateFramesCI
{
    public GameObject HasBerries;
    public GameObject NoBerries;

    [ComponentInject] private HarvestableMaterialScript harvestableMaterialScript;

    protected override int FramesTillSlowUpdate => 30;

    protected override void SlowUpdate()
    {
        if (harvestableMaterialScript.MaterialCount == 0 && !NoBerries.activeSelf)
        {
            UpdateActiveBerriesGos();
        }
        if (harvestableMaterialScript.MaterialCount > 0 && !HasBerries.activeSelf)
        {
            UpdateActiveBerriesGos();
        }
    }

    private void UpdateActiveBerriesGos()
    {
        NoBerries.SetActive(harvestableMaterialScript.MaterialCount == 0);
        HasBerries.SetActive(harvestableMaterialScript.MaterialCount != 0);
    }
}