using System.Linq;
using UnityEngine;

public class DisplayUnitIsSelected : MonoBehaviourSlowUpdateFramesCI
{
    private GameObject unitSelectionSphere;

    private new void Awake()
    {
        base.Awake();
        unitSelectionSphere = Instantiate(Load.GoMapUI[Constants.GO_PREFAB_UI_UNIT_SELECTION_DISPLAY], transform);
    }

    protected override int FramesTillSlowUpdate => 20;

    protected override void SlowUpdate()
    {
        unitSelectionSphere.SetActive(RtsUnitSelectionManager.Instance.CurrentSelected.GetUnits().Any(x => x == gameObject));
    }
}
