using UnityEngine;
using UnityEngine.EventSystems;

public partial class UiManager : MonoBehaviour
{
    private void ActiveRangedDisplay(GameObject buildingHit)
    {
        var resourceRangeBuilding1 = buildingHit.transform.GetComponentInChildren<RangeDisplayBehaviour>(true);
        var resourceRangeBuilding2 = buildingHit.transform.GetComponent<RangeDisplayBehaviour>();
        var resourceRangeBuilding = resourceRangeBuilding1 != null ? resourceRangeBuilding1 : resourceRangeBuilding2;

        if (resourceRangeBuilding != null)
        {
            ActiveRangeDisplayBehaviour = resourceRangeBuilding;
            resourceRangeBuilding.gameObject.SetActive(true);
        }
    }
}