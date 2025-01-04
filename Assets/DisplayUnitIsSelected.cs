using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisplayUnitIsSelected : MonoBehaviourSlowUpdateFramesCI
{
    private GameObject unitSelectionSphere;

    private new void Awake()
    {
        base.Awake();
        unitSelectionSphere = Instantiate(Load.GoMap["UnitSelectionSphere"], transform);
    }

    protected override int FramesTillSlowUpdate => 20;

    protected override void SlowUpdate()
    {
        unitSelectionSphere.SetActive(RtsUnitManager.Instance.CurrentSelected.Units.Any(x => x == gameObject));
    }
}
