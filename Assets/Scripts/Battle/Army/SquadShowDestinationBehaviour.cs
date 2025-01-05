using System.Collections.Generic;
using UnityEngine;

public class SquadShowDestinationBehaviour : MonoBehaviourCI
{
    public void ShowPositions(List<Vector3> positions)
    {
        foreach (var position in positions)
        {
            Instantiate(Load.GoMapUI[Constants.GO_PREFAB_UI_UNIT_DESTINATION_DISPLAY], position ,Quaternion.identity, transform);
        }
        Destroy(this);
    }
}